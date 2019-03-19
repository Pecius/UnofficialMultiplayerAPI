using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer.Client;
using Multiplayer.Common;
using UnofficialMultiplayerAPI;
using Log = UnofficialMultiplayerAPI_UTILS.Log;

namespace UnofficialMultiplayerAPIHost
{
	public class SyncerEntry
	{
		public delegate void syncerDelegateRef(SyncWorker sync, ref object obj);

		public Type type;
		public syncerDelegateRef syncer;
		public bool shouldConstruct;
		private List<SyncerEntry> subclasses;

		public SyncerEntry(Type type, syncerDelegateRef syncer, bool initSubclasses = true, bool shouldConstruct = true)
		{
			this.type = type;
			this.syncer = syncer;
			this.shouldConstruct = shouldConstruct;

			if (initSubclasses)
				subclasses = new List<SyncerEntry>();
		}

		public SyncerEntry(SyncerEntry other)
		{
			type = other.type;
			syncer = other.syncer;
			subclasses = other.subclasses;
			shouldConstruct = other.shouldConstruct;
		}

		public SyncerEntry Add(SyncerEntry other)
		{
			SyncerEntry newEntry = InternalAdd(other.type, other.syncer, null, other.shouldConstruct);

			newEntry.subclasses = other.subclasses;

			return newEntry;
		}

		public SyncerEntry Add(Type type, syncerDelegateRef syncer, bool shouldConstruct = true)
		{
			return InternalAdd(type, syncer, null, shouldConstruct);
		}

		private SyncerEntry InternalAdd(Type type, syncerDelegateRef syncer, SyncerEntry parent, bool shouldConstruct)
		{
			if (type.IsAssignableFrom(this.type))   // Is parent
			{
				SyncerEntry newEntry;

				if (parent != null)
				{
					List<SyncerEntry> ps = parent.subclasses;
					newEntry = new SyncerEntry(type, syncer);
					newEntry.subclasses.Add(this);

					ps[ps.IndexOf(this)] = newEntry;
					return newEntry;
				}
				else
				{
					newEntry = new SyncerEntry(this);
					this.type = type;
					this.syncer = syncer;
					this.shouldConstruct = shouldConstruct;

					subclasses = new List<SyncerEntry>() { newEntry };
					return this;
				}
			}
			else if (this.type.IsAssignableFrom(type)) // Is child
			{
				for (int i = 0; i < subclasses.Count; i++)
				{
					SyncerEntry res = subclasses[i].InternalAdd(type, syncer, this, shouldConstruct);
					if (res != null)
						return res;
				}

				var newEntry = new SyncerEntry(type, syncer);
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

		public void Add(Type type, SyncerEntry.syncerDelegateRef syncer, bool isImplicit = false, bool shouldConstruct = true)
		{
			if (!isImplicit)
			{
				explicitEntries.Add(type, new SyncerEntry(type, syncer, false, shouldConstruct));
				return;
			}

			if (type.IsInterface)
			{
				interfaceEntries.Add(new SyncerEntry(type, syncer, false, shouldConstruct));
				return;
			}

			SyncerEntry entry = null;

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
					entry = e.Add(type, syncer);
				}
			}

			if (entry == null)
			{
				//Console.WriteLine($"*{type}");
				implicitEntries.Add(new SyncerEntry(type, syncer, shouldConstruct));
				return;
			}

			foreach (var e in toRemove)
			{
				implicitEntries.Remove(e);
			}
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




	public class WritingSyncWorker : SyncWorker
	{
		private readonly ByteWriter writer;

		public WritingSyncWorker(ByteWriter writer) : base(true)
		{
			this.writer = writer;
		}

		public override void Bind<T>(ref T obj)
		{
			if (Syncer.WriteSyncer(obj, typeof(T), this))
				return;
			
			Sync.WriteSyncObject(writer, obj, typeof(T));
		}

		public override void Bind(object obj, string name)
		{
			object value = MpReflection.GetValue(obj, name);
			Type type = value.GetType();

			if (Syncer.WriteSyncer(value, type, this))
				return;

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
	}

	public class ReadingSyncWorker : SyncWorker
	{
		private readonly ByteReader reader;


		public ReadingSyncWorker(ByteReader reader) : base(false)
		{
			this.reader = reader;
		}

		public override void Bind<T>(ref T obj)
		{
			object res = Syncer.ReadSyncer(obj, typeof(T), this);

			if (res != null)
				obj = (T)res;

			obj = (T)Sync.ReadSyncObject(reader, typeof(T));
		}

		public override void Bind(object obj, string name)
		{
			object value = MpReflection.GetValue(obj, name);

			Type type = value.GetType();

			object res = Syncer.ReadSyncer(value, type, this);

			if (res == null)
				res = Sync.ReadSyncObject(reader, type);

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
	}

	public class Syncer
	{
		public static object ReadSyncer(object obj, Type type, ReadingSyncWorker syncWorker = null, ByteReader reader = null, SyncerEntry syncer = null)
		{
			if (syncer == null)
			{
				if (!Proxies.IFSync.syncers.TryGetValue(type.Assembly, out var syncers) || !syncers.TryGetValue(type, out syncer))
					return null;
			}

			if (syncWorker == null)
				syncWorker = new ReadingSyncWorker(reader);

			if (obj == null && (syncer.shouldConstruct || type.IsValueType))
				obj = Activator.CreateInstance(type);

			try
			{
				syncer.syncer(syncWorker, ref obj);
			}
			catch(Exception exc)
			{
				Log.Error($"An exception has been thrown while executing a reading syncer for type {type}: {exc}");
				throw;
			}

			return obj;
		}
		
		public static bool WriteSyncer(object obj, Type type, WritingSyncWorker syncWorker = null, ByteWriter writer = null, SyncerEntry syncer = null)
		{
			if (syncer == null)
			{
				if (!Proxies.IFSync.syncers.TryGetValue(type.Assembly, out var syncers) || !syncers.TryGetValue(type, out syncer))
					return false;
			}

			if (syncWorker == null)
				syncWorker = new WritingSyncWorker(writer);


			try
			{
				syncer.syncer(syncWorker, ref obj);
			}
			catch(Exception exc)
			{
				Log.Error($"An exception has been thrown while executing a writing syncer for type {type}: {exc}");
				throw;
			}

			return true;
		}
	}

	
}
