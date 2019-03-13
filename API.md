<a name='assembly'></a>
# 0UnofficialMultiplayerAPI

## Contents

- [IMultiplayerInit](#T-UnofficialMultiplayerAPI-IMultiplayerInit 'UnofficialMultiplayerAPI.IMultiplayerInit')
  - [Init()](#M-UnofficialMultiplayerAPI-IMultiplayerInit-Init 'UnofficialMultiplayerAPI.IMultiplayerInit.Init')
- [ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField')
  - [CancelIfValueNull()](#M-UnofficialMultiplayerAPI-ISyncField-CancelIfValueNull 'UnofficialMultiplayerAPI.ISyncField.CancelIfValueNull')
  - [InGameLoop()](#M-UnofficialMultiplayerAPI-ISyncField-InGameLoop 'UnofficialMultiplayerAPI.ISyncField.InGameLoop')
  - [PostApply(action)](#M-UnofficialMultiplayerAPI-ISyncField-PostApply-System-Action{System-Object,System-Object}- 'UnofficialMultiplayerAPI.ISyncField.PostApply(System.Action{System.Object,System.Object})')
  - [PreApply(action)](#M-UnofficialMultiplayerAPI-ISyncField-PreApply-System-Action{System-Object,System-Object}- 'UnofficialMultiplayerAPI.ISyncField.PreApply(System.Action{System.Object,System.Object})')
  - [SetBufferChanges()](#M-UnofficialMultiplayerAPI-ISyncField-SetBufferChanges 'UnofficialMultiplayerAPI.ISyncField.SetBufferChanges')
  - [SetDebugOnly()](#M-UnofficialMultiplayerAPI-ISyncField-SetDebugOnly 'UnofficialMultiplayerAPI.ISyncField.SetDebugOnly')
  - [SetHostOnly()](#M-UnofficialMultiplayerAPI-ISyncField-SetHostOnly 'UnofficialMultiplayerAPI.ISyncField.SetHostOnly')
  - [SetVersion()](#M-UnofficialMultiplayerAPI-ISyncField-SetVersion-System-Int32- 'UnofficialMultiplayerAPI.ISyncField.SetVersion(System.Int32)')
  - [Watch(target,index)](#M-UnofficialMultiplayerAPI-ISyncField-Watch-System-Object,System-Object- 'UnofficialMultiplayerAPI.ISyncField.Watch(System.Object,System.Object)')
- [ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod')
  - [CancelIfAnyArgNull()](#M-UnofficialMultiplayerAPI-ISyncMethod-CancelIfAnyArgNull 'UnofficialMultiplayerAPI.ISyncMethod.CancelIfAnyArgNull')
  - [CancelIfNoSelectedMapObjects()](#M-UnofficialMultiplayerAPI-ISyncMethod-CancelIfNoSelectedMapObjects 'UnofficialMultiplayerAPI.ISyncMethod.CancelIfNoSelectedMapObjects')
  - [CancelIfNoSelectedWorldObjects()](#M-UnofficialMultiplayerAPI-ISyncMethod-CancelIfNoSelectedWorldObjects 'UnofficialMultiplayerAPI.ISyncMethod.CancelIfNoSelectedWorldObjects')
  - [ExposeParameter(index)](#M-UnofficialMultiplayerAPI-ISyncMethod-ExposeParameter-System-Int32- 'UnofficialMultiplayerAPI.ISyncMethod.ExposeParameter(System.Int32)')
  - [MinTime(time)](#M-UnofficialMultiplayerAPI-ISyncMethod-MinTime-System-Int32- 'UnofficialMultiplayerAPI.ISyncMethod.MinTime(System.Int32)')
  - [SetContext(context)](#M-UnofficialMultiplayerAPI-ISyncMethod-SetContext-UnofficialMultiplayerAPI-SyncContext- 'UnofficialMultiplayerAPI.ISyncMethod.SetContext(UnofficialMultiplayerAPI.SyncContext)')
  - [SetDebugOnly()](#M-UnofficialMultiplayerAPI-ISyncMethod-SetDebugOnly 'UnofficialMultiplayerAPI.ISyncMethod.SetDebugOnly')
  - [SetPreInvoke(action)](#M-UnofficialMultiplayerAPI-ISyncMethod-SetPreInvoke-System-Action{System-Object,System-Object[]}- 'UnofficialMultiplayerAPI.ISyncMethod.SetPreInvoke(System.Action{System.Object,System.Object[]})')
  - [SetVersion(version)](#M-UnofficialMultiplayerAPI-ISyncMethod-SetVersion-System-Int32- 'UnofficialMultiplayerAPI.ISyncMethod.SetVersion(System.Int32)')
- [MPApi](#T-UnofficialMultiplayerAPI-MPApi 'UnofficialMultiplayerAPI.MPApi')
  - [enabled](#F-UnofficialMultiplayerAPI-MPApi-enabled 'UnofficialMultiplayerAPI.MPApi.enabled')
  - [IsHosting](#P-UnofficialMultiplayerAPI-MPApi-IsHosting 'UnofficialMultiplayerAPI.MPApi.IsHosting')
  - [IsInMultiplayer](#P-UnofficialMultiplayerAPI-MPApi-IsInMultiplayer 'UnofficialMultiplayerAPI.MPApi.IsInMultiplayer')
  - [PlayerName](#P-UnofficialMultiplayerAPI-MPApi-PlayerName 'UnofficialMultiplayerAPI.MPApi.PlayerName')
  - [FieldWatchPostfix()](#M-UnofficialMultiplayerAPI-MPApi-FieldWatchPostfix 'UnofficialMultiplayerAPI.MPApi.FieldWatchPostfix')
  - [FieldWatchPrefix()](#M-UnofficialMultiplayerAPI-MPApi-FieldWatchPrefix 'UnofficialMultiplayerAPI.MPApi.FieldWatchPrefix')
  - [RegisterSyncMethod()](#M-UnofficialMultiplayerAPI-MPApi-RegisterSyncMethod-System-Type,System-String,UnofficialMultiplayerAPI-SyncType[]- 'UnofficialMultiplayerAPI.MPApi.RegisterSyncMethod(System.Type,System.String,UnofficialMultiplayerAPI.SyncType[])')
  - [RegisterSyncMethod(method,argTypes)](#M-UnofficialMultiplayerAPI-MPApi-RegisterSyncMethod-System-Reflection-MethodInfo,UnofficialMultiplayerAPI-SyncType[]- 'UnofficialMultiplayerAPI.MPApi.RegisterSyncMethod(System.Reflection.MethodInfo,UnofficialMultiplayerAPI.SyncType[])')
  - [SyncField(targetType,memberPath)](#M-UnofficialMultiplayerAPI-MPApi-SyncField-System-Type,System-String- 'UnofficialMultiplayerAPI.MPApi.SyncField(System.Type,System.String)')
  - [Watch(field,target,index)](#M-UnofficialMultiplayerAPI-MPApi-Watch-UnofficialMultiplayerAPI-ISyncField,System-Object,System-Object- 'UnofficialMultiplayerAPI.MPApi.Watch(UnofficialMultiplayerAPI.ISyncField,System.Object,System.Object)')
- [SyncContext](#T-UnofficialMultiplayerAPI-SyncContext 'UnofficialMultiplayerAPI.SyncContext')
  - [CurrentMap](#F-UnofficialMultiplayerAPI-SyncContext-CurrentMap 'UnofficialMultiplayerAPI.SyncContext.CurrentMap')
  - [MapMouseCell](#F-UnofficialMultiplayerAPI-SyncContext-MapMouseCell 'UnofficialMultiplayerAPI.SyncContext.MapMouseCell')
  - [MapSelected](#F-UnofficialMultiplayerAPI-SyncContext-MapSelected 'UnofficialMultiplayerAPI.SyncContext.MapSelected')
  - [None](#F-UnofficialMultiplayerAPI-SyncContext-None 'UnofficialMultiplayerAPI.SyncContext.None')
  - [QueueOrder_Down](#F-UnofficialMultiplayerAPI-SyncContext-QueueOrder_Down 'UnofficialMultiplayerAPI.SyncContext.QueueOrder_Down')
  - [WorldSelected](#F-UnofficialMultiplayerAPI-SyncContext-WorldSelected 'UnofficialMultiplayerAPI.SyncContext.WorldSelected')
- [SyncMethodAttribute](#T-UnofficialMultiplayerAPI-SyncMethodAttribute 'UnofficialMultiplayerAPI.SyncMethodAttribute')
  - [#ctor(context)](#M-UnofficialMultiplayerAPI-SyncMethodAttribute-#ctor-UnofficialMultiplayerAPI-SyncContext- 'UnofficialMultiplayerAPI.SyncMethodAttribute.#ctor(UnofficialMultiplayerAPI.SyncContext)')

<a name='T-UnofficialMultiplayerAPI-IMultiplayerInit'></a>
## IMultiplayerInit `type`

##### Namespace

UnofficialMultiplayerAPI

##### Summary

An interface that is used as an entry point for multiplayer initialization

<a name='M-UnofficialMultiplayerAPI-IMultiplayerInit-Init'></a>
### Init() `method`

##### Summary

Entry point for initialization

##### Returns

`void`

##### Parameters

This method has no parameters.

<a name='T-UnofficialMultiplayerAPI-ISyncField'></a>
## ISyncField `type`

##### Namespace

UnofficialMultiplayerAPI

##### Summary

SyncField interface

<a name='M-UnofficialMultiplayerAPI-ISyncField-CancelIfValueNull'></a>
### CancelIfValueNull() `method`

##### Summary

Instructs SyncField to cancel synchronization if the value of the member it's pointing at is null.

##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncField-InGameLoop'></a>
### InGameLoop() `method`

##### Summary

Instructs SyncField to sync in game loop

##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncField-PostApply-System-Action{System-Object,System-Object}-'></a>
### PostApply(action) `method`

##### Summary

Adds an Action that runs after a field is synchronized.

##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| action | [System.Action{System.Object,System.Object}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{System.Object,System.Object}') | An action ran after a field is synchronized. Called with target and value |

<a name='M-UnofficialMultiplayerAPI-ISyncField-PreApply-System-Action{System-Object,System-Object}-'></a>
### PreApply(action) `method`

##### Summary

Adds an Action that runs before a field is synchronized.

##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| action | [System.Action{System.Object,System.Object}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{System.Object,System.Object}') | An action ran before a field is synchronized. Called with target and value |

<a name='M-UnofficialMultiplayerAPI-ISyncField-SetBufferChanges'></a>
### SetBufferChanges() `method`

##### Summary

Instructs SyncField to use a buffer instead of syncing instantly (when [FieldWatchPostfix](#M-UnofficialMultiplayerAPI-MPApi-FieldWatchPostfix 'UnofficialMultiplayerAPI.MPApi.FieldWatchPostfix') is called)

##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncField-SetDebugOnly'></a>
### SetDebugOnly() `method`

##### Summary

Instructs SyncField to synchronize only in debug mode.

##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncField-SetHostOnly'></a>
### SetHostOnly() `method`

##### Summary

Instructs SyncField to synchronize only if it's invoked by the host.

##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncField-SetVersion-System-Int32-'></a>
### SetVersion() `method`

##### Summary



##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncField-Watch-System-Object,System-Object-'></a>
### Watch(target,index) `method`

##### Summary



##### Returns

[ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') self

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| target | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | An object of type set in the [ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField'). If null, a static field will be used instead |
| index | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | Index in the field path set in [ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') |

<a name='T-UnofficialMultiplayerAPI-ISyncMethod'></a>
## ISyncMethod `type`

##### Namespace

UnofficialMultiplayerAPI

##### Summary

SyncMethod interface

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-CancelIfAnyArgNull'></a>
### CancelIfAnyArgNull() `method`

##### Summary

Instructs SyncMethod to cancel synchronization if any arg is null

##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-CancelIfNoSelectedMapObjects'></a>
### CancelIfNoSelectedMapObjects() `method`

##### Summary

Instructs SyncMethod to cancel synchronization if no map objects were selected during call replication

##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-CancelIfNoSelectedWorldObjects'></a>
### CancelIfNoSelectedWorldObjects() `method`

##### Summary

Instructs SyncMethod to cancel synchronization if no world objects were selected during call replication

##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-ExposeParameter-System-Int32-'></a>
### ExposeParameter(index) `method`

##### Summary

Use argument type's IExposable interface to transfer it's data to other clients

##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| index | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Index at which argument is marked to expose |

##### Remarks

IExposable is the interface used for saving data to the save which means it utilizes IExposable.ExposeData() method

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-MinTime-System-Int32-'></a>
### MinTime(time) `method`

##### Summary

Currently unused in the Multiplayer mod

##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| time | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Milliseconds between resends |

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-SetContext-UnofficialMultiplayerAPI-SyncContext-'></a>
### SetContext(context) `method`

##### Summary

Instructs method to send context along with the call

##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [UnofficialMultiplayerAPI.SyncContext](#T-UnofficialMultiplayerAPI-SyncContext 'UnofficialMultiplayerAPI.SyncContext') | Context |

##### Remarks

Context is restored after method is called

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-SetDebugOnly'></a>
### SetDebugOnly() `method`

##### Summary

Instructs SyncMethod to synchronize only in debug mode.

##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

This method has no parameters.

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-SetPreInvoke-System-Action{System-Object,System-Object[]}-'></a>
### SetPreInvoke(action) `method`

##### Summary

Adds an Action that runs before a call is replicated on client.

##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| action | [System.Action{System.Object,System.Object[]}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{System.Object,System.Object[]}') | An action ran before a call is replicated on client. Called with target and value |

<a name='M-UnofficialMultiplayerAPI-ISyncMethod-SetVersion-System-Int32-'></a>
### SetVersion(version) `method`

##### Summary



##### Returns

[ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod') self

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| version | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Handler version |

<a name='T-UnofficialMultiplayerAPI-MPApi'></a>
## MPApi `type`

##### Namespace

UnofficialMultiplayerAPI

<a name='F-UnofficialMultiplayerAPI-MPApi-enabled'></a>
### enabled `constants`

##### Summary

Returns `true` if API is initialized.

##### Returns

`bool`

<a name='P-UnofficialMultiplayerAPI-MPApi-IsHosting'></a>
### IsHosting `property`

##### Summary

Returns `true` if currently running on a host.

##### Returns

`bool`

<a name='P-UnofficialMultiplayerAPI-MPApi-IsInMultiplayer'></a>
### IsInMultiplayer `property`

##### Summary

Returns `true` if currently running in a multiplayer session (both on client and host).

##### Returns

`bool`

<a name='P-UnofficialMultiplayerAPI-MPApi-PlayerName'></a>
### PlayerName `property`

##### Summary

Returns local player's name.

##### Returns

[String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String')

<a name='M-UnofficialMultiplayerAPI-MPApi-FieldWatchPostfix'></a>
### FieldWatchPostfix() `method`

##### Summary

Ends the current synchronization stack and executes it.

##### Returns

`void`

##### Parameters

This method has no parameters.

##### Remarks

Required to be called after invoking Watch methods.

<a name='M-UnofficialMultiplayerAPI-MPApi-FieldWatchPrefix'></a>
### FieldWatchPrefix() `method`

##### Summary

Starts a new synchronization stack.

##### Returns

`void`

##### Parameters

This method has no parameters.

##### Remarks

Required to be called before invoking Watch methods.

<a name='M-UnofficialMultiplayerAPI-MPApi-RegisterSyncMethod-System-Type,System-String,UnofficialMultiplayerAPI-SyncType[]-'></a>
### RegisterSyncMethod() `method`

##### Summary

Registers a method for syncing and returns its [ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod').

##### Returns

A new registered [ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod')

##### Parameters

This method has no parameters.

##### Remarks

Has to be called inside of [IMultiplayerInit](#T-UnofficialMultiplayerAPI-IMultiplayerInit 'UnofficialMultiplayerAPI.IMultiplayerInit').[Init](#M-UnofficialMultiplayerAPI-IMultiplayerInit-Init 'UnofficialMultiplayerAPI.IMultiplayerInit.Init')

<a name='M-UnofficialMultiplayerAPI-MPApi-RegisterSyncMethod-System-Reflection-MethodInfo,UnofficialMultiplayerAPI-SyncType[]-'></a>
### RegisterSyncMethod(method,argTypes) `method`

##### Summary

Registers a method for syncing and returns its [ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod').

##### Returns

A new registered [ISyncMethod](#T-UnofficialMultiplayerAPI-ISyncMethod 'UnofficialMultiplayerAPI.ISyncMethod')

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| method | [System.Reflection.MethodInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Reflection.MethodInfo 'System.Reflection.MethodInfo') | MethodInfo of a method to register |
| argTypes | [UnofficialMultiplayerAPI.SyncType[]](#T-UnofficialMultiplayerAPI-SyncType[] 'UnofficialMultiplayerAPI.SyncType[]') | Method's argument types |

##### Remarks

Has to be called inside of [IMultiplayerInit](#T-UnofficialMultiplayerAPI-IMultiplayerInit 'UnofficialMultiplayerAPI.IMultiplayerInit').[Init](#M-UnofficialMultiplayerAPI-IMultiplayerInit-Init 'UnofficialMultiplayerAPI.IMultiplayerInit.Init')

<a name='M-UnofficialMultiplayerAPI-MPApi-SyncField-System-Type,System-String-'></a>
### SyncField(targetType,memberPath) `method`

##### Summary

Registers a field for syncing and returns it's [ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField')

##### Returns

A new registered [ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField')

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| targetType | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | Type of the target class that contains a specified member |
| memberPath | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Path to a member. If the member is to be indexed, it has to end with /[] eg. `"myArray/[]"` |

##### Remarks

Has to be called inside of [IMultiplayerInit](#T-UnofficialMultiplayerAPI-IMultiplayerInit 'UnofficialMultiplayerAPI.IMultiplayerInit').[Init](#M-UnofficialMultiplayerAPI-IMultiplayerInit-Init 'UnofficialMultiplayerAPI.IMultiplayerInit.Init')

<a name='M-UnofficialMultiplayerAPI-MPApi-Watch-UnofficialMultiplayerAPI-ISyncField,System-Object,System-Object-'></a>
### Watch(field,target,index) `method`

##### Summary

An alias for [Watch](#M-UnofficialMultiplayerAPI-ISyncField-Watch-System-Object,System-Object- 'UnofficialMultiplayerAPI.ISyncField.Watch(System.Object,System.Object)')

##### Returns

`void`

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| field | [UnofficialMultiplayerAPI.ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') | [ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') object to watch |
| target | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | An object of type set in the [ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField'). If null, a static field will be used instead |
| index | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | Index in the field path set in [ISyncField](#T-UnofficialMultiplayerAPI-ISyncField 'UnofficialMultiplayerAPI.ISyncField') |

<a name='T-UnofficialMultiplayerAPI-SyncContext'></a>
## SyncContext `type`

##### Namespace

UnofficialMultiplayerAPI

##### Summary

Context flags which are sent along with a command

<a name='F-UnofficialMultiplayerAPI-SyncContext-CurrentMap'></a>
### CurrentMap `constants`

##### Summary

Send current map context

<a name='F-UnofficialMultiplayerAPI-SyncContext-MapMouseCell'></a>
### MapMouseCell `constants`

##### Summary

Send mouse cell context (Emulates mouse position)

<a name='F-UnofficialMultiplayerAPI-SyncContext-MapSelected'></a>
### MapSelected `constants`

##### Summary

Send map selected context (object selected on the map)

<a name='F-UnofficialMultiplayerAPI-SyncContext-None'></a>
### None `constants`

##### Summary

Send no context

<a name='F-UnofficialMultiplayerAPI-SyncContext-QueueOrder_Down'></a>
### QueueOrder_Down `constants`

##### Summary

Send order queue context (Emulates pressing KeyBindingDefOf.QueueOrder)

<a name='F-UnofficialMultiplayerAPI-SyncContext-WorldSelected'></a>
### WorldSelected `constants`

##### Summary

Send world selected context (object selected on the world map)

<a name='T-UnofficialMultiplayerAPI-SyncMethodAttribute'></a>
## SyncMethodAttribute `type`

##### Namespace

UnofficialMultiplayerAPI

##### Summary

An attribute that is used to mark methods for syncing.

<a name='M-UnofficialMultiplayerAPI-SyncMethodAttribute-#ctor-UnofficialMultiplayerAPI-SyncContext-'></a>
### #ctor(context) `constructor`

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [UnofficialMultiplayerAPI.SyncContext](#T-UnofficialMultiplayerAPI-SyncContext 'UnofficialMultiplayerAPI.SyncContext') | Context |
