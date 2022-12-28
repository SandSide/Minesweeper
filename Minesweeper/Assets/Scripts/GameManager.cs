using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject levelPrefab;
    GameObject currentGame;
    
    // Start is called before the first frame update
    void Start()
    {

        currentGame = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);

    }

}

public struct TileStateColours
{
    public static Color HasBomb = Color.red;
    public static Color Hidden = Color.gray;
    public static Color Clicked = Color.blue;
}
