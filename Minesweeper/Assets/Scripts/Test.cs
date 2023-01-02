using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public Tile testTile;
    // Start is called before the first frame update
    void Start()
    {
        int i= 1;
        Vector3 pos = Vector3.zero;

        while(i < 9)
        {
            Instantiate(testTile, Vector3.zero + Vector3.right * i,Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            testTile.ChangeState(TileState.Clicked, 5);
        }
    }
}
