using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is a component to create a grid of gridnodes with each ndoe being interacted based on mouse position
/// </summary>
public class Grid : MonoBehaviour
{
    public int width;
    public int height;
    public float nodeSize;

    public bool displayGridGizmos;
    private Vector3 gizmoNodeSize = Vector3.one * 0.9f;
    private Color defaultNodeColour = Color.green;

    private Vector2 bottomLeftCornerPos;
    public GridNode[,] grid;

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
