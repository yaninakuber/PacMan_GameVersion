using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileState
{
    Free, // Tile Libre
    Obstacle, 
    Start, // Posicion al inicio
    Goal, // Objetivo
    PacMan 
}


public class Tile 
{
    //Algoritmo A* (A star)

    // Costos utilizados en el algoritmo A*
    public int GCost; // Costo actual del camino desde un punto de inicio hasta este tile.
    public int HCost; // Costo estimado total desde este tile hasta un destino (objetivo).

    public int FCost // propiedad calcula el costo total para llegar desde el punto de origen hasta este tile.
    {
        get { return GCost + HCost; } 
    }

    public Tile ParentTile; // Referencia al tile padre que llevó al camino más corto hasta este tile.

    public int PositionX; // Coordenadas de posición del tile en el plano en los ejes X y Z.
    public int PositionZ;
    
    public TileState State;

    public bool IsWalkable;

    public Tile(int _positionX, int _positionZ, TileState _state, bool _isWalkable) // constructor
    {
        PositionX = _positionX;
        PositionZ = _positionZ;
        State = _state;
        IsWalkable = _isWalkable;
    }

}
