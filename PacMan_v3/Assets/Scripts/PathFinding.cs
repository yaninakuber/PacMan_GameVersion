using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{   
    List<Node> openList = new List<Node>(); // Lista de nodos abiertos para explorar
    List<Node> closedList = new List<Node>(); // Lista de nodos cerrados (explorados y considerados)

    int MovementCost = 10; // valor del costo de movimiento

    GridController grid; // referencia al controllador de la cuadricula

    private void Awake()
    {
        grid = GetComponent<GridController>(); //busca el componente grid controlle en el mismo objeto del script y almacena esa referencia en la variable grid 
    }

    private void Update()
    {
        FindPath();
    }


    void FindPath()
    {
        Node startNode = grid.NodeRequest(grid.start.transform.position); // current in grid
        Node goalNode = grid.NodeRequest(grid.goal.transform.position); // pacmans position in grid
        
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
            foreach (Node neighbor in grid.GetNeighborNodes(currentNode)) //sino se encontro el objetivo se procede a explorar a los vecinos del current node //por cada vecino: 
            {
                if(!neighbor.IsWalkable || closedList.Contains(neighbor)) // se verifica que sea transitable y q no este en la closed list 
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
 
        void CalculatePath (Node startNode, Node endNode) //  rastrear la ruta desde el nodo de destino (endNode) hasta el nodo de inicio (startNode) siguiendo los enlaces de nodos padres (ParentNode) y almacenando esta ruta en una lista llamada path
        {
            List<Node> path = new List<Node>(); // almacenar los nodos que forman la ruta desde el nodo de destino hasta el nodo de inicio.
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
    }



}
