using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject levelPrefab;
    public static GameManager instance;
    GameObject currentGame;

    void Awake()
    {
        // Make the object singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }
    
    // Start is called before the first frame update
    void Start()
    {
        currentGame = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
    }

}


public enum LevelState{
    Over,
    Active
}


