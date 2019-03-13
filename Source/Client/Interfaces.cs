using System;
using System.Reflection;

#pragma warning disable CS1591

namespace UnofficialMultiplayerAPI
{
	/// <summary>
	/// Context flags which are sent along with a command
	/// </summary>
	[Flags]
	public enum SyncContext
	{
		/// <summary>Send no context</summary>
		None = 0,
		/// <summary>Send mouse cell context (Emulates mouse position)</summary>
		MapMouseCell = 1,
		/// <summary>Send map selected context (object selected on the map)</summary>
		MapSelected = 2,
		/// <summary>Send world selected context (object selected on the world map)</summary>
		WorldSelected = 4,
		/// <summary>Send order queue context (Emulates pressing KeyBindingDefOf.QueueOrder)</summary>
		QueueOrder_Down = 8,
		/// <summary>Send current map context</summary>
		CurrentMap = 16,
	}

	/// <summary>
	/// An attribute that is used to mark methods for syncing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class SyncMethodAttribute : Attribute
	{
		public SyncContext context;

		/// <param name="context">Context</param>
		public SyncMethodAttribute(SyncContext context = SyncContext.None)
		{
			this.context = context;
		}
	}

	public struct SyncType
	{
		public readonly Type type;
		public bool expose;
		public bool contextMap;

		public SyncType(Type type)
		{
			this.type = type;
			this.expose = false;
			contextMap = false;
		}
	}

	/// <summary>
	/// SyncField interface
	/// </summary>
	public interface ISyncField
	{
		/// <summary>
		/// Instructs SyncField to cancel synchronization if the value of the member it's pointing at is null.
		/// </summary>
		/// <returns><see cref="ISyncField"/> self</returns>
		ISyncField CancelIfValueNull();

		/// <summary>
		/// Instructs SyncField to sync in game loop
		/// </summary>
		/// <returns><see cref="ISyncField"/> self</returns>
		ISyncField InGameLoop();

		/// <summary>
		/// Adds an Action that runs after a field is synchronized.
		/// </summary>
		/// <param name="action">An action ran after a field is synchronized. Called with target and value</param>
		/// <returns><see cref="ISyncField"/> self</returns>
		ISyncField PostApply(Action<object, object> action);

		/// <summary>
		/// Adds an Action that runs before a field is synchronized.
		/// </summary>
		/// <param name="action">An action ran before a field is synchronized. Called with target and value</param>
		/// <returns><see cref="ISyncField"/> self</returns>
		ISyncField PreApply(Action<object, object> action);

		/// <summary>
		/// Instructs SyncField to use a buffer instead of syncing instantly (when <see cref="MPApi.FieldWatchPostfix"/> is called)
		/// </summary>
		/// <returns><see cref="ISyncField"/> self</returns>
		ISyncField SetBufferChanges();

		/// <summary>
		/// Instructs SyncField to synchronize only in debug mode.
		/// </summary>
		/// <returns><see cref="ISyncField"/> self</returns>
		ISyncField SetDebugOnly();

		/// <summary>
		/// Instructs SyncField to synchronize only if it's invoked by the host.
		/// </summary>
		/// <returns><see cref="ISyncField"/> self</returns>
		ISyncField SetHostOnly();

		/// <summary>
		/// 
		/// </summary>
		/// <returns><see cref="ISyncField"/> self</returns>
		ISyncField SetVersion(int version);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target">An object of type set in the <see cref="ISyncField"/>. If null, a static field will be used instead</param>
		/// <param name="index">Index in the field path set in <see cref="ISyncField"/></param>
		/// <returns><see cref="ISyncField"/> self</returns>
		void Watch(object target = null, object index = null);

		string ToString();
	}

	/// <summary>
	/// SyncMethod interface
	/// </summary>
	public interface ISyncMethod
	{
		/// <summary>
		/// Instructs SyncMethod to cancel synchronization if any arg is null
		/// </summary>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod CancelIfAnyArgNull();

		/// <summary>
		/// Instructs SyncMethod to cancel synchronization if no map objects were selected during call replication
		/// </summary>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod CancelIfNoSelectedMapObjects();

		/// <summary>
		/// Instructs SyncMethod to cancel synchronization if no world objects were selected during call replication
		/// </summary>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod CancelIfNoSelectedWorldObjects();

		/// <summary>
		/// Use argument type's IExposable interface to transfer it's data to other clients 
		/// </summary>
		/// <remarks>IExposable is the interface used for saving data to the save which means it utilizes IExposable.ExposeData() method</remarks>
		/// <param name="index">Index at which argument is marked to expose</param>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod ExposeParameter(int index);

		/// <summary>
		/// Currently unused in the Multiplayer mod
		/// </summary>
		/// <param name="time">Milliseconds between resends</param>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod MinTime(int time);

		/// <summary>
		/// Instructs method to send context along with the call
		/// </summary>
		/// <remarks>Context is restored after method is called</remarks>
		/// <param name="context">Context</param>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod SetContext(SyncContext context);

		/// <summary>
		/// Instructs SyncMethod to synchronize only in debug mode.
		/// </summary>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod SetDebugOnly();

		/// <summary>
		/// Adds an Action that runs before a call is replicated on client.
		/// </summary>
		/// <param name="action">An action ran before a call is replicated on client. Called with target and value</param>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod SetPreInvoke(Action<object, object[]> action);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="version">Handler version</param>
		/// <returns><see cref="ISyncMethod"/> self</returns>
		ISyncMethod SetVersion(int version);

		string ToString();
	}

	public interface ISyncDelegate
	{
		ISyncDelegate CancelIfAnyFieldNull(params string[] without);
		ISyncDelegate CancelIfFieldsNull(params string[] whitelist);
		ISyncDelegate CancelIfNoSelectedObjects();
		ISyncDelegate RemoveNullsFromLists(params string[] listFields);
		ISyncDelegate SetContext(SyncContext context);
		ISyncDelegate SetDebugOnly();
		string ToString();
	}

	public interface IAPI
	{
		bool IsHosting { get; }
		bool IsInMultiplayer { get; }
		string PlayerName { get; }

		void FieldWatchPrefix();
		void FieldWatchPostfix();

		//ISyncDelegate RegisterSyncDelegate(Type type, string nestedType, string method);
		//ISyncDelegate RegisterSyncDelegate(Type inType, string nestedType, string methodName, string[] fields, Type[] args = null);

		ISyncMethod RegisterSyncMethod(Type type, string methodOrPropertyName, SyncType[] argTypes = null);
		ISyncMethod RegisterSyncMethod(MethodInfo method, SyncType[] argTypes);

		void Watch(ISyncField field, object target = null, object index = null);
		ISyncField SyncField(Type targetType, string memberPath);
	}

	/// <summary>
	/// An interface that is used as an entry point for multiplayer initialization
	/// </summary>
	public interface IMultiplayerInit
	{
		/// <summary>
		/// Entry point for initialization
		/// </summary>
		/// <returns><see langword="void"/></returns>
		void Init();
	}
}
