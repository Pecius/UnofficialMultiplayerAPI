using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Multiplayer.Client;
using Multiplayer.Common;
using RimWorld;
using RimWorld.Planet;
using UnofficialMultiplayerAPI;
using Verse;
using Log = UnofficialMultiplayerAPI_UTILS.Log;

namespace UnofficialMultiplayerAPIHost
{
	public class SyncerEntry
	{
		public delegate bool syncerDelegateRef(SyncWorker sync, ref object obj);

		public Type type;
		//public syncerDelegateRef syncer;
		private List<syncerDelegateRef> syncers;
		public bool shouldConstruct;
		private List<SyncerEntry> subclasses;

		public int SyncerCount => syncers.Count();

		public SyncerEntry(Type type, bool initSubclasses = true, bool shouldConstruct = true)
		{
			this.type = type;
			//this.syncer = syncer;
			this.syncers = new List<syncerDelegateRef>();
			this.shouldConstruct = shouldConstruct;

			if (initSubclasses)
				subclasses = new List<SyncerEntry>();
		}

		public SyncerEntry(SyncerEntry other)
		{
			type = other.type;
			//syncer = other.syncer;
			syncers = other.syncers;
			subclasses = other.subclasses;
			shouldConstruct = other.shouldConstruct;
		}

		public void AddSyncer(syncerDelegateRef syncer, bool append = true)
		{
			if (append)
				syncers.Add(syncer);
			else
				syncers.Insert(0, syncer);
		}

		public bool Invoke(SyncWorker worker, ref object obj)
		{
			for(int i = 0; i < syncers.Count; i++)
			{
				if (syncers[i](worker, ref obj))
					return true;

				worker.Reset();
			}

			return false;
		}

		public SyncerEntry Add(SyncerEntry other)
		{
			SyncerEntry newEntry = InternalAdd(other.type, null, other.shouldConstruct);

			newEntry.subclasses = other.subclasses;

			return newEntry;
		}

		public SyncerEntry Add(Type type, bool shouldConstruct = true)
		{
			return InternalAdd(type, null, shouldConstruct);
		}

		private SyncerEntry InternalAdd(Type type, SyncerEntry parent, bool shouldConstruct)
		{
			if(type == this.type)
			{
				if (shouldConstruct)
					this.shouldConstruct = true;

				return this;
			}
			else if (type.IsAssignableFrom(this.type))   // Is parent
			{
				SyncerEntry newEntry;

				if (parent != null)
				{
					List<SyncerEntry> ps = parent.subclasses;
					newEntry = new SyncerEntry(type, shouldConstruct);
					newEntry.subclasses.Add(this);

					ps[ps.IndexOf(this)] = newEntry;
					return newEntry;
				}
				else
				{
					newEntry = new SyncerEntry(this);
					this.type = type;
					//this.syncer = syncer;
					this.shouldConstruct = shouldConstruct;

					syncers = new List<syncerDelegateRef>();
					subclasses = new List<SyncerEntry>() { newEntry };
					return this;
				}
			}
			else if (this.type.IsAssignableFrom(type)) // Is child
			{
				for (int i = 0; i < subclasses.Count; i++)
				{
					SyncerEntry res = subclasses[i].InternalAdd(type, this, shouldConstruct);
					if (res != null)
						return res;
				}

				var newEntry = new SyncerEntry(type, shouldConstruct);
				subclasses.Add(newEntry);

				return newEntry;
			}

			return null;
		}

		public SyncerEntry GetClosest(Type type)
		{
			if (this.type.IsAssignableFrom(type))
			{
				int len = subclasses.Count;

				if (len == 0)
					return this;

				for (int i = 0; i < len; i++)
				{
					SyncerEntry res = subclasses[i].GetClosest(type);

					if (res != null)
						return res;
				}

				return this;
			}

			return null;
		}

		public string PrintStructure()
		{
			StringBuilder builder = new StringBuilder();

			PrintStructureInternal(0, builder);

			return builder.ToString();
		}

		private void PrintStructureInternal(int level, StringBuilder str)
		{
			str.Append(' ', 4 * level);
			str.AppendLine(type.ToString());

			for (int i = 0; i < subclasses.Count; i++)
				subclasses[i].PrintStructureInternal(level + 1, str);
		}
	}

	class SyncerDictionary
	{
		private readonly Dictionary<Type, SyncerEntry> explicitEntries = new Dictionary<Type, SyncerEntry>();
		private readonly List<SyncerEntry> implicitEntries = new List<SyncerEntry>();
		private readonly List<SyncerEntry> interfaceEntries = new List<SyncerEntry>();

