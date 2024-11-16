using System;
using Mono.CecilX;

namespace Mirror.Weaver
{
    public static class WeaverTypes
    {
        public static MethodReference ScriptableObjectCreateInstanceMethod;

<<<<<<< Updated upstream
        public static MethodReference NetworkBehaviourDirtyBitsReference;
        public static MethodReference GetPooledWriterReference;
        public static MethodReference RecycleWriterReference;
=======
        public MethodReference NetworkBehaviourDirtyBitsReference;
        public MethodReference GetWriterReference;
        public MethodReference ReturnWriterReference;
>>>>>>> Stashed changes

        public static MethodReference ReadyConnectionReference;

        public static MethodReference CmdDelegateConstructor;

        public static MethodReference NetworkServerGetActive;
        public static MethodReference NetworkServerGetLocalClientActive;
        public static MethodReference NetworkClientGetActive;

        // custom attribute types
        public static MethodReference InitSyncObjectReference;

        // array segment
        public static MethodReference ArraySegmentConstructorReference;

        // syncvar
        public static MethodReference syncVarEqualReference;
        public static MethodReference syncVarNetworkIdentityEqualReference;
        public static MethodReference syncVarGameObjectEqualReference;
        public static MethodReference setSyncVarReference;
        public static MethodReference setSyncVarHookGuard;
        public static MethodReference getSyncVarHookGuard;
        public static MethodReference setSyncVarGameObjectReference;
        public static MethodReference getSyncVarGameObjectReference;
        public static MethodReference setSyncVarNetworkIdentityReference;
        public static MethodReference getSyncVarNetworkIdentityReference;
        public static MethodReference syncVarNetworkBehaviourEqualReference;
        public static MethodReference setSyncVarNetworkBehaviourReference;
        public static MethodReference getSyncVarNetworkBehaviourReference;
        public static MethodReference registerCommandDelegateReference;
        public static MethodReference registerRpcDelegateReference;
        public static MethodReference getTypeFromHandleReference;
        public static MethodReference logErrorReference;
        public static MethodReference logWarningReference;
        public static MethodReference sendCommandInternal;
        public static MethodReference sendRpcInternal;
        public static MethodReference sendTargetRpcInternal;

        public static MethodReference readNetworkBehaviourGeneric;

<<<<<<< Updated upstream
        static AssemblyDefinition currentAssembly;

        public static TypeReference Import<T>() => Import(typeof(T));
=======
        // attributes
        public TypeDefinition initializeOnLoadMethodAttribute;
        public TypeDefinition runtimeInitializeOnLoadMethodAttribute;
>>>>>>> Stashed changes

        public static TypeReference Import(Type t) => currentAssembly.MainModule.ImportReference(t);

        public static void SetupTargetTypes(AssemblyDefinition currentAssembly)
        {
            // system types
            WeaverTypes.currentAssembly = currentAssembly;

            TypeReference ArraySegmentType = Import(typeof(ArraySegment<>));
            ArraySegmentConstructorReference = Resolvers.ResolveMethod(ArraySegmentType, currentAssembly, ".ctor");

<<<<<<< Updated upstream
            TypeReference ListType = Import(typeof(System.Collections.Generic.List<>));

            TypeReference NetworkServerType = Import(typeof(NetworkServer));
            NetworkServerGetActive = Resolvers.ResolveMethod(NetworkServerType, currentAssembly, "get_active");
            NetworkServerGetLocalClientActive = Resolvers.ResolveMethod(NetworkServerType, currentAssembly, "get_localClientActive");
            TypeReference NetworkClientType = Import(typeof(NetworkClient));
            NetworkClientGetActive = Resolvers.ResolveMethod(NetworkClientType, currentAssembly, "get_active");

            TypeReference cmdDelegateReference = Import<RemoteCalls.CmdDelegate>();
            CmdDelegateConstructor = Resolvers.ResolveMethod(cmdDelegateReference, currentAssembly, ".ctor");

            TypeReference NetworkBehaviourType = Import<NetworkBehaviour>();
            TypeReference RemoteCallHelperType = Import(typeof(RemoteCalls.RemoteCallHelper));

            TypeReference ScriptableObjectType = Import<UnityEngine.ScriptableObject>();

            ScriptableObjectCreateInstanceMethod = Resolvers.ResolveMethod(
                ScriptableObjectType, currentAssembly,
                md => md.Name == "CreateInstance" && md.HasGenericParameters);

            NetworkBehaviourDirtyBitsReference = Resolvers.ResolveProperty(NetworkBehaviourType, currentAssembly, "syncVarDirtyBits");
            TypeReference NetworkWriterPoolType = Import(typeof(NetworkWriterPool));
            GetPooledWriterReference = Resolvers.ResolveMethod(NetworkWriterPoolType, currentAssembly, "GetWriter");
            RecycleWriterReference = Resolvers.ResolveMethod(NetworkWriterPoolType, currentAssembly, "Recycle");

            TypeReference ClientSceneType = Import(typeof(ClientScene));
            ReadyConnectionReference = Resolvers.ResolveMethod(ClientSceneType, currentAssembly, "get_readyConnection");

            syncVarEqualReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SyncVarEqual");
            syncVarNetworkIdentityEqualReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SyncVarNetworkIdentityEqual");
            syncVarGameObjectEqualReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SyncVarGameObjectEqual");
            setSyncVarReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SetSyncVar");
            setSyncVarHookGuard = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "setSyncVarHookGuard");
            getSyncVarHookGuard = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "getSyncVarHookGuard");

            setSyncVarGameObjectReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SetSyncVarGameObject");
            getSyncVarGameObjectReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "GetSyncVarGameObject");
            setSyncVarNetworkIdentityReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SetSyncVarNetworkIdentity");
            getSyncVarNetworkIdentityReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "GetSyncVarNetworkIdentity");
            syncVarNetworkBehaviourEqualReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SyncVarNetworkBehaviourEqual");
            setSyncVarNetworkBehaviourReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SetSyncVarNetworkBehaviour");
            getSyncVarNetworkBehaviourReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "GetSyncVarNetworkBehaviour");

            registerCommandDelegateReference = Resolvers.ResolveMethod(RemoteCallHelperType, currentAssembly, "RegisterCommandDelegate");
            registerRpcDelegateReference = Resolvers.ResolveMethod(RemoteCallHelperType, currentAssembly, "RegisterRpcDelegate");
=======
            TypeReference ActionType = Import(typeof(Action<,>));
            ActionT_T = Resolvers.ResolveMethod(ActionType, assembly, Log, ".ctor", ref WeavingFailed);

            TypeReference NetworkServerType = Import(typeof(NetworkServer));
            NetworkServerGetActive = Resolvers.ResolveMethod(NetworkServerType, assembly, Log, "get_active", ref WeavingFailed);
            TypeReference NetworkClientType = Import(typeof(NetworkClient));
            NetworkClientGetActive = Resolvers.ResolveMethod(NetworkClientType, assembly, Log, "get_active", ref WeavingFailed);

            TypeReference RemoteCallDelegateType = Import<RemoteCalls.RemoteCallDelegate>();
            RemoteCallDelegateConstructor = Resolvers.ResolveMethod(RemoteCallDelegateType, assembly, Log, ".ctor", ref WeavingFailed);

            TypeReference NetworkBehaviourType = Import<NetworkBehaviour>();
            TypeReference RemoteProcedureCallsType = Import(typeof(RemoteCalls.RemoteProcedureCalls));

            TypeReference ScriptableObjectType = Import<ScriptableObject>();

            ScriptableObjectCreateInstanceMethod = Resolvers.ResolveMethod(
                ScriptableObjectType, assembly, Log,
                md => md.Name == "CreateInstance" && md.HasGenericParameters,
                ref WeavingFailed);

            NetworkBehaviourDirtyBitsReference = Resolvers.ResolveProperty(NetworkBehaviourType, assembly, "syncVarDirtyBits");
            TypeReference NetworkWriterPoolType = Import(typeof(NetworkWriterPool));
            GetWriterReference = Resolvers.ResolveMethod(NetworkWriterPoolType, assembly, Log, "Get", ref WeavingFailed);
            ReturnWriterReference = Resolvers.ResolveMethod(NetworkWriterPoolType, assembly, Log, "Return", ref WeavingFailed);

            NetworkClientConnectionReference = Resolvers.ResolveMethod(NetworkClientType, assembly, Log, "get_connection", ref WeavingFailed);

            generatedSyncVarSetter = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GeneratedSyncVarSetter", ref WeavingFailed);
            generatedSyncVarSetter_GameObject = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GeneratedSyncVarSetter_GameObject", ref WeavingFailed);
            generatedSyncVarSetter_NetworkIdentity = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GeneratedSyncVarSetter_NetworkIdentity", ref WeavingFailed);
            generatedSyncVarSetter_NetworkBehaviour_T = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GeneratedSyncVarSetter_NetworkBehaviour", ref WeavingFailed);

            generatedSyncVarDeserialize_GameObject = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GeneratedSyncVarDeserialize_GameObject", ref WeavingFailed);
            generatedSyncVarDeserialize = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GeneratedSyncVarDeserialize", ref WeavingFailed);
            generatedSyncVarDeserialize_NetworkIdentity = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GeneratedSyncVarDeserialize_NetworkIdentity", ref WeavingFailed);
            generatedSyncVarDeserialize_NetworkBehaviour_T = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GeneratedSyncVarDeserialize_NetworkBehaviour", ref WeavingFailed);

            getSyncVarGameObjectReference = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GetSyncVarGameObject", ref WeavingFailed);
            getSyncVarNetworkIdentityReference = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GetSyncVarNetworkIdentity", ref WeavingFailed);
            getSyncVarNetworkBehaviourReference = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "GetSyncVarNetworkBehaviour", ref WeavingFailed);

            registerCommandReference = Resolvers.ResolveMethod(RemoteProcedureCallsType, assembly, Log, "RegisterCommand", ref WeavingFailed);
            registerRpcReference = Resolvers.ResolveMethod(RemoteProcedureCallsType, assembly, Log, "RegisterRpc", ref WeavingFailed);
