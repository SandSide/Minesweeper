using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class LevelScriptableObject : ScriptableObject
{
    public int width;
    public int height;
    public float nodeSize;
    public int bombAmount;

}
