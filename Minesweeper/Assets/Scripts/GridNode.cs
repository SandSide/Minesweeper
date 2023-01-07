using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public Vector3 position;
    public Vector2 gridPos;
    public bool hasBomb;
    public NodeState nodeState;

    public GridNode(Vector3 pos, bool hasBomb, int x, int y, NodeState nodeState)
    {
        position = pos;
        this.hasBomb = hasBomb;
        gridPos.x = x;
        gridPos.y = y;
        this.nodeState = nodeState;
    }
}

public enum NodeState{
    Hidden,
    Clicked,
    Flagged
}