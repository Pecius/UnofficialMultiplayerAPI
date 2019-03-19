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
	/// <summary>
	/// The primary static class that contains methods used to interface with the multiplayer mod.
	/// </summary>
	[StaticConstructorOnStartup]
	public static class MPApi
    {
		private static Assembly m_hostAssembly;

		/// <value>
		/// Returns <see langword="true"/> if API is initialized.
		/// </value>
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

		/// <value>
		/// Returns <see langword="true"/> if currently running on a host.
		/// </value>
		public static bool IsHosting => Sync.IsHosting;

		/// <value>
		/// Returns <see langword="true"/> if currently running in a multiplayer session (both on client and host).
		/// </value>
		public static bool IsInMultiplayer => Sync.IsInMultiplayer;

		/// <value>
		/// Returns local player's name.
		/// </value>
		public static string PlayerName => Sync.PlayerName;

		/// <summary>
		/// Starts a new synchronization stack.
		/// </summary>
		/// <remarks>
		/// <para>Has to be called before invoking Watch methods.</para>
		/// <para>See also <see cref="ISyncField.Watch(object, object)"/>.</para>
		/// </remarks>
		public static void FieldWatchPrefix() => Sync.FieldWatchPrefix();

		/// <summary>
		/// Ends the current synchronization stack and executes it.
		/// </summary>
		/// <remarks>
		/// <para>Has to be called after invoking Watch methods.</para>
		/// <para>See also <see cref="ISyncField.Watch(object, object)"/>.</para>
		/// </remarks>
		public static void FieldWatchPostfix() => Sync.FieldWatchPostfix();

		/// <summary>
		/// Registers a method for syncing and returns its <see cref="ISyncMethod"/>.
		/// </summary>
		/// <remarks>
		/// <para>Has to be called inside of <see cref="IMultiplayerInit"/>.<see cref="IMultiplayerInit.Init"/>.</para>
		/// <para>It's recommended to use <see cref="SyncMethodAttribute"/> instead, unless you have to otherwise.</para>
		/// </remarks>
		/// <param name="type">Type that contains the method</param>
		/// <param name="methodOrPropertyName">Name of the method</param>
		/// <param name="argTypes">Method's parameter types</param>
		/// <returns>A new registered <see cref="ISyncMethod"/></returns>
		public static ISyncMethod RegisterSyncMethod(Type type, string methodOrPropertyName, SyncType[] argTypes = null) => Sync.RegisterSyncMethod(type, methodOrPropertyName, argTypes);

		/// <summary>
		/// Registers a method for syncing and returns its <see cref="ISyncMethod"/>.
		/// </summary>
		/// <param name="method">MethodInfo of a method to register</param>
		/// <param name="argTypes">Method's parameter types</param>
		/// <remarks>
		/// <para>Has to be called inside of <see cref="IMultiplayerInit"/>.<see cref="IMultiplayerInit.Init"/>.</para>
		/// <para>It's recommended to use <see cref="SyncMethodAttribute"/> instead, unless you have to otherwise.</para>
		/// </remarks>
		/// <returns>A new registered <see cref="ISyncMethod"/></returns>
		/// <example>
		/// Register a method for syncing using reflection and set it to debug only.
		/// <code>
		///	RegisterSyncMethod(typeof(MyType).GetMethod(nameof(MyType.MyMethod))).SetDebugOnly();
		/// </code>
		/// </example>
		public static ISyncMethod RegisterSyncMethod(MethodInfo method, SyncType[] argTypes = null) => Sync.RegisterSyncMethod(method, argTypes);

		/// <summary>
		/// An alias for <see cref="ISyncField.Watch"/>.
		/// </summary>
		/// <param name="field"><see cref="ISyncField"/> object to watch</param>
		/// <param name="target">An object of type set in the <see cref="ISyncField"/>. Set to null if you're watching a static field.</param>
		/// <param name="index">Index in the field path set in <see cref="ISyncField"/></param>
		public static void Watch(ISyncField field, object target = null, object index = null) => Sync.Watch(field, target, index);

		/// <summary>
		/// Registers a field for syncing and returns it's <see cref="ISyncField"/>.
		/// </summary>
		/// <remarks>Has to be called inside of <see cref="IMultiplayerInit"/>.<see cref="IMultiplayerInit.Init"/>.</remarks>
		/// <param name="targetType">
		/// <para>Type of the target class that contains the specified member</para>
		/// <para>if null, <paramref name="memberPath"/> will point at field from the global namespace</para>
		/// </param>
		/// <param name="memberPath">Path to a member. If the member is to be indexed, it has to end with /[] eg. <c>"myArray/[]"</c></param>
		/// <returns>A new registered <see cref="ISyncField"/></returns>
		public static ISyncField SyncField(Type targetType, string memberPath) => Sync.SyncField(targetType, memberPath);
	}
}
