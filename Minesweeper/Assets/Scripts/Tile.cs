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
    public Color flaggedColour;
    public Color highlightColour;

    [Header("Number Colours")]
    public Color[] numberColour;

    void Awake()
    {
        ChangeState(TileState.Hidden);
    }

    /// <summary>
    /// Change state of tile
    /// </summary>
    /// <param name="newState"> New state of the tile</param>
    /// <param name="bombNum"> Number of bombs to dispaly as text </param>
    public void ChangeState(TileState newState, int bombNum = 0)
    {

        switch(newState)
        {
            case TileState.Hidden:
                spriteRen.color = hiddenColour;
                bombNumber.text = "";
                break;

            case TileState.HasBomb:
                spriteRen.color = HasBombColour;
                break;

            case TileState.Clicked:
                spriteRen.color = clickedColour;
                if(bombNum > 0)
                {
                    bombNumber.text = bombNum.ToString();
                    bombNumber.color = numberColour[bombNum-1];
                }
                break;   

            case TileState.Flagged:
                spriteRen.color = flaggedColour;
                break;

            case TileState.Highlight:
                spriteRen.color = highlightColour;
                break;
        }
    }
}

public enum TileState{
    HasBomb,
    Hidden,
    Clicked,
    Flagged,
    Highlight
}


