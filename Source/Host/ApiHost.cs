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

namespace UnofficialMultiplayerAPIHost
{
	[StaticConstructorOnStartup]
	internal static class LateInit
	{
		static LateInit()
		{
			CheckInterfaceVersions();
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
				.SelectMany(t => t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
				.Where(m => m.HasAttribute<UnofficialMultiplayerAPI.SyncMethodAttribute>());

			foreach(MethodInfo method in allMethods)
			{
				Log.Debug($"Installing {method.Name}");
				Sync.RegisterSyncMethod(method, null);
			}

			var initializers = allTypes.Where(t => !t.IsInterface && typeof(UnofficialMultiplayerAPI.IMultiplayerInit).IsAssignableFrom(t));

			foreach(Type t in initializers)
			{
				try
				{
					Log.Debug($"Calling Init for {t}");
					var @if = (UnofficialMultiplayerAPI.IMultiplayerInit)Activator.CreateInstance(t);

					@if.Init();
				}
				catch(Exception exc)
				{
					Log.Error($"An exception occurred while calling IMultiplayerInit.Init in {t.Assembly.GetName().Name}.dll\n{exc}", true);
				}
			}
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