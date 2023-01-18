using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct screenData{
    public string screenName;
    public GameObject screen;
}


/// <summary>
/// Manages screens in the scene. Switches current active scene with another
/// </summary>
public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    [SerializeField]
    public List<screenData> screenList;

    private GameObject currentScreen;

    [Header("Game Overlay Settings")]
    public TMP_Text bombCountText;
    public TMP_Text timerText;

    [Header("Game Over Buttons")]
    public Button retryLevel;
    public Button exitLevel;

    [Header("Game Won Buttons")]
    public Button exitLevelW;

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
        exitLevel.onClick.AddListener(GameManager.instance.EndLevel);
        retryLevel.onClick.AddListener(GameManager.instance.RestartLevel);
        exitLevelW.onClick.AddListener(GameManager.instance.EndLevel);
    }

    /// <summary>
    /// Switch current active sceene with a new scene
    /// </summary>
    /// <param name="screenName"> Scene to display</param>
    public void SwitchScreen(string screenName)
    {

        // Find if specified sceen exists
        foreach (screenData s in screenList)
        {   
            // Replace current active with this
            if(s.screenName == screenName)
            {
                if(currentScreen != null)
                    currentScreen.SetActive(false);

                currentScreen = s.screen;
                currentScreen.SetActive(true);
                return;
            }
        }

        Debug.Log($"Screen {screenName} not found");
    }


    /// <summary>
    /// Updates bomb amount text on game overlay
    /// </summary>
    /// <param name="bombAmmount"> Number to display</param>
    public void UpdateBombCount(int bombAmmount){
        bombCountText.text = bombAmmount.ToString();
    }

    /// <summary>
    /// Updates game time in game overlay
    /// </summary>
    /// <param name="seconds"> Game time in seconds</param>
    public void UpdateTimer(int seconds){

        timerText.text = seconds.ToString();
    }

}