		public SyncerEntry Add(Type type, bool isImplicit = false, bool shouldConstruct = true)
		{
			if (TryGetValue(type, out SyncerEntry entry))
				return entry;

			if (!isImplicit)
			{
				entry = new SyncerEntry(type, false, shouldConstruct);

				explicitEntries.Add(type, entry);
				return entry;
			}

			if (type.IsInterface)
			{
				entry = new SyncerEntry(type, false, shouldConstruct);

				interfaceEntries.Add(entry);
				return entry;
			}

			//SyncerEntry entry = null;

			Stack<SyncerEntry> toRemove = new Stack<SyncerEntry>();

			foreach (var e in implicitEntries)
			{
				//Console.WriteLine($"{type} {e.type} parent: {type.IsAssignableFrom(e.type)} child: {e.type.IsAssignableFrom(type)}");

				if (type.IsAssignableFrom(e.type) || e.type.IsAssignableFrom(type))
				{
					if (entry != null)
					{
						entry.Add(e);
						//Console.WriteLine($"{e.type} >> {type}");
						toRemove.Push(e);
						continue;
					}
					//Console.WriteLine($"{type} => {e.type}");
					entry = e.Add(type);
				}
			}

			if (entry == null)
			{
				//Console.WriteLine($"*{type}");
				entry = new SyncerEntry(type, shouldConstruct: shouldConstruct);
				implicitEntries.Add(entry);
				return entry;
			}

			foreach (var e in toRemove)
			{
				implicitEntries.Remove(e);
			}

			return entry;
		}

		public bool TryGetValue(Type type, out SyncerEntry syncer)
		{
			explicitEntries.TryGetValue(type, out syncer);

			if (syncer != null)
				return true;

			foreach (var e in implicitEntries)
			{
				syncer = e.GetClosest(type);

				if (syncer != null)
					return true;
			}

			foreach (var e in interfaceEntries)
			{
				syncer = e.GetClosest(type);

				if (syncer != null)
					return true;
			}

			return false;
		}

		public bool Contains(Type type)
		{
			return TryGetValue(type, out var _);
		}

		/*public SyncerEntry.syncerDelegateRef this[Type type]
		{
			get
			{

			}
		}*/
	}

	internal static class TypeRWHelper
	{
		private static Dictionary<Type, Type[]> cache = new Dictionary<Type, Type[]>();

		static TypeRWHelper()
		{
			cache[typeof(IStoreSettingsParent)] = Sync.storageParents;
			cache[typeof(IPlantToGrowSettable)] = Sync.plantToGrowSettables;

			cache[typeof(ThingComp)]			= Sync.thingCompTypes;
			cache[typeof(Designator)]			= Sync.designatorTypes;
			cache[typeof(WorldObjectComp)]		= Sync.worldObjectCompTypes;

			cache[typeof(GameComponent)]		= Sync.gameCompTypes;
			cache[typeof(WorldComponent)]		= Sync.worldCompTypes;
			cache[typeof(MapComponent)]			= Sync.mapCompTypes;
		}

		internal static void FlushCache()
		{
			cache.Clear();
		}

		internal static Type[] GenTypeCache(Type type)
		{
			var types = GenTypes.AllTypes
				.Where(t => t != type && type.IsAssignableFrom(t))
				.OrderBy(t => t.IsInterface)
				.ToArray();

			cache[type] = types;
			return types;
		}

		internal static Type GetType(ushort index, Type baseType)
		{
			if(!cache.TryGetValue(baseType, out Type[] types))
				types = GenTypeCache(baseType);

			return types[index];
		}

		internal static ushort GetTypeIndex(Type type, Type baseType)
		{
			if (!cache.TryGetValue(baseType, out Type[] types))
				types = GenTypeCache(baseType);

			return (ushort)types.FindIndex(type);
		}
	}

	internal static class RWUtil
	{
		internal static Harmony.SetterHandler ReaderSetIndex = Harmony.FastAccess.CreateSetterHandler(typeof(ByteReader)
			.GetField("index", BindingFlags.NonPublic | BindingFlags.Instance));
		internal static Harmony.GetterHandler WriterGetStream = Harmony.FastAccess.CreateGetterHandler(typeof(ByteWriter)
			.GetField("stream", BindingFlags.NonPublic | BindingFlags.Instance));

		internal static void Reset(this SyncWorker sync)
		{
			if(sync is ReadingSyncWorker reader)
			{
				ReaderSetIndex(reader.reader, reader.initialPos);
			}
			else if(sync is WritingSyncWorker writer)
			{
				MemoryStream ms = (MemoryStream)WriterGetStream(writer.writer);
				ms.SetLength(writer.initialPos);
			}
		}
	}

	public class WritingSyncWorker : SyncWorker
	{
		internal readonly ByteWriter writer;
		internal int initialPos;

		public WritingSyncWorker(ByteWriter writer) : base(true)
		{
			this.writer = writer;
			initialPos = writer.Position;
		}

		public override void Bind<T>(ref T obj)
		{
			//if (Syncer.WriteSyncer(obj, typeof(T), this))
			//	return;
			
			Sync.WriteSyncObject(writer, obj, typeof(T));
		}

		public override void Bind(object obj, string name)
		{
			object value = MpReflection.GetValue(obj, name);
			Type type = value.GetType();

			//if (Syncer.WriteSyncer(value, type, this))
			//	return;

			Sync.WriteSyncObject(writer, value, type);
		}

