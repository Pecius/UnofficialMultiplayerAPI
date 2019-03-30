using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnofficialMultiplayerAPI;
using Harmony;

using MPC = Multiplayer.Client;

namespace UnofficialMultiplayerAPIHost
{
	namespace Proxies
	{
		internal static class Convert
		{
			public static MPC.SyncType SyncType(SyncType st)
			{
				var nt = new MPC.SyncType(st.type)
				{
					expose = st.expose,
					contextMap = st.contextMap
				};

				return nt;
			}

			public static MPC.SyncType[] SyncTypeA(SyncType[] st)
			{
				return (MPC.SyncType[])st.Select(s => SyncType(s));
			}
		}

		public class SyncMethod : MPC.SyncMethod, ISyncMethod
		{
			public SyncMethod(Type targetType, MethodInfo method, SyncType[] argTypes)
				: base(targetType, method, Convert.SyncTypeA(argTypes))
			{
			}

			public SyncMethod(Type targetType, string instancePath, string methodName, SyncType[] argTypes)
				: base(targetType, instancePath, methodName, Convert.SyncTypeA(argTypes))
			{
			}

			public ISyncMethod SetContext(SyncContext context)
			{
				SetContext((MPC.SyncContext)(int)context);
				return this;
			}

			public new ISyncMethod CancelIfAnyArgNull()
			{
				base.CancelIfAnyArgNull();
				return this;
			}

			public new ISyncMethod CancelIfNoSelectedMapObjects()
			{
				base.CancelIfNoSelectedMapObjects();
				return this;
			}

			public new ISyncMethod CancelIfNoSelectedWorldObjects()
			{
				base.CancelIfNoSelectedWorldObjects();
				return this;
			}

			public new ISyncMethod ExposeParameter(int index)
			{
				base.ExposeParameter(index);
				return this;
			}

			public new ISyncMethod MinTime(int time)
			{
				base.MinTime(time);
				return this;
			}

			public new ISyncMethod SetDebugOnly()
			{
				base.SetDebugOnly();
				return this;
			}

			public new ISyncMethod SetPreInvoke(Action<object, object[]> action)
			{
				base.SetPreInvoke(action);
				return this;
			}

			public new ISyncMethod SetVersion(int version)
			{
				base.SetVersion(version);
				return this;
			}
		}


		public class SyncField : MPC.SyncField, ISyncField
		{
			public SyncField(Type targetType, string memberPath) : base(targetType, memberPath)
			{
			}

			public new ISyncField CancelIfValueNull()
			{
				base.CancelIfValueNull();
				return this;
			}

			public new ISyncField InGameLoop()
			{
				base.InGameLoop();
				return this;
			}

			public new ISyncField PostApply(Action<object, object> action)
			{
				base.PostApply(action);
				return this;
			}

			public new ISyncField PreApply(Action<object, object> action)
			{
				base.PreApply(action);
				return this;
			}
			public new ISyncField SetBufferChanges()
			{
				base.SetBufferChanges();
				return this;
			}

			public new ISyncField SetDebugOnly()
			{
				base.SetDebugOnly();
				return this;
			}

			public new ISyncField SetHostOnly()
			{
				base.SetHostOnly();
				return this;
			}

			public new ISyncField SetVersion(int version)
			{
				base.SetVersion(version);
				return this;
			}

			public void Watch(object target = null, object index = null)
			{
				MPC.Sync.Watch(this, target, index);
			}
		}

		class SyncDelegate : MPC.SyncDelegate, ISyncDelegate
		{
			public SyncDelegate(Type delegateType, MethodInfo method, string[] fieldPaths)
				: base(delegateType, method, fieldPaths)
			{
			}

			public ISyncDelegate SetContext(SyncContext context)
			{
				SetContext((MPC.SyncContext)(int)context);
				return this;
			}

			public new ISyncDelegate CancelIfAnyFieldNull(params string[] without)
			{
				base.CancelIfAnyFieldNull(without);
				return this;
			}

			public new ISyncDelegate CancelIfFieldsNull(params string[] whitelist)
			{
				base.CancelIfFieldsNull(whitelist);
				return this;
			}

			public new ISyncDelegate CancelIfNoSelectedObjects()
			{
				base.CancelIfNoSelectedObjects();
				return this;
			}

			public new ISyncDelegate RemoveNullsFromLists(params string[] listFields)
			{
				base.RemoveNullsFromLists(listFields);
				return this;
			}

			public new ISyncDelegate SetDebugOnly()
			{
				base.SetDebugOnly();
				return this;
			}
		}

		public class IFSync : IAPI
		{
			internal static SyncerDictionary syncers = new SyncerDictionary();

			public bool IsHosting => Multiplayer.Common.MultiplayerServer.instance != null;//MPC.Multiplayer.LocalServer != null;
			public bool IsInMultiplayer => MPC.Multiplayer.session != null;

			public string PlayerName => MPC.Multiplayer.username;

			public void FieldWatchPostfix()
			{
				MPC.Sync.FieldWatchPostfix();
			}

			public void FieldWatchPrefix()
			{
				MPC.Sync.FieldWatchPrefix();
			}

			/*public ISyncDelegate RegisterSyncDelegate(Type type, string nestedType, string method)
			{
				throw new NotImplementedException();
			}

			public ISyncDelegate RegisterSyncDelegate(Type inType, string nestedType, string methodName, string[] fields, Type[] args = null)
			{
				throw new NotImplementedException();
			}*/

			public ISyncMethod RegisterSyncMethod(Type type, string methodOrPropertyName, SyncType[] argTypes = null)
			{
				MethodInfo method = AccessTools.Method(type, methodOrPropertyName, argTypes != null ? argTypes.Select(t => t.type).ToArray() : null);

				if (method == null)
				{
					PropertyInfo property = AccessTools.Property(type, methodOrPropertyName);
					method = property.GetSetMethod();
				}

				if (method == null)
					throw new Exception($"Couldn't find method or property {methodOrPropertyName} in type {type}");

				return RegisterSyncMethod(method, argTypes);
			}

			public ISyncMethod RegisterSyncMethod(MethodInfo method, SyncType[] argTypes)
			{
				MPC.MpUtil.MarkNoInlining(method);

				SyncMethod handler = new SyncMethod((method.IsStatic ? null : method.DeclaringType), method, argTypes);
				MPC.Sync.syncMethods[method] = handler;
				MPC.Sync.handlers.Add(handler);

				Traverse.Create(typeof(MPC.Sync)).Method("PatchMethodForSync", method);

				return handler;
			}

			public ISyncField SyncField(Type targetType, string memberPath)
			{
				SyncField handler = new SyncField(targetType, memberPath);
				MPC.Sync.handlers.Add(handler);
				return handler;
			}

			public void Watch(ISyncField field, object target = null, object index = null)
			{
				MPC.Sync.Watch((SyncField)field, target, index);
			}
		}
	}
}
