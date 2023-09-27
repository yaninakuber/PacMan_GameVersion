using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NodeState
{
    Free, // Node Libre
    Obstacle, 
    Start, // Posicion al inicio
    Goal, // Objetivo
    PacMan 
}


public class Node 
{
    //Algoritmo A* (A star)

    // Costos utilizados en el algoritmo A*
    public int GCost; // Costo acumulado para llegar a un nodo desde el nodo de inicio.
    public int HCost; // Costo estimado total desde este tile hasta un destino (objetivo).

    public int FCost // propiedad calcula el costo total para llegar desde el punto de origen hasta este tile.
    {
        get { return GCost + HCost; } 
    }

    public Node ParentNode; // Referencia al tile padre que llevó al camino más corto hasta este tile.

    public int PositionX; // Coordenadas de posición del tile en el plano en los ejes X y Z.
    public int PositionZ;
    
    public NodeState State;

    public bool IsWalkable;

    public Node(int _positionX, int _positionZ, NodeState _state, bool _isWalkable) // constructor
    {
        PositionX = _positionX;
        PositionZ = _positionZ;
        State = _state;
        IsWalkable = _isWalkable;
    }

}