>>>>>>> Stashed changes

            TypeReference unityDebug = Import(typeof(UnityEngine.Debug));
            // these have multiple methods with same name, so need to check parameters too
            logErrorReference = Resolvers.ResolveMethod(unityDebug, currentAssembly, (md) =>
            {
                return md.Name == "LogError" &&
                    md.Parameters.Count == 1 &&
                    md.Parameters[0].ParameterType.FullName == typeof(object).FullName;
            });
            logWarningReference = Resolvers.ResolveMethod(unityDebug, currentAssembly, (md) =>
            {
                return md.Name == "LogWarning" &&
                    md.Parameters.Count == 1 &&
                    md.Parameters[0].ParameterType.FullName == typeof(object).FullName;

            });

            TypeReference typeType = Import(typeof(Type));
<<<<<<< Updated upstream
            getTypeFromHandleReference = Resolvers.ResolveMethod(typeType, currentAssembly, "GetTypeFromHandle");
            sendCommandInternal = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SendCommandInternal");
            sendRpcInternal = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SendRPCInternal");
            sendTargetRpcInternal = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "SendTargetRPCInternal");

            InitSyncObjectReference = Resolvers.ResolveMethod(NetworkBehaviourType, currentAssembly, "InitSyncObject");
=======
            getTypeFromHandleReference = Resolvers.ResolveMethod(typeType, assembly, Log, "GetTypeFromHandle", ref WeavingFailed);
            sendCommandInternal = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "SendCommandInternal", ref WeavingFailed);
            sendRpcInternal = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "SendRPCInternal", ref WeavingFailed);
            sendTargetRpcInternal = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "SendTargetRPCInternal", ref WeavingFailed);

            InitSyncObjectReference = Resolvers.ResolveMethod(NetworkBehaviourType, assembly, Log, "InitSyncObject", ref WeavingFailed);
>>>>>>> Stashed changes

            TypeReference readerExtensions = Import(typeof(NetworkReaderExtensions));
            readNetworkBehaviourGeneric = Resolvers.ResolveMethod(readerExtensions, currentAssembly, (md =>
            {
                return md.Name == nameof(NetworkReaderExtensions.ReadNetworkBehaviour) &&
                    md.HasGenericParameters;
            }));
        }
    }
}
