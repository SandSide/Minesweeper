using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Dictionary<Vector2, GameObject> tileDictonary = new Dictionary<Vector2, GameObject>();
    private Grid gridInstance;

    public GameObject tile;
    public int bombAmount = 10;
    public int width = 10;
    public int height = 10;
    public float nodeSize = .5f;


    void Awake()
    {
        gridInstance = GetComponent<Grid>();

        Debug.Log(gridInstance);
        gridInstance.width = width;
        gridInstance.height = height;
        gridInstance.nodeSize = nodeSize;

        gridInstance.CreateGrid();

        Vector3 tileScale = new Vector3(gridInstance.nodeSize, gridInstance.nodeSize, gridInstance.nodeSize);

        PlaceTiles(tileScale);
        PlaceBombs(bombAmount);

    }

        // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Button Pressed");
            Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SelectTile(cameraPos);
        }
    }

    /// <summary>
    /// Places tiles on grid node posiitons
    /// </summary>
    /// <param name="tileScale"> Scale of the tile </param>
    void PlaceTiles(Vector3 tileScale)
    {
        // Create storage object
        GameObject tileStorage = new GameObject("Tiles");
        tileStorage.transform.parent = transform;
        tile.transform.localScale = tileScale;
        
        int id = 0;

        // Instance tile for each gridnode as a child
        foreach (var n in gridInstance.grid)
        {
            GameObject temp = Instantiate(tile, n.position, Quaternion.Euler(Vector3.zero), tileStorage.transform);
            tileDictonary.Add(n.gridPos, temp);

            temp.GetComponent<SpriteRenderer>().color = TileStateColours.Hidden;
            temp.name += id.ToString();
            id++;
        }
    }



    /// <summary>
    /// Adds n bombs onto the grid
    /// </summary>
    /// <param name="bombsToSpawn"> Number of bombs to spawn</param>
    public void PlaceBombs(int bombsToSpawn)
    {
        // Store palcable locations
        List<GridNode> placeableNodes = new List<GridNode>();


        // Make each grid node a possible bomb placement location
        foreach(GridNode n in gridInstance.grid)
        {   
            placeableNodes.Add(n);
        }


        // Assign bobms to grid nodes
        while(bombsToSpawn >0) 
        {
            // Get random node in list
            int temp = Random.Range(0, placeableNodes.Count -1);
            placeableNodes[temp].hasBomb = true;
            placeableNodes.RemoveAt(temp);
            bombsToSpawn--;
        }
    }


    /// <summary>
    /// Get closests node to world position
    /// </summary>
    /// <param name="worldPos"> World position to get gridnode of </param>
    public void SelectTile(Vector3 worldPos)
    {
        Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridNode node = gridInstance.WorldPositionToGridNode(cameraPos);

        if(node.isHidden)
        {
            if(node.hasBomb)
            {
                ChangeTileColour(tileDictonary[node.gridPos], TileStateColours.HasBomb);
            }
            else
            {
                CheckNeighbours(node);
                ChangeTileColour(tileDictonary[node.gridPos], TileStateColours.Clicked);
            } 
 
            node.isHidden = false;   
        }
    }

    public void ChangeTileColour(GameObject tile, Color Colour)
    {
        tile.GetComponent<SpriteRenderer>().color = Colour;
    }


    /// <summary>
    /// Get number of bobms around gridnode
    /// </summary>
    /// <param name="node"> Node to count around </param>
    /// <returns> Number of bombs around node </returns>
    public int CheckNeighbours(GridNode node)
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
        
        return bombAmount;
    }

}
