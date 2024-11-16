using System;
<<<<<<< Updated upstream
using System.Runtime.InteropServices;
=======
using System.Runtime.CompilerServices;
>>>>>>> Stashed changes
using System.Security.Cryptography;
using UnityEngine;

namespace Mirror
{
    // Handles network messages on client and server
    public delegate void NetworkMessageDelegate(NetworkConnection conn, NetworkReader reader, int channelId);

    // Handles requests to spawn objects on the client
    public delegate GameObject SpawnDelegate(Vector3 position, Guid assetId);

    public delegate GameObject SpawnHandlerDelegate(SpawnMessage msg);

    // Handles requests to unspawn objects on the client
    public delegate void UnSpawnDelegate(GameObject spawned);

<<<<<<< Updated upstream
    // invoke type for Cmd/Rpc
    public enum MirrorInvokeType
    {
        Command,
        ClientRpc
    }

    public enum Version
    {
        Current = 1
    }

    public static class Channels
    {
        public const int DefaultReliable = 0;
        public const int DefaultUnreliable = 1;
    }

    // -- helpers for float conversion without allocations --
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntFloat
    {
        [FieldOffset(0)]
        public float floatValue;

        [FieldOffset(0)]
        public uint intValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntDouble
    {
        [FieldOffset(0)]
        public double doubleValue;

        [FieldOffset(0)]
        public ulong longValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntDecimal
    {
        [FieldOffset(0)]
        public ulong longValue1;

        [FieldOffset(8)]
        public ulong longValue2;

        [FieldOffset(0)]
        public decimal decimalValue;
=======
    // channels are const ints instead of an enum so people can add their own
    // channels (can't extend an enum otherwise).
    //
    // note that Mirror is slowly moving towards quake style networking which
    // will only require reliable for handshake, and unreliable for the rest.
    // so eventually we can change this to an Enum and transports shouldn't
    // add custom channels anymore.
    public static class Channels
    {
        public const int Reliable = 0;   // ordered
        public const int Unreliable = 1; // unordered
>>>>>>> Stashed changes
    }

    public static class Utils
    {
        public static uint GetTrueRandomUInt()
        {
            // use Crypto RNG to avoid having time based duplicates
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }
        }

        public static bool IsPrefab(GameObject obj)
        {
#if UNITY_EDITOR
<<<<<<< Updated upstream
    #if UNITY_2018_3_OR_NEWER
            return UnityEditor.PrefabUtility.IsPartOfPrefabAsset(obj);
    #elif UNITY_2018_2_OR_NEWER
            return UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(obj) == null &&
                   UnityEditor.PrefabUtility.GetPrefabObject(obj) != null;
    #else
            return UnityEditor.PrefabUtility.GetPrefabParent(obj) == null &&
                   UnityEditor.PrefabUtility.GetPrefabObject(obj) != null;
    #endif
=======
            return UnityEditor.PrefabUtility.IsPartOfPrefabAsset(obj);
>>>>>>> Stashed changes
#else
            return false;
#endif
        }
<<<<<<< Updated upstream
=======

        public static bool IsSceneObjectWithPrefabParent(GameObject gameObject, out GameObject prefab)
        {
            prefab = null;

#if UNITY_EDITOR
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                return false;
            }
            prefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
#endif

            if (prefab == null)
            {
                Debug.LogError($"Failed to find prefab parent for scene object [name:{gameObject.name}]");
                return false;
            }
            return true;
        }

        // is a 2D point in screen? (from ummorpg)
        // (if width = 1024, then indices from 0..1023 are valid (=1024 indices)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointInScreen(Vector2 point) =>
            0 <= point.x && point.x < Screen.width &&
            0 <= point.y && point.y < Screen.height;

        // pretty print bytes as KB/MB/GB/etc. from DOTSNET
        // long to support > 2GB
        // divides by floats to return "2.5MB" etc.
        public static string PrettyBytes(long bytes)
        {
            // bytes
            if (bytes < 1024)
                return $"{bytes} B";
            // kilobytes
            else if (bytes < 1024L * 1024L)
                return $"{(bytes / 1024f):F2} KB";
            // megabytes
            else if (bytes < 1024 * 1024L * 1024L)
                return $"{(bytes / (1024f * 1024f)):F2} MB";
            // gigabytes
            return $"{(bytes / (1024f * 1024f * 1024f)):F2} GB";
        }

        // universal .spawned function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NetworkIdentity GetSpawnedInServerOrClient(uint netId)
        {
            // server / host mode: use the one from server.
            // host mode has access to all spawned.
            if (NetworkServer.active)
            {
                NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity entry);
                return entry;
            }

            // client
            if (NetworkClient.active)
            {
                NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity entry);
                return entry;
            }

            return null;
        }
>>>>>>> Stashed changes
    }
}
