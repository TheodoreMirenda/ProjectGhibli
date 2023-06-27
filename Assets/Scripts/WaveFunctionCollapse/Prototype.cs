using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/Prototype")]
public class Prototype : ScriptableObject
{
    public Prototype basePrototype;
    public int meshRotation;
    public GameObject prefab;
    public SocketSO posZ;
    public SocketSO posX;
    public SocketSO negZ;
    public SocketSO negX;
    public PrototypeClass prototypeClass;
    public NeighbourList validNeighbours;
}
[System.Serializable] public class NeighbourList
{
    public List<Prototype> posX = new List<Prototype>();
    public List<Prototype> posZ = new List<Prototype>();
    public List<Prototype> negX = new List<Prototype>();
    public List<Prototype> negZ = new List<Prototype>();
}
public enum PrototypeClass {Grass, Road, River}