using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the state of the game. At awake, it set us the levle by first generating the grid, tiles and placing the bombs.
/// It then handles users input and interaces with the grid
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Grid Prefabs")]
    public GameObject tilePrefab;
    public GameObject gridBackGround;
    
    [Header("Level Settings")]
    public LevelScriptableObject levelData;

    private Dictionary<Vector2, GameObject> tileDictonary = new Dictionary<Vector2, GameObject>();
    private Grid gridInstance;
    private List<GridNode> visitedNodes = new List<GridNode>();

    private LevelState levelState;

    void Awake()
    {
        if(levelData!= null)
            StartLevel();
    }


    // Update is called once per frame
    void Update()
    {
        if(levelState == LevelState.Active)
            HandleInput();
    }

    /// <summary>
    /// Creates the grid and starts the level
    /// </summary>
    public void StartLevel()
    {
        SetUpLevel();
        UIManager.instance.SwitchScreen("Game Overlay");
    }

    /// <summary>
    /// Creates the grid, tiles and placeds bombs onto the grid to get to be interaced with
    /// </summary>
    public void SetUpLevel()
    {
        gridInstance = GetComponent<Grid>();

        gridInstance.width = levelData.width;
        gridInstance.height = levelData.height;
        gridInstance.nodeSize = levelData.nodeSize;

        gridInstance.CreateGrid();

        Vector3 tileScale = new Vector3(gridInstance.nodeSize, gridInstance.nodeSize, gridInstance.nodeSize);

        PlaceTiles(tileScale);
        PlaceBombs(levelData.bombAmount);
        ReSizeBackGround();

        levelState = LevelState.Active;
    }

    /// <summary>
    /// Handle User input
    /// </summary>
    public void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SelectTile(cameraPos);

            if(CheckForWinCondition()) 
                GameWon();
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

    public void ReSizeBackGround()
    {
        Vector3 newBGScale = new Vector3(levelData.width * levelData.nodeSize , levelData.height*levelData.nodeSize, 1) + Vector3.one * levelData.nodeSize;
        gridBackGround.transform.localScale = newBGScale;

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
        if(node.nodeState == NodeState.Hidden)
        {
            GameObject tile = tileDictonary[node.gridPos];

            // Change state
            if(node.hasBomb)
            {
                tile.GetComponent<Tile>().ChangeState(TileState.HasBomb);
                GameOver();
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
 
            node.nodeState = NodeState.Clicked;   
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





    /// <summary>
    /// Traverse GridNodes around a gridnode which has bomb free enighbours
    /// </summary>
    /// <param name="node"> Node to traverse around </param>
    public void RecursiveTraverse(GridNode currentNode)
    {
        
        List<GridNode> neighbours = GetNeighbourNodes(currentNode);

        // If neighbours have no bombs
        if(CheckNeighbours(currentNode) == 0)
        {   

            // Get current node tile
            GameObject tile = tileDictonary[currentNode.gridPos];

            tile.GetComponent<Tile>().ChangeState(TileState.Clicked);
            currentNode.nodeState = NodeState.Clicked;
            visitedNodes.Add(currentNode);


            // Traverse neighbours
            foreach(GridNode n in neighbours)
            {
                tile = tileDictonary[n.gridPos];
                tile.GetComponent<Tile>().ChangeState(TileState.Clicked, CheckNeighbours(n));
                n.nodeState = NodeState.Clicked;


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

    public bool CheckForWinCondition()
    {
        int minClickedNodes = (levelData.height * levelData.width) - levelData.bombAmount;
        
        for(int i = 0; i < levelData.height; i++)
        {
            for(int j = 0; j < levelData.height; j++)
            {
                GridNode node = gridInstance.grid[i,j];

                if(node.nodeState == NodeState.Hidden && !node.hasBomb)
                    return false;

            }
        }

        return true;
    }

    public void GameOver()
    {
        levelState = LevelState.Over;
        UIManager.instance.SwitchScreen("Game Over");

    }

    public void GameWon()
    {
        levelState = LevelState.Over;
        UIManager.instance.SwitchScreen("Game Won");
    }
}

