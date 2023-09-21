using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GameObject LeftTile;
    public GameObject RightTile;

    Tile[,] myGrid;

    public List<Tile> path;

    public LayerMask UnWalkable;

    //GRID INFO
    int xStart;
    int zStart;

    int xEnd;
    int zEnd;

    int quantityVerticalCell;
    int quantityHorizontalCell;

    int cellWidth = 1;
    int cellHeight = 1;

    private void Awake()
    {
        CreateGrid();
    }


    void CreateGrid ()
    {
        xStart = (int)LeftTile.transform.position.x;
        zStart = (int)LeftTile.transform.position.z;

        xEnd = (int)RightTile.transform.position.x;
        zEnd = (int)RightTile.transform.position.z;

        quantityHorizontalCell = (int)(xEnd - xStart / cellWidth);
        quantityVerticalCell = (int)(zEnd - zStart / cellHeight);

        myGrid = new Tile[quantityHorizontalCell, quantityVerticalCell];

        UpdateGrid();
    }   

    public void UpdateGrid()
    {
        for (int i = 0; i <= quantityHorizontalCell; i++)
        {
            for(int j = 0; j <= quantityVerticalCell; j++)
            {
                bool walkable = !(Physics.CheckSphere(new Vector3(xStart + i, 0, zStart + j), 0.4f, UnWalkable));

                myGrid[i, j] = new Tile(i, j, 0, walkable); // 0= free
            }
        }
    }


}
