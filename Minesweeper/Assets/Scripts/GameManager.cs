using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject levelPrefab;
    public static GameManager instance;
    GameObject currentGame;

    [Header("UI Screens")]
    public GameObject gameOverScreen;

    [Header("Game Level Buttons")]
    public Button easyLevel;
    public Button normalLevel;
    public Button hardLevel;

    public LevelScriptableObject[] levels;

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


    void Start()
    {
        easyLevel.onClick.AddListener(delegate {StartLevel("Easy"); });
        normalLevel.onClick.AddListener(delegate {StartLevel("Normal"); });
        hardLevel.onClick.AddListener(delegate {StartLevel("Hard"); });

        UIManager.instance.SwitchScreen("Main Menu");
    }
    

    /// <summary>
    /// Starts the level mentioned
    /// </summary>
    /// <param name="levelName"> Name of the levle to start</param>
    public void StartLevel(string levelName)
    {
        // Find level
        foreach(LevelScriptableObject level in levels)
        {
            if (level.name == levelName)
            {
                // Create levle instances and give levle details
                currentGame = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
                currentGame.GetComponent<LevelManager>().levelData = level;
                currentGame.GetComponent<LevelManager>().StartLevel();
            }
        }
    }

    /// <summary>
    /// End current active level
    /// </summary>
    public void EndLevel()
    {
        // If there is a current game
        if(currentGame != null)
        {
            Destroy(currentGame);
            currentGame = null;

            UIManager.instance.SwitchScreen("Main Menu");
        }
    }

    /// <summary>
    /// Restart current active level
    /// </summary>
    public void RestartLevel()
    {
        if(currentGame != null)
        {
            LevelScriptableObject levelData = currentGame.GetComponent<LevelManager>().levelData;
            EndLevel();
            StartLevel(levelData.name);
        }
    }
}


public enum LevelState{
    Over,
    Active
}


