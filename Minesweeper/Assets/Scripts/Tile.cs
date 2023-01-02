using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    [Header("Tile Elements")]
    public TMP_Text bombNumber;
    public SpriteRenderer spriteRen;

    [Header("Tile Colours")]
    public Color hiddenColour;
    public Color HasBombColour;
    public Color clickedColour;

    [Header("Number Colours")]
    public Color[] numberColour;

    void Awake()
    {
        spriteRen.color = hiddenColour;
        bombNumber.text = "";
    }

    /// <summary>
    /// Change state of tile
    /// </summary>
    /// <param name="newState"> New state of the tile</param>
    /// <param name="bombNum"> Number of bombs to dispaly as text </param>
    public void ChangeState(TileState newState, int bombNum = 0)
    {
        // Change tile based on state
        if(newState == TileState.HasBomb)
        {
            spriteRen.color = HasBombColour;
        }
        if(newState == TileState.Clicked)
        {
            spriteRen.color = clickedColour;

            if(bombNum > 0)
            {
                bombNumber.text = bombNum.ToString();
                bombNumber.color = numberColour[bombNum-1];
            }

        }
    }
}

public enum TileState{
    HasBomb,
    Clicked
}


