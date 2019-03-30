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


				if(Proxies.IFSync.syncers.TryGetValue(type, out var syncer))
				{
					return !Syncer.ReadSyncer(null, type, out __result, reader: data, syncer: syncer); ;
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

				if (Proxies.IFSync.syncers.TryGetValue(type, out var syncer))
				{
					return !Syncer.WriteSyncer(obj, type, writer: data, syncer: syncer);
				}

				return true;
			}

		}

	}
}
