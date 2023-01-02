using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public GameObject testTile;
    public float offset = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        int i= 1;
        Vector3 pos = Vector3.zero;
        

        while(i < 9)
        {
            Tile temp = Instantiate(testTile, Vector3.zero + Vector3.right * (offset* i),Quaternion.identity).GetComponent<Tile>();
            temp.ChangeState(TileState.Clicked, i);
            i++;
        }
    }

}
