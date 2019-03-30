using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Verse;
using Multiplayer.Client;
using Log = UnofficialMultiplayerAPI_UTILS.Log;
using System.Diagnostics;
using System.IO;
using ClientAPI = UnofficialMultiplayerAPI;
using DynDelegate;

namespace UnofficialMultiplayerAPIHost
{
	[StaticConstructorOnStartup]
	internal static class LateInit
	{
		static LateInit()
		{
			CheckInterfaceVersions();
			Patches.Harmony.DoPatches();
			RegisterAllMethods();
			Sync.InitHandlers();
		}

		private static void RegisterAllMethods()
		{
			Log.Debug("Registering all methods with SyncMethod attribute.");

			IEnumerable<Assembly> allActive = (IEnumerable<Assembly>)typeof(GenTypes)
				.GetProperty("AllActiveAssemblies", BindingFlags.NonPublic | BindingFlags.Static).GetGetMethod(true).Invoke(null, null);

			allActive = allActive.Where(a => a.GetReferencedAssemblies().Any(n => n.Name == "0UnofficialMultiplayerAPI"));

			IEnumerable<Type> allTypes = allActive.SelectMany(a => a.GetTypes());

			IEnumerable<MethodInfo> allMethods = allTypes
				.SelectMany(t => t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));

			foreach(MethodInfo method in allMethods.Where(m => m.HasAttribute<ClientAPI.SyncMethodAttribute>()))
			{
				RegisterMethod(method);
			}

			foreach(MethodInfo method in allMethods.Where(m => m.HasAttribute<ClientAPI.SyncerAttribute>()))
			{
				RegisterSyncer(method);
			}

			var initializers = allTypes.Where(t => !t.IsInterface && typeof(ClientAPI.IMultiplayerInit).IsAssignableFrom(t));

			foreach (Type t in initializers)
			{
				try
				{
					Log.Debug($"Calling Init for {t}");
					var @if = (ClientAPI.IMultiplayerInit)Activator.CreateInstance(t);

					@if.Init();
				}
				catch (Exception exc)
				{
					Log.Error($"An exception occurred while calling IMultiplayerInit.Init in {t.Assembly.GetName().Name}.dll\n{exc}", true);
				}
			}
		}

		private static void RegisterMethod(MethodInfo method)
		{
			var attribute = method.TryGetAttribute<ClientAPI.SyncMethodAttribute>();

			Log.Debug($"Installing {method.DeclaringType.FullName}::{method}");
			int[] exposeParameters = attribute.exposeParameters;
			int paramNum = method.GetParameters().Length;

			if (exposeParameters != null)
			{
				if (exposeParameters.Length != paramNum)
				{
					Log.Error($"Failed to register a method: Invalid number of parameters to expose in SyncMethod attribute applied to {method.DeclaringType.FullName}::{method}. Expected {paramNum}, got {exposeParameters.Length}");
					return;
				}
				else if (exposeParameters.Any(p => p < 0 || p >= paramNum))
				{
					Log.Error($"Failed to register a method: One or more indexes of parameters to expose in SyncMethod attribute applied to {method.DeclaringType.FullName}::{method} is invalid.");
					return;
				}
			}

			SyncMethod sm = Sync.RegisterSyncMethod(method, null);

			sm.SetContext((SyncContext)(int)attribute.context);

			if (attribute.cancelIfAnyArgNull)
				sm.CancelIfAnyArgNull();

			if (attribute.cancelIfNoSelectedMapObjects)
				sm.CancelIfNoSelectedMapObjects();

			if (attribute.cancelIfNoSelectedWorldObjects)
				sm.CancelIfNoSelectedWorldObjects();

			if (attribute.setDebugOnly)
				sm.SetDebugOnly();

			if (exposeParameters != null)
			{
				int i = 0;

				try
				{
					for (; i < exposeParameters.Length; i++)
					{
						Log.Debug($"Exposing parameter {exposeParameters[i]}");
						sm.ExposeParameter(exposeParameters[i]);
					}
				}
				catch (Exception exc)
				{
					Log.Error($"An exception occurred while exposing parameter {i} ({method.GetParameters()[i]}) for method {method.DeclaringType.FullName}::{method}: {exc}");
				}
			}
		}

