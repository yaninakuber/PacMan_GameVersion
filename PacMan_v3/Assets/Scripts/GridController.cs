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

    public Node NodeRequest(Vector3 position)
    {
        // Calcula la distancia desde la posici�n a lo largo de los ejes X y Z
        int gridX = (int)Vector3.Distance(new Vector3(position.x, 0, 0), new Vector3(xStart, 0, 0));
        int gridZ = (int)Vector3.Distance(new Vector3(0, 0, position.z), new Vector3(0, 0, zStart));

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

                // Calcular las coordenadas de la posici�n a verificar
                int checkPositionX = node.PositionX + x;
                int checkPositionZ = node.PositionZ + z;

                // Verificar si las coordenadas est�n dentro de los l�mites de la cuadr�cula // Esto se hace para asegurarse de que no se acceda a posiciones fuera de la cuadr�cula, lo que podr�a causar errores.
                if (checkPositionX >= 0 && checkPositionX < horizontalCellCount && checkPositionZ >= 0 && checkPositionZ < verticalCellCount)
                {
                   neighbours.Add(gridTiles[checkPositionX,checkPositionZ]);
                }        
            }
        }
        return neighbours; // la funci�n devuelve la lista neighbours, que contiene todos los nodos vecinos v�lidos al nodo actual.
    }

    public bool CheckInsideGrid(Vector3 requestedPosition)
    {
        int gridX = (int)(requestedPosition.x - xStart);
        int gridZ = (int)(requestedPosition.z - zStart);

        if(gridX >= horizontalCellCount)
        {
            return false;
        }
        else if (gridX < 0)
        {
            return false;
        }
        else if (gridZ >= verticalCellCount)
        {
            return false;
        }
        else if (gridZ < 0)
        {
            return false;
        }

        if (!NodeRequest(requestedPosition).IsWalkable)
        {
            return false;
        }
        return true;

    }

    public Vector3 GetNearestNonWallNode(Vector3 target) //encontrar el nodo mas cercano al target que no sea una pared
    {
        float minDistance = 1000; //me aseguro que cualquier distancia encontrada sea menor en la primera iteracion
        int minIndexI = 0;
        int minIndexJ = 0;

        for (int i = 0; i < horizontalCellCount; i++) //recorro toda la cuadricula para ver donde hay un wall
        {
            for (int j = 0; j < verticalCellCount; j++)
            {
                if (gridTiles[i,j].IsWalkable) //verifico que la celda correspondiente sea transitable
                {
                    Vector3 nextPoint = NextPathPoint(gridTiles[i,j]); 
                    float distance = Vector3.Distance(nextPoint, target); //medimos la distancia entre el siguiente punto y el target para buscar el mas corto o mas corto que el minimo. 
                    if (distance < minDistance) //si la distancia calculada es menor que la min distancia se actualiza 
                    { 
                        minDistance = distance; 
                        minIndexI = i; //rastreamos la posicion de la celda mas cercana rastreada hasta el momento
                        minIndexJ = j; //almacenamos aca por que despues de recorrer no puedo acceder a i o a j
                    }
                }
            }
        }
        return NextPathPoint(gridTiles[minIndexI,minIndexJ]);
    }

}
