using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public Vector3 position;
    public Vector2 gridPos;
    public bool hasBomb;
    public bool isHidden;

    public bool test = false;

    public GridNode(Vector3 pos, bool hasBomb, int x, int y, bool isHidden)
    {
        position = pos;
        this.hasBomb = hasBomb;
        gridPos.x = x;
        gridPos.y = y;
        this.isHidden = isHidden;
    }
}