		private static void RegisterSyncer(MethodInfo method)
		{
			Type[] parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();

			if (!method.IsStatic)
			{
				Log.Error($"Error in {method.DeclaringType.FullName}::{method}: Syncer method has to be static.");
				return;
			}

			if (parameters.Length != 2)
			{
				Log.Error($"Error in {method.DeclaringType.FullName}::{method}: Syncer method has an invalid number of parameters.");
				return;
			}

			if (parameters[0] != typeof(ClientAPI.SyncWorker))
			{
				Log.Error($"Error in {method.DeclaringType.FullName}::{method}: Syncer method has an invalid first parameter (got {parameters[0]}, expected ISyncWorker).");
				return;
			}

			if (!parameters[1].IsByRef)
			{
				Log.Error($"Error in {method.DeclaringType.FullName}::{method}: Syncer method has an invalid second parameter, should be a ref.");
				return;
			}

			var type = parameters[1].GetElementType();
			var attribute = method.TryGetAttribute<ClientAPI.SyncerAttribute>();

			if (attribute.isImplicit)
			{
				if (method.ReturnType != typeof(bool))
				{
					Log.Error($"Error in {method.DeclaringType.FullName}::{method}: Syncer set as implicit (or the argument type is an interface) requires bool type as a return value.");
					return;
				}
			}
			else if (method.ReturnType != typeof(void))
			{
				Log.Error($"Error in {method.DeclaringType.FullName}::{method}: Syncer set as explicit should have void as a return value.");
				return;
			}

			SyncerEntry entry = Proxies.IFSync.syncers.Add(type, attribute.isImplicit, attribute.shouldConstruct);
			entry.AddSyncer(DynamicDelegate.Create<SyncerEntry.syncerDelegateRef>(method), method.ReturnType == typeof(void));

			if (!(attribute.isImplicit || type.IsInterface) && entry.SyncerCount > 1)
			{
				Log.Warning($"Warning in {method.DeclaringType.FullName}::{method}: type {type} has already registered an explicit syncer, the code in this method may be not used.");
			}

			Log.Debug($"Registered a syncer {method.DeclaringType.FullName}::{method} for type {type} in assembly {method.DeclaringType.Assembly.GetName().Name}");
		}

		private static void CheckInterfaceVersions()
		{
			var curVersion = new Version((AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "0UnofficialMultiplayerAPI")
				.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0] as AssemblyFileVersionAttribute).Version);

			Log.Debug($"Current client API version: {curVersion}");
			
			foreach (var mod in LoadedModManager.RunningMods)
			{
				if (!mod.LoadedAnyAssembly)
					continue;

				Assembly APIClient = mod.assemblies.loadedAssemblies.FirstOrDefault(a => a.GetName().Name == "0UnofficialMultiplayerAPI");

				if(APIClient != null)
				{
					var version = new Version(FileVersionInfo.GetVersionInfo(Path.Combine(mod.AssembliesFolder, $"{APIClient.GetName().Name}.dll")).FileVersion);

					Log.Debug($"Mod {mod.Name} has API client ({version})");

					if (curVersion > version)
						Log.Warning($"Mod {mod.Name} uses an older API version (mod: {version}, current: {curVersion})");
					else if (curVersion < version)
						Log.Error($"Mod {mod.Name} uses a newer API version! (mod: {version}, current: {curVersion})\nMake sure the Unofficial Multiplayer API is up to date and is loaded before mods that use it are (place it just under the Multiplayer mod)");
				}
			}
		}
	}

	public static class APIHost
    {
		public static readonly Proxies.IFSync Sync = new Proxies.IFSync();

		static APIHost()
		{

		}
    }
}