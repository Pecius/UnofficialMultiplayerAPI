using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using Log = UnofficialMultiplayerAPI_UTILS.Log;

#pragma warning disable CS1591

namespace UnofficialMultiplayerAPI
{
	[StaticConstructorOnStartup]
	public class MPApi
    {
		private static Assembly m_hostAssembly;

		/// <summary>
		/// Returns <see langword="true"/> if API is initialized.
		/// </summary>
		/// <returns><see langword="bool"/></returns>
		public static readonly bool enabled = false;

		private static readonly IAPI Sync;

		static MPApi()
		{
			IEnumerable<Assembly> assemblies = LoadedModManager.RunningMods.SelectMany(m => m.assemblies.loadedAssemblies);

			m_hostAssembly = assemblies.FirstOrDefault(a => a.GetName().Name == "UnofficialMultiplayerAPIHost");

			if (m_hostAssembly == null)
			{
				var mpAssembly = assemblies.FirstOrDefault(a => a.GetName().Name == "Multiplayer");

				if (mpAssembly != null)
					LongEventHandler.QueueLongEvent(() => Find.WindowStack.Add(new InfoWindow()), null, false, null);

				Log.Debug("m_hostAssembly == null");

				Sync = new Dummy();

				return;
			}

			Sync = (IAPI)m_hostAssembly.GetType("UnofficialMultiplayerAPIHost.APIHost").GetField("Sync").GetValue(null);
			Log.Debug($"Sync = {Sync}");

			enabled = true;
		}

		// Some nice shortcuts ready to be inlined by the JIT compiler :)

		/// <summary>
		/// Returns <see langword="true"/> if currently running on a host.
		/// </summary>
		/// <returns><see langword="bool"/></returns>
		public static bool IsHosting => Sync.IsHosting;

		/// <summary>
		/// Returns <see langword="true"/> if currently running in a multiplayer session (both on client and host).
		/// </summary>
		/// <returns><see langword="bool"/></returns>
		public static bool IsInMultiplayer => Sync.IsInMultiplayer;

		/// <summary>
		/// Returns local player's name.
		/// </summary>
		/// <returns><see cref="string"/></returns>
		public static string PlayerName => Sync.PlayerName;

		/// <summary>
		/// Starts a new synchronization stack.
		/// </summary>
		/// <remarks>
		/// Required to be called before invoking Watch methods.
		/// <seealso cref="ISyncField.Watch(object, object)"/>
		/// <seealso cref="IAPI.FieldWatchPrefix()"/>
		/// </remarks>
		/// <returns><see langword="void"/></returns>
		public static void FieldWatchPrefix() => Sync.FieldWatchPrefix();

		/// <summary>
		/// Ends the current synchronization stack and executes it.
		/// </summary>
		/// <remarks>
		/// Required to be called after invoking Watch methods.
		/// <seealso cref="ISyncField.Watch(object, object)"/>
		/// <seealso cref="IAPI.FieldWatchPrefix()"/>
		/// </remarks>
		/// <returns><see langword="void"/></returns>
		public static void FieldWatchPostfix() => Sync.FieldWatchPostfix();

		/// <summary>
		/// Registers a method for syncing and returns its <see cref="ISyncMethod"/>.
		/// </summary>
		/// <remarks>Has to be called inside of <see cref="IMultiplayerInit"/>.<see cref="IMultiplayerInit.Init"/></remarks>
		/// <returns>A new registered <see cref="ISyncMethod"/></returns>
		public static ISyncMethod RegisterSyncMethod(Type type, string methodOrPropertyName, SyncType[] argTypes = null) => Sync.RegisterSyncMethod(type, methodOrPropertyName, argTypes);

		/// <summary>
		/// Registers a method for syncing and returns its <see cref="ISyncMethod"/>.
		/// </summary>
		/// <param name="method">MethodInfo of a method to register</param>
		/// <param name="argTypes">Method's argument types</param>
		/// <remarks>Has to be called inside of <see cref="IMultiplayerInit"/>.<see cref="IMultiplayerInit.Init"/></remarks>
		/// <returns>A new registered <see cref="ISyncMethod"/></returns>
		public static ISyncMethod RegisterSyncMethod(MethodInfo method, SyncType[] argTypes = null) => Sync.RegisterSyncMethod(method, argTypes);

		/// <summary>
		/// An alias for <see cref="ISyncField.Watch"/>
		/// <seealso cref="ISyncField.Watch(object, object)"/>
		/// </summary>
		/// <param name="field"><see cref="ISyncField"/> object to watch</param>
		/// <param name="target">An object of type set in the <see cref="ISyncField"/>. If null, a static field will be used instead</param>
		/// <param name="index">Index in the field path set in <see cref="ISyncField"/></param>
		/// <returns><see langword="void"/></returns>
		public static void Watch(ISyncField field, object target = null, object index = null) => Sync.Watch(field, target, index);

		/// <summary>
		/// Registers a field for syncing and returns it's <see cref="ISyncField"/>
		/// </summary>
		/// <remarks>Has to be called inside of <see cref="IMultiplayerInit"/>.<see cref="IMultiplayerInit.Init"/></remarks>
		/// <param name="targetType">Type of the target class that contains a specified member</param>
		/// <param name="memberPath">Path to a member. If the member is to be indexed, it has to end with /[] eg. <c>"myArray/[]"</c></param>
		/// <returns>A new registered <see cref="ISyncField"/></returns>
		public static ISyncField SyncField(Type targetType, string memberPath) => Sync.SyncField(targetType, memberPath);
	}
}
