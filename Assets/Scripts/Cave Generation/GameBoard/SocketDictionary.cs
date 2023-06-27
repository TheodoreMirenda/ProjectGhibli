using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/SocketDictionary")]
public class SocketDictionary : ScriptableObject
{
    [SerializeField] private SocketSO grassGrassSocket, castleGrassSocket, grassCastleSocket, castleCastleSocket;
    public SocketSO GetSocket(int a, int b)
    {
        switch (a,b)
        {
            case (0,0):
                return grassGrassSocket;
            case (0,1):
                return grassCastleSocket;
            case (1,0):
                return castleGrassSocket;
            case (1,1):
                return castleCastleSocket;
            default:
                return null;
        }
    }
}
