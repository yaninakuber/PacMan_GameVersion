using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GameObject StartPointOfGrill;
    public GameObject EndPointOfGrill;
    
    Node[,] gridTiles; // representa la grilla. Cada elemento es un nodo que contiene informacion de la clase

    public List<Node> ClydePath; // Lista de nodos que representan el camino calculado por el algoritmo
    public List<Node> BlinkyPath;
    public List<Node> InkyPath;
    public List<Node> PinkyPath;



    public LayerMask UnWalkable; //cuales nodos deben ser obtaculos y cuales no

    //GRID INFO - trabajar como propiedad
    int xStart;
    int zStart;

    int xEnd;
    int zEnd;

    int verticalCellCount;
    int horizontalCellCount;

    int cellWidth = 1;
    int cellHeight = 1;

    private void Awake()
    {
        CreateGrid();
    }


    void CreateGrid ()
    {
        // obtiene coordenadas del punto de inicio y fin de la cuadricula
        xStart = (int)StartPointOfGrill.transform.position.x; 
        zStart = (int)StartPointOfGrill.transform.position.z;

        xEnd = (int)EndPointOfGrill.transform.position.x;
        zEnd = (int)EndPointOfGrill.transform.position.z;

        //calcula el numero de celdas en la grilla en los dos ejes
        horizontalCellCount = (int)(((xEnd - xStart)+1) / cellWidth);
        verticalCellCount = (int)(((zEnd - zStart)+1) / cellHeight);

        gridTiles = new Node[horizontalCellCount, verticalCellCount]; // matriz que representa la cuadricula del juego - especifico la extension de la cuadricula 

        UpdateGrid(); // llena la matriz gridTiles con nodos determinando si cada celda es transitable o no
    }   

    public void UpdateGrid()
    {
        for (int i = 0; i < horizontalCellCount; i++)
        {
            for(int j = 0; j < verticalCellCount; j++)
            {
                bool walkable = !(Physics.CheckSphere(new Vector3(xStart + i, 0, zStart + j), 0.4f, UnWalkable)); // verifica si la celda es transitable : true

                gridTiles[i, j] = new Node(i, j, NodeState.Free, walkable); //crea un nuevo nodo y lo asigna a la matriz gridTiles // llamo al constructor
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (gridTiles != null)
        {
            foreach (Node node in gridTiles)
            {
                Gizmos.color = (node.IsWalkable) ? Color.white : Color.black;

                if (ClydePath != null)
                {
                    if (ClydePath.Contains(node))
                    {
                        Gizmos.color = Color.yellow;
                    }
                }
                if (InkyPath != null)
                {
                    if (InkyPath.Contains(node))
                    {
                        Gizmos.color = Color.cyan;
                    }
                }
                if (PinkyPath != null)
                {
                    if (PinkyPath.Contains(node))
                    {
                        Gizmos.color = Color.magenta;
                    }
                }
                if (BlinkyPath != null)
                {
                    if (BlinkyPath.Contains(node))
                    {
                        Gizmos.color = Color.red;
                    }
                }

                Vector3 nodePosition = new Vector3(xStart + node.PositionX, 0.75f, zStart + node.PositionZ);
                Gizmos.DrawWireCube(nodePosition, new Vector3(0.8f, 1.5f, 0.8f));
            }
        }
    }

    public Node NodeRequest(Vector3 position) //Mapear un posición a un nodo en la grilla
    {
        // Calcula la distancia desde la posición a lo largo de los ejes X y Z
        int gridX = (int)Vector3.Distance(new Vector3(position.x, 0, 0), new Vector3(xStart, 0, 0)); // Esto da una idea de cuán lejos está la posición en el eje X desde el inicio de la cuadrícula. 
        int gridZ = (int)Vector3.Distance(new Vector3(0,0, position.z), new Vector3(0, 0, zStart));

        // Devuelve el nodo correspondiente en la cuadrícula de la posicion especificada
        return gridTiles[gridX, gridZ];
    }

    public Vector3 NextPathPoint(Node node) //siguiente punto de paso
    {
        int gridX = (int)(xStart+node.PositionX);
        int gridZ = (int)(zStart+node.PositionZ);

        return new Vector3(gridX,0, gridZ);
    }



    public List<Node> GetNeighborNodes (Node node) 
    {
        List<Node> neighbours = new List<Node>(); //representa todos los Nodos vecinos del Nodo actual
        //find all neighbors in a 3x3 scuare around current node
        for (int x = -1; x <= 1 ; x++)
        {
            for (int z = -1; z <= 1 ; z++) 
            { 
                //ignora nodos diagonales por que no se analizan recorridos diagonales
                //ignore following fields 
                if(x == 0 && z == 0)
                {
                    continue;
                }
                //ignore top left
                if(x == -1 && z == 1)
                {
                    continue;
                }
                //ignore top right
                if(x == 1 & z == 1)
                {
                    continue;
                }
                //ignore bottom left
                if (x == 1 & z == -1)
                {
                    continue;
                }
                //ignore bottom right
                if (x == -1 & z == -1)
                {
                    continue;
                }

                // Calcular las coordenadas de la posición a verificar
                int checkPositionX = node.PositionX + x;
                int checkPositionZ = node.PositionZ + z;

                // Verificar si las coordenadas están dentro de los límites de la cuadrícula // Esto se hace para asegurarse de que no se acceda a posiciones fuera de la cuadrícula, lo que podría causar errores.
                if (checkPositionX >= 0 && checkPositionX < horizontalCellCount && checkPositionZ >= 0 && checkPositionZ < verticalCellCount)
                {
                   neighbours.Add(gridTiles[checkPositionX,checkPositionZ]);
                }        
            }
        }
        return neighbours; // la función devuelve la lista neighbours, que contiene todos los nodos vecinos válidos al nodo actual.
    }

}
