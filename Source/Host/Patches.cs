using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using Multiplayer.Client;
using Multiplayer.Common;
using ClientAPI = UnofficialMultiplayerAPI;
using System.Reflection;

namespace UnofficialMultiplayerAPIHost
{
	namespace Patches
	{
		public static class Harmony
		{
			public static void DoPatches()
			{
				var harmony = HarmonyInstance.Create("com.Pecius.UnofficialMultiplayerAPI");
				harmony.PatchAll(Assembly.GetExecutingAssembly());
			}
		}


		// TODO: cram those in with a transpiler
		[HarmonyPatch(typeof(Sync), nameof(Sync.ReadSyncObject))]
		class ReadSyncObject
		{
			public static bool Prefix(ByteReader data, SyncType syncType, ref object __result)
			{
				Type type = syncType.type;

				if (typeof(ClientAPI.ISyncable).IsAssignableFrom(type))
				{
					var obj = Activator.CreateInstance(type);

					((ClientAPI.ISyncable)obj).Sync(new ReadingSyncWorker(data));
					__result = obj;
					return false;
				}

				if(Proxies.IFSync.syncers.TryGetValue(type.Assembly, out SyncerDictionary syncers))
				{
					if(syncers.TryGetValue(type, out var syncer))
					{
						__result = Syncer.ReadSyncer(null, type, reader: data, syncer: syncer);
						return false;
					}
				}

				return true;
			}

		}

		[HarmonyPatch(typeof(Sync), nameof(Sync.WriteSyncObject))]
		class WriteSyncObject
		{
			public static bool Prefix(ByteWriter data, object obj, SyncType syncType)
			{
				Type type = syncType.type;

				if (typeof(ClientAPI.ISyncable).IsAssignableFrom(type))
				{
					((ClientAPI.ISyncable)obj).Sync(new WritingSyncWorker(data));
					return false;
				}

				
				if (Proxies.IFSync.syncers.TryGetValue(type.Assembly, out SyncerDictionary syncers))
				{
					if (syncers.TryGetValue(type, out var syncer))
					{
						Syncer.WriteSyncer(obj, type, writer: data, syncer: syncer);
						return false;
					}
				}

				return true;
			}

		}

	}
}
