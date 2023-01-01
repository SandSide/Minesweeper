using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject tilePrefab;
    public int bombAmountToSpawn = 10;
    public int width = 10;
    public int height = 10;
    public float nodeSize = .5f;

    private Dictionary<Vector2, GameObject> tileDictonary = new Dictionary<Vector2, GameObject>();
    private Grid gridInstance;
    private List<GridNode> visitedNodes = new List<GridNode>();

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
        PlaceBombs(bombAmountToSpawn);

    }

        // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
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
        tilePrefab.transform.localScale = tileScale;
        
        int id = 0;

        // Instance tile for each gridnode as a child
        foreach (var n in gridInstance.grid)
        {
            GameObject temp = Instantiate(tilePrefab, n.position, Quaternion.Euler(Vector3.zero), tileStorage.transform);
            tileDictonary.Add(n.gridPos, temp);
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
        // Get Grid node based on mouse pos
        Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridNode node = gridInstance.WorldPositionToGridNode(cameraPos);

        // If if it wasnt clciked before
        if(node.isHidden)
        {
            GameObject tile = tileDictonary[node.gridPos];

            // Change state
            if(node.hasBomb)
            {
                tile.GetComponent<Tile>().ChangeState(TileState.HasBomb);
            }
            else
            {
                // COunt bombs around node
                int bombCount = CheckNeighbours(node);
                tile.GetComponent<Tile>().ChangeState(TileState.Clicked, bombCount);
                
                // If not bombs around it
                if(bombCount == 0){
                    visitedNodes = new List<GridNode>();
                    RecursiveTraverse(node);
                }


            } 
 
            node.isHidden = false;   
        }
    }

    /// <summary>
    /// Get list of nodes around node
    /// </summary>
    /// <param name="node"> Node to get neighbours of </param>
    /// <returns> List of neighbour nodes </returns>
    public List<GridNode> GetNeighbourNodes(GridNode node)
    {
        List<GridNode> nodeList = new List<GridNode>();

        int x = (int)node.gridPos.x;
        int y = (int)node.gridPos.y;

        for(int i = x - 1; i <= x + 1 && i < gridInstance.width; i++)
        {
            for(int j = y - 1; j <= y + 1; j++)
            {

                if(i >= gridInstance.width || i <0)
                    continue;

                if(j >= gridInstance.height || j <0)
                    continue;
             
                if(x == i && y == j)
                    continue;

                nodeList.Add(gridInstance.grid[i,j]);
            }
        }

        return nodeList;
    }




    // public void ShowTilesAround(GridNode node)
    // {
    //     List<GridNode> neighbours = GetNeighbourNodes(node);
    //     List<GridNode> checkedNeighbours = new List<GridNode>();

    //     foreach( GridNode n in neighbours)
    //     {
            

            
            
    //         if(CheckNeighbours(n) == 0)
    //         {
    //             GameObject tile = tileDictonary[n.gridPos];
    //             tile.GetComponent<Tile>().ChangeState(TileState.Clicked);

    //             checkedNeighbours.Add(n);
                
    //         }
    //     }
    // }




    void RecursiveTraverse(GridNode currentNode)
    {
        
        List<GridNode> neighbours = GetNeighbourNodes(currentNode);

        if(CheckNeighbours(currentNode) == 0)
        {

            GameObject tile = tileDictonary[currentNode.gridPos];

            tile.GetComponent<Tile>().ChangeState(TileState.Clicked);
            currentNode.isHidden = false;
            visitedNodes.Add(currentNode);



            foreach(GridNode n in neighbours)
            {
                tile = tileDictonary[n.gridPos];
                tile.GetComponent<Tile>().ChangeState(TileState.Clicked, CheckNeighbours(n));
                n.isHidden = false;


                if(!visitedNodes.Contains(n))
                    RecursiveTraverse(n);
            }

        }
    }









    /// <summary>
    /// Get number of bobms around gridnode
    /// </summary>
    /// <param name="node"> Node to count around </param>
    /// <returns> Number of bombs around node </returns>
    public int CheckNeighbours(GridNode node)
    {

        List<GridNode> neighbours = GetNeighbourNodes(node);
        int bombCount = 0;

        foreach (GridNode n in neighbours)
        {
            if(n.hasBomb)
                bombCount++;
        }

        return bombCount;
    }

}
