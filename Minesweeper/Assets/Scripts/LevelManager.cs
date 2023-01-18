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

    public Dictionary<Vector2, GameObject> tileDictonary = new Dictionary<Vector2, GameObject>();
    private Grid gridInstance;
    private LevelState levelState;
    private GridNode currentHoverNode;

    private int bombsLeft;
    private int timerSeconds;

    void Awake()
    {
        if(levelData!= null)
            StartLevel();
    }


    // Update is called once per frame
    void Update()
    {
        if(levelState == LevelState.Active)
        {
            HandleInput();
            HighlightNode();
        }

    }

    /// <summary>
    /// Creates the grid and starts the level
    /// </summary>
    public void StartLevel()
    {
        SetUpLevel();
        UIManager.instance.SwitchScreen("Game Overlay");

        levelState = LevelState.Active;
        
        StartCoroutine(Timer());
    }

    /// <summary>
    /// Creates the grid, tiles and placeds bombs onto the grid to get to be interaced with
    /// </summary>
    public void SetUpLevel()
    {
        // Create Grid
        gridInstance = GetComponent<Grid>();
        gridInstance.width = levelData.width;
        gridInstance.height = levelData.height;
        gridInstance.nodeSize = levelData.nodeSize;
        gridInstance.CreateGrid();
        Vector3 tileScale = new Vector3(gridInstance.nodeSize, gridInstance.nodeSize, gridInstance.nodeSize);


        PlaceTiles(tileScale);
        PlaceBombs(levelData.bombAmount);
        ReSizeBackGround();

        timerSeconds = 0;

        // Update UI
        UIManager.instance.UpdateBombCount(levelData.bombAmount);
        bombsLeft = levelData.bombAmount;
    }

    /// <summary>
    /// Handle User input
    /// </summary>
    public void HandleInput()
    {

        bool madeAMove = false;

        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ClickTile(mousePos);
            madeAMove = true;
        }

        if(Input.GetMouseButtonDown(1))
        {
            FlagTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            madeAMove = true;
        }

        if(madeAMove)
        {
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

    /// <summary>
    /// Reszies background iamge of grid to surrond the grid nodes
    /// </summary>
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
    public void ClickTile(Vector3 worldPos)
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
                int bombCount = gridInstance.CheckNeighbours(node);
                tile.GetComponent<Tile>().ChangeState(TileState.Clicked, bombCount);
                
                // If not bombs around it
                if(bombCount == 0){
                    gridInstance.RecursiveTraverse(node, tileDictonary);
                }
            } 
 
            node.nodeState = NodeState.Clicked;   
        }
    }

    /// <summary>
    /// Flaggs/Unflaggs grid ndoe nearst to posiiton
    /// </summary>
    /// <param name="worldPos"> Position to find nearst node of</param>
    public void FlagTile(Vector3 worldPos)
    {
        // Get Grid node based on mouse pos
        Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridNode node = gridInstance.WorldPositionToGridNode(cameraPos);

        if(node.nodeState == NodeState.Hidden)
        {            
            tileDictonary[node.gridPos].GetComponent<Tile>().ChangeState(TileState.Flagged);
            node.nodeState = NodeState.Flagged;
            UIManager.instance.UpdateBombCount(levelData.bombAmount);
            bombsLeft--;
        }
        else if(node.nodeState == NodeState.Flagged)
        {
            tileDictonary[node.gridPos].GetComponent<Tile>().ChangeState(TileState.Hidden);
            node.nodeState = NodeState.Hidden;
            bombsLeft++;
        }

        UIManager.instance.UpdateBombCount(bombsLeft);
    }


    public void HighlightNode()
    {
        Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridNode node = gridInstance.WorldPositionToGridNode(cameraPos);
        
        if(node == currentHoverNode)
            return;

        if(currentHoverNode != null && currentHoverNode.nodeState == NodeState.Hidden)
            tileDictonary[currentHoverNode.gridPos].GetComponent<Tile>().ChangeState(TileState.Hidden);

        currentHoverNode = null;

    
        if(node.nodeState == NodeState.Hidden)
        {
            tileDictonary[node.gridPos].GetComponent<Tile>().ChangeState(TileState.Highlight);
            currentHoverNode = node;
        }  
        
    }

    /// <summary>
    /// Checks if player meets the win condititon
    /// </summary>
    /// <param name="node"> Node to get neighbours of </param>
    /// <returns> List of neighbour nodes </returns>
    public bool CheckForWinCondition()
    {
        int minClickedNodes = (levelData.height * levelData.width) - levelData.bombAmount;
        
        // Checks to see if any node with no bombs has not been clicked
        for(int i = 0; i < levelData.height; i++)
        {
            for(int j = 0; j < levelData.height; j++)
            {
                GridNode node = gridInstance.grid[i,j];

                if((node.nodeState == NodeState.Hidden || node.nodeState == NodeState.Flagged) && !node.hasBomb)
                    return false;

            }
        }

        return true;
    }

    /// <summary>
    /// Game has been lost
    /// </summary>
    public void GameOver()
    {
        levelState = LevelState.Over;
        UIManager.instance.SwitchScreen("Game Over");
    }

    /// <summary>
    /// Game has been won
    /// </summary>
    public void GameWon()
    {
        levelState = LevelState.Over;
        UIManager.instance.SwitchScreen("Game Won");
    }


    IEnumerator Timer()
    {
        while (true)
        {
            timerSeconds += 1;
            UIManager.instance.UpdateTimer(timerSeconds);
            yield return new WaitForSeconds(1.0f);
        }
    }
}

