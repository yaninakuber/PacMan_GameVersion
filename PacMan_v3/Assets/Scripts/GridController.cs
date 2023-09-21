using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GameObject StartTile;
    public GameObject EndTile;

    Tile[,] gridTiles;

    public List<Tile> path;

    public LayerMask UnWalkable;

    //GRID INFO
    int xStart;
    int zStart;

    int xEnd;
    int zEnd;

    int horizontalCellCount;
    int verticalCellCount;

    int cellWidth = 1;
    int cellHeight = 1;

    private void Awake()
    {
        CreateGrid();
    }


    void CreateGrid ()
    {
        xStart = (int)StartTile.transform.position.x;
        zStart = (int)StartTile.transform.position.z;

        xEnd = (int)EndTile.transform.position.x;
        zEnd = (int)EndTile.transform.position.z;

        verticalCellCount = (int)(((xEnd - xStart)+1) / cellWidth);
        horizontalCellCount = (int)(((zEnd - zStart)+1) / cellHeight);

        gridTiles = new Tile[verticalCellCount, horizontalCellCount];

        UpdateGrid();
    }   

    public void UpdateGrid()
    {
        for (int i = 0; i < verticalCellCount; i++)
        {
            for(int j = 0; j < horizontalCellCount; j++)
            {
                bool walkable = !(Physics.CheckSphere(new Vector3(xStart + i, 0, zStart + j), 0.4f, UnWalkable));

                gridTiles[i, j] = new Tile(i, j, 0, walkable); // 0= free
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (gridTiles != null)
        {
            foreach (Tile tile in gridTiles)
            {
                Gizmos.color = (tile.IsWalkable) ? Color.white : Color.red;

                Gizmos.DrawWireCube(new Vector3(xStart + tile.PositionX, 0.75f, zStart + tile.PositionZ), new Vector3 (0.8f, 1.5f, 0.8f));
            }
        }
    }

}
