using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public TMP_Text bombNumber;
    public SpriteRenderer spriteRen;

    void Start()
    {
        spriteRen.color = Color.gray;
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
            spriteRen.color = Color.red;
        }
        if(newState == TileState.Clicked)
        {
            spriteRen.color = Color.white;

            if(bombNum > 0)
                bombNumber.text = bombNum.ToString();
        }
    }
}

public enum TileState{
    HasBomb,
    Clicked
}


