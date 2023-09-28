using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    //PATHFINDING//
    List<Node> path = new List<Node>(); // almacenar los nodos que forman la ruta desde el nodo de destino hasta el nodo de inicio.
    int MovementCost = 10; //Heuristic Distance - Cost per step
    Node lastVisitedNode;
    public GridController grid; // referencia al controllador de la cuadricula

    //TARGET//
    public Transform CurrentTarget; 
    
    public Transform PacManTarget;
    public Transform HomeTarget;
    public Transform FrightedTarget;
    public Transform ScatterTarget;
    public Transform LeavingHomeTarget; //<<<<<


    //MOVEMENT//
    public float SpeedGhost = 3f;
    Vector3 nextPosition;
    Vector3 destination;

    //DIRECTION//
    Vector3 up = Vector3.zero,
    right = new Vector3(0, 90, 0),
    down = new Vector3(0, 180, 0),
    left = new Vector3(0, 270, 0),
    currentDirection = Vector3.zero;

    //STATEMACHINE//
    public enum GhostStates
    {
        Home,
        LeavingHome, //salio de casa
        Chase, //perseguir
        Scatter,
        Frightend, //pastilla de poder
        GotEaten, //fue comido, ojos
    }

    public GhostStates state;

    //APPEARENCE
    int activeAppearance; // 0 = NORMAL, 1 = FRIGHTENED, 2 = EYES ONLY 
    public GameObject[] appearance;




    private void Start()
    {
        destination = transform.position;
    }


    private void Update()
    {
        CheckState();
    }


    void FindPath()
    {
        Node startNode = grid.NodeRequest(this.transform.position); // current in grid
        Node goalNode = grid.NodeRequest(CurrentTarget.position); // pacmans position in grid

        List<Node> openList = new List<Node>(); // Lista de nodos abiertos para explorar (se colocan aqui para que las calcule en cada update) (siempre se crea una nueva lista abierta y cerrada)
        List<Node> closedList = new List<Node>(); // Lista de nodos cerrados (explorados y considerados)

        //add start Node
        openList.Add(startNode);

        // Start search looping
        while (openList.Count > 0)
        {
            Node currentNode = openList[0];//seleccion del nodo actual
            for (int i = 1; i < openList.Count; i++) //recorre la open list
            {
                if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost) // busca el F mas bajo 
                {
                    currentNode = openList[1]; //el nodo con el f mas bajo se convierte en el current Node 
                }
            }
            openList.Remove(currentNode); //Una vez seleccionado el current Node se elimina de la lista Open List 
            closedList.Add(currentNode); // y se agrega a la closed list

            //goal has been found?
            if (currentNode == goalNode)  //si el current es igual al destino ya se encontro el camino
            {
                // get path before we exit 
                CalculatePath(startNode, goalNode);
                return;
            }

            //CHECK ALL NEIGHBOR NODES - EXCEPT BACKWARDS//
            foreach (Node neighbor in grid.GetNeighborNodes(currentNode)) //sino se encontro el objetivo se procede a explorar a los vecinos del current node //por cada vecino: 
            {
                if (!neighbor.IsWalkable || closedList.Contains(neighbor) || neighbor == lastVisitedNode) // se verifica que sea transitable y q no este en la closed list 
                {
                    continue;
                }
                int calcMoveCost = currentNode.GCost + GetDistance(currentNode, neighbor); // se calcula el costo de movimiento desde el current node hasta el vecino sumando el costo acumulado GCost del currentNode al costo de distancia al vecino (GetDistance).

                if (calcMoveCost < neighbor.GCost || !openList.Contains(neighbor)) // Si el calcMoveCost es menor que el GCost actual del vecino o si el vecino no está en la lista openList, se actualizan los valores GCost, HCost, y ParentNode del vecino. Esto significa que se ha encontrado un camino más corto hacia este vecino.
                {
                    neighbor.GCost = calcMoveCost;
                    neighbor.HCost = GetDistance(neighbor, goalNode);

                    neighbor.ParentNode = currentNode;
                    if (!openList.Contains(neighbor)) // Si el vecino no está en la lista openList, se agrega a la lista para su consideración en futuras iteraciones.
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
    }
 
    void CalculatePath (Node startNode, Node goalNode) //  rastrear la ruta desde el nodo de destino (endNode) hasta el nodo de inicio (startNode) siguiendo los enlaces de nodos padres (ParentNode) y almacenando esta ruta en una lista llamada path
    {
        lastVisitedNode = startNode; //coloco el start node en ya visitado 
        path.Clear(); // me aseguro que la lista este vacia antes de calcular una nueva ruta 

        Node currentNode = goalNode; // porque comenzamos desde el nodo de destino y retrocedemos hacia el nodo de inicio.

        while (currentNode != startNode) //  recorrer la ruta desde el nodo de destino hasta el nodo de inicio. El bucle continúa hasta que currentNode sea igual al nodo de inicio (startNode).
        {
            path.Add(currentNode);
            currentNode = currentNode.ParentNode; // En cada iteración del bucle, se agrega el nodo actual a la lista path y luego se actualiza currentNode con el nodo padre, lo que efectivamente retrocede a través de la ruta hacia el nodo de inicio.
        }
        //reverse path to get is sorted right
        path.Reverse(); // Para obtener la ruta en el orden correcto, se invierte la lista utilizando el método Reverse()
        grid.path = path; // se almacena la ruta 

    }

    int GetDistance(Node a, Node b) // estimación de la distancia entre dos nodos en una cuadrícula //  esta distancia se utiliza para ayudar a determinar la prioridad de exploración de los nodos vecinos
    {
        // // Calcula la diferencia en las coordenadas X e Z entre los nodos a y b.
        int distanceX = Mathf.Abs(a.PositionX - b.PositionX);
        int distanceZ = Mathf.Abs(a.PositionZ - b.PositionZ);

        int totalDistance = distanceX + distanceZ;

        // Multiplica la distancia total por un valor de costo de movimiento (MovementCost).
        return MovementCost * totalDistance;
    }

    void MoveGhost()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, SpeedGhost * Time.deltaTime);
        if (Vector3.Distance(transform.position, destination) < 0.0001f)
        {
            FindPath();

            if (path.Count > 0)
            {
                //DESTINATION//
                nextPosition = grid.NextPathPoint(path[0]);
                destination = nextPosition;

                //ROTATION//
                SetDirection(); //actualizo rotacion
                transform.localEulerAngles = currentDirection; //transformo el angulo local 
            }
        }
    }

    void SetDirection ()
    {
        int directionX = (int)(nextPosition.x - transform.position.x);
        int directionZ = (int)(nextPosition.z - transform.position.z);

        if (directionX == 0 && directionZ > 0)
        {
            currentDirection = up;
        }
        else if (directionX == 0 && directionZ < 0) 
        {
            currentDirection = down;
        }
        else if (directionX > 0 && directionZ == 0)
        {
            currentDirection = right;
        }
        else if (directionX < 0 && directionZ == 0)
        {
            currentDirection = left;
        }

    }
    
    void CheckState()
    {
        switch (state)
        {
            case GhostStates.Home:
                activeAppearance = 0;
                SetAppearance();
                break;
            case GhostStates.LeavingHome:
                activeAppearance = 0;
                SetAppearance();
                break;
            //CHASE PACMAN
            case GhostStates.Chase:
                activeAppearance = 0;
                SetAppearance();
                CurrentTarget = PacManTarget;
                MoveGhost();
                break;
            case GhostStates.Scatter:
                activeAppearance = 0;
                SetAppearance();
                break;
            case GhostStates.Frightend:
                activeAppearance = 1;
                SetAppearance();
                CurrentTarget = FrightedTarget;
                MoveGhost();
                break;
            case GhostStates.GotEaten:
                activeAppearance = 2;
                SetAppearance();
                break;
        }
    }

    void SetAppearance()
    {
        for (int i = 0; i < appearance.Length; i++)
        {
            appearance[i].SetActive(i == activeAppearance); // i == ... devuelve un bool.
        }
    }

}
