using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dictionary<int, GridNode> tileTable = new Dictionary<int, GridNode>();
    public GameObject tile;
    private Grid gridInstance;
    public int bombAmount = 10;

    // Start is called before the first frame update
    void Start()
    {
        gridInstance = GameObject.Find("Grid").GetComponent(typeof(Grid)) as Grid;
        Vector3 tileScale = new Vector3(gridInstance.nodeSize, gridInstance.nodeSize, gridInstance.nodeSize);
        PlaceTiles(tileScale);
        AddBombs(bombAmount);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Button Pressed");
            SelectTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void PlaceTiles(Vector3 tileScale)
    {
        GameObject tileStorage = new GameObject("Tiles");
        tile.transform.localScale = tileScale;
        
        int id = 0;

        foreach (var n in gridInstance.grid)
        {
            GameObject temp = Instantiate(tile, n.position, Quaternion.Euler(Vector3.zero), tileStorage.transform);
            temp.name += id.ToString();
            AddTile(temp, n);

            id++;
        }
    }

    public void AddBombs(int bombsToSpawn)
    {
        List<GridNode> placeableNodes = new List<GridNode>();

        foreach(GridNode n in gridInstance.grid)
        {   
            placeableNodes.Add(n);
        }

        while(bombsToSpawn >0)
        {
            int temp = Random.Range(0, placeableNodes.Count -1);
            placeableNodes[temp].hasBomb = true;

            placeableNodes.RemoveAt(temp);
            bombsToSpawn--;
        }
    }

    public void AddTile(GameObject tile, GridNode node)
    {
        int id = tile.GetInstanceID();

        if (!tileTable.ContainsKey(id))
        {
            tileTable.Add(id, node);
        }
    }

    public void SelectTile(Vector3 worldPos)
    {
        Collider2D  col = Physics2D.OverlapPoint(worldPos);

        if(col != null)
        {      
            int id = col.gameObject.GetInstanceID();

            if (tileTable.ContainsKey(id))
            {
                if(tileTable[id].hasBomb)
                    Debug.Log("Game Over");
                else
                    CheckNeighbours(tileTable[id]);
            }
        }
    }

    public void CheckNeighbours(GridNode node)
    {
        int x = (int)node.gridPos.x;
        int y = (int)node.gridPos.y;

        int bombCount = 0;
        for(int i = x - 1; i <= x + 1 && i < gridInstance.width; i++)
        {

            for(int j = y - 1; j <= y + 1; j++)
            {

                if(i >= gridInstance.width || i <0)
                    continue;

                if(j >= gridInstance.height || j <0)
                    continue;

                gridInstance.grid[i,j].test = true;
                
                if(gridInstance.grid[i,j].hasBomb)
                    bombCount++;
            }
        }
        
        Debug.Log("Number of Bombs " + bombCount);
    }
}
