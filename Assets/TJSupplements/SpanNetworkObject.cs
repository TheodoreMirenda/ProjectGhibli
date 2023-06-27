// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;
// using System.Threading.Tasks;

// namespace TJ.Supplements
// {
// public static class SpawnNetworkObject
// {
//     public static async Task WaitUntilSpawned(NetworkObject networkObject)
//     {
//         if(!networkObject.IsSpawned)
//             networkObject.Spawn();

//         while (!networkObject.IsSpawned)
//             await Task.Yield();
//     }
// }
// }