		public override void Bind(ref byte obj)
		{
			writer.WriteByte(obj);
		}

		public override void Bind(ref sbyte obj)
		{
			writer.WriteSByte(obj);
		}

		public override void Bind(ref short obj)
		{
			writer.WriteShort(obj);
		}

		public override void Bind(ref ushort obj)
		{
			writer.WriteUShort(obj);
		}

		public override void Bind(ref int obj)
		{
			writer.WriteInt32(obj);
		}

		public override void Bind(ref uint obj)
		{
			writer.WriteUInt32(obj);
		}

		public override void Bind(ref long obj)
		{
			writer.WriteLong(obj);
		}

		public override void Bind(ref ulong obj)
		{
			writer.WriteULong(obj);
		}

		public override void Bind(ref float obj)
		{
			writer.WriteFloat(obj);
		}

		public override void Bind(ref double obj)
		{
			writer.WriteDouble(obj);
		}

		public override void Bind(ref bool obj)
		{
			writer.WriteBool(obj);
		}

		public override void Bind(ref string obj)
		{
			writer.WriteString(obj);
		}

		public override void BindType<T>(ref Type type)
		{
			writer.WriteUShort(TypeRWHelper.GetTypeIndex(type, typeof(T)));
		}
	}

	public class ReadingSyncWorker : SyncWorker
	{
		internal readonly ByteReader reader;
		internal int initialPos;

		public ReadingSyncWorker(ByteReader reader) : base(false)
		{
			this.reader = reader;
			initialPos = reader.Position;
		}

		public override void Bind<T>(ref T obj)
		{
			//if (Syncer.ReadSyncer(obj, typeof(T), out object res, this))
			//	obj = (T)res;

			obj = (T)Sync.ReadSyncObject(reader, typeof(T));
		}

		public override void Bind(object obj, string name)
		{
			object value = MpReflection.GetValue(obj, name);

			Type type = value.GetType();

			//if (!Syncer.ReadSyncer(value, type, out object res, this))
			var	res = Sync.ReadSyncObject(reader, type);

			MpReflection.SetValue(obj, name, res);
		}

		public override void Bind(ref byte obj)
		{
			obj = reader.ReadByte();
		}

		public override void Bind(ref sbyte obj)
		{
			obj = reader.ReadSByte();
		}

		public override void Bind(ref short obj)
		{
			obj = reader.ReadShort();
		}

		public override void Bind(ref ushort obj)
		{
			obj = reader.ReadUShort();
		}

		public override void Bind(ref int obj)
		{
			obj = reader.ReadInt32();
		}

		public override void Bind(ref uint obj)
		{
			obj = reader.ReadUInt32();
		}

		public override void Bind(ref long obj)
		{
			obj = reader.ReadLong();
		}

		public override void Bind(ref ulong obj)
		{
			obj = reader.ReadULong();
		}

		public override void Bind(ref float obj)
		{
			obj = reader.ReadFloat();
		}

		public override void Bind(ref double obj)
		{
			obj = reader.ReadDouble();
		}

		public override void Bind(ref bool obj)
		{
			obj = reader.ReadBool();
		}

		public override void Bind(ref string obj)
		{
			obj = reader.ReadString();
		}

		public override void BindType<T>(ref Type type)
		{
			type = TypeRWHelper.GetType(reader.ReadUShort(), typeof(T));
		}
	}

	public class Syncer
	{
		public static bool ReadSyncer(object obj, Type type, out object res, ReadingSyncWorker syncWorker = null, ByteReader reader = null, SyncerEntry syncer = null)
		{
			res = null;

			if (syncer == null)
			{
				if (!Proxies.IFSync.syncers.TryGetValue(type, out syncer))
					return false;
			}

			if (syncWorker == null)
				syncWorker = new ReadingSyncWorker(reader);

			if (obj == null && (syncer.shouldConstruct || type.IsValueType))
				obj = Activator.CreateInstance(type);

			try
			{
				bool r = syncer.Invoke(syncWorker, ref obj);
				res = obj;

				return r;
			}
			catch(Exception exc)
			{
				Log.Error($"An exception has been thrown while executing a reading syncer for type {type}: {exc}");
				throw;
			}
		}
		
		public static bool WriteSyncer(object obj, Type type, WritingSyncWorker syncWorker = null, ByteWriter writer = null, SyncerEntry syncer = null)
		{
			if (syncer == null)
			{
				if (!Proxies.IFSync.syncers.TryGetValue(type, out syncer))
					return false;
			}

			if (syncWorker == null)
				syncWorker = new WritingSyncWorker(writer);


			try
			{
				return syncer.Invoke(syncWorker, ref obj);
			}
			catch(Exception exc)
			{
				Log.Error($"An exception has been thrown while executing a writing syncer for type {type}: {exc}");
				throw;
			}
		}
	}

	
}
