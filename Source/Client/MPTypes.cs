using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace UnofficialMultiplayerAPI
{
	/// <summary>
	/// Context flags which are sent along with a command
	/// </summary>
	[Flags]
	public enum SyncContext
	{
		/// <summary>Default  value. (no context)</summary>
		None = 0,
		/// <summary>Send mouse cell context (emulates mouse position)</summary>
		MapMouseCell = 1,
		/// <summary>Send map selected context (object selected on the map)</summary>
		MapSelected = 2,
		/// <summary>Send world selected context (object selected on the world map)</summary>
		WorldSelected = 4,
		/// <summary>Send order queue context (emulates pressing KeyBindingDefOf.QueueOrder)</summary>
		QueueOrder_Down = 8,
		/// <summary>Send current map context</summary>
		CurrentMap = 16,
	}

	/// <summary>
	/// An attribute that is used to mark methods for syncing.
	/// </summary>
	/// <example>
	/// <para>An example showing how to mark a method for syncing.</para>
	/// <code>
	/// [SyncMethod]
	/// public void MyMethod(...)
	/// {
	///		...
	///	}
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method)]
	public class SyncMethodAttribute : Attribute
	{
		public SyncContext context;

		/// <summary>Instructs SyncMethod to cancel synchronization if any arg is null (see <see cref="ISyncMethod.CancelIfAnyArgNull"/>).</summary>
		public bool cancelIfAnyArgNull = false;

		/// <summary>Instructs SyncMethod to cancel synchronization if no map objects were selected during the call (see <see cref="ISyncMethod.CancelIfNoSelectedMapObjects"/>).</summary>
		public bool cancelIfNoSelectedMapObjects = false;

		/// <summary>Instructs SyncMethod to cancel synchronization if no world objects were selected during call replication(see <see cref="ISyncMethod.CancelIfNoSelectedWorldObjects"/>).</summary>
		public bool cancelIfNoSelectedWorldObjects = false;

		/// <summary>Instructs SyncMethod to synchronize only in debug mode (see <see cref="ISyncMethod.SetDebugOnly"/>).</summary>
		public bool setDebugOnly = false;

		/// <summary>A list of types to expose (see <see cref="ISyncMethod.ExposeParameter"/>)</summary>
		public int[] exposeParameters;

		/// <param name="context">Context</param>
		public SyncMethodAttribute(SyncContext context = SyncContext.None)
		{
			this.context = context;
		}
	}

	public class SyncType
	{
		public readonly Type type;
		public bool expose = false;
		public bool contextMap = false;

		public SyncType(Type type)
		{
			this.type = type;
		}
	}
}
