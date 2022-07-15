using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbours<T>
{
    public T[,,] neighs = new T[3, 3, 3];

    public T GetNeighbour(Direction direction)
    {
        var arrPos = GameData.directions[direction] + Vector3Int.one;
        return neighs[arrPos.x, arrPos.y, arrPos.z];
    }

    public void SetNeighbour(T neigh, Direction direction)
    {
        var arrPos = GameData.directions[direction] + Vector3Int.one;
        neighs[arrPos.x, arrPos.y, arrPos.z] = neigh;
    }
}
