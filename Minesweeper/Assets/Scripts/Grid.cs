using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is a component to create a grid of gridnodes with each ndoe being interacted based on mouse position
/// </summary>
public class Grid : MonoBehaviour
{

    [Header("Grid Attributes")]
    public int width;
    public int height;
    public float nodeSize;
    public bool displayGridGizmos;
    private Vector2 bottomLeftCornerPos;

    private Vector3 gizmoNodeSize = Vector3.one * 0.9f;
    private Color defaultNodeColour = Color.green;

    public GridNode[,] grid;
    private List<GridNode> visitedNodes = new List<GridNode>();

    // // Start is called before the first frame update
    // void Awake()
    // {
    //     CreateGrid();
    // }

    // void Update(){
    //     if(Input.GetMouseButtonDown(0))
    //     {
    //         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         GridNode test = WorldPositionToGridNode(mousePos);
    //         test.test = true;
    //     }
    // }

    /// <summary>
    /// Creates a grid of nodes in the center of this components object postition.
    /// </summary>
    public void CreateGrid()
    {
        // Create grid container
        grid = new GridNode[width, height];

          // Get node radius
        float nodeRadius = nodeSize / 2;

        // Calculate left most corner of the grid
        bottomLeftCornerPos = (Vector2)transform.position - (Vector2.right * (width * nodeSize) / 2) - (Vector2.up * (height * nodeSize) / 2);
   
        // Create nodes with bottom left corner as starting position
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 nodePos = bottomLeftCornerPos + Vector2.right * (x * nodeSize + nodeRadius) + Vector2.up * (y * nodeSize + nodeRadius);
                grid[x, y] = new GridNode(nodePos, false, x, y, NodeState.Hidden);
            }
        }
    }

    /// <summary>
    /// Translate world position into grid position
    /// </summary>
    /// <param name="worldPos">Positon of object in world posiiton</param>
    /// <returns> Closest node </returns>
    public GridNode WorldPositionToGridNode(Vector3 worldPos)
    {
        Vector2 CornerPosition = new Vector2(worldPos.x, worldPos.y) - bottomLeftCornerPos;

        Vector2 gridPosition = new Vector2(Mathf.FloorToInt(CornerPosition.x / nodeSize), Mathf.FloorToInt(CornerPosition.y / nodeSize));

        gridPosition.x = Mathf.Clamp(gridPosition.x, 0, width - 1);
        gridPosition.y = Mathf.Clamp(gridPosition.y, 0, height - 1);

        return grid[(int)gridPosition.x, (int)gridPosition.y];

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

        for(int i = x - 1; i <= x + 1 && i < width; i++)
        {
            for(int j = y - 1; j <= y + 1; j++)
            {

                if(i >= width || i <0)
                    continue;

                if(j >= height || j <0)
                    continue;
             
                if(x == i && y == j)
                    continue;

                nodeList.Add(grid[i,j]);
            }
        }

        return nodeList;
    }

        /// <summary>
    /// Traverse GridNodes around a gridnode which has bomb free enighbours
    /// </summary>
    /// <param name="node"> Node to traverse around </param>
    public void RecursiveTraverse(GridNode currentNode, Dictionary<Vector2, GameObject> tileDictonary)
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
                    RecursiveTraverse(n, tileDictonary);
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


    void OnDrawGizmos(){
        // Draw cube for grid borders
        Gizmos.DrawWireCube(transform.position, new Vector3(width*nodeSize, height* nodeSize, 0));

        if (grid != null){
            if (displayGridGizmos) {
                // Draw each individual node
                foreach (GridNode n in grid)
                {
                    Gizmos.color = (n.hasBomb) ? Color.red : defaultNodeColour;

                }
            }
        }
    }
}
