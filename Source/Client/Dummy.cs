using System;
using System.Reflection;

#pragma warning disable CS1591

namespace UnofficialMultiplayerAPI
{
	/// <summary>
	/// An exception that is thrown if you try to use the API without avaiable host.
	/// </summary>
	public class UninitializedAPI : Exception
	{
	}

	class Dummy : IAPI
	{
		public bool IsHosting => false;

		public bool IsInMultiplayer => false;

		public string PlayerName => null;

		//public SyncerDictionary Syncers => throw new UninitializedAPI();

		public void FieldWatchPostfix()
		{
			throw new UninitializedAPI();
		}

		public void FieldWatchPrefix()
		{
			throw new UninitializedAPI();
		}
		/*
		public ISyncDelegate RegisterSyncDelegate(Type type, string nestedType, string method)
		{
			throw new UninitializedAPI();
		}

		public ISyncDelegate RegisterSyncDelegate(Type inType, string nestedType, string methodName, string[] fields, Type[] args = null)
		{
			throw new UninitializedAPI();
		}*/

		public ISyncMethod RegisterSyncMethod(Type type, string methodOrPropertyName, SyncType[] argTypes = null)
		{
			throw new UninitializedAPI();
		}

		public ISyncMethod RegisterSyncMethod(MethodInfo method, SyncType[] argTypes)
		{
			throw new UninitializedAPI();
		}

		public ISyncField SyncField(Type targetType, string memberPath)
		{
			throw new UninitializedAPI();
		}

		public void Watch(ISyncField field, object target = null, object index = null)
		{
			throw new UninitializedAPI();
		}
	}
}
