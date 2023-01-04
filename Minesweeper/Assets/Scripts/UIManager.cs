using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
