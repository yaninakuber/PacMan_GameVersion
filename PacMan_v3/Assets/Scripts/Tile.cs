using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile 
{
    //Algoritmo A* (A star)
    public int GCost; //representa el costo actual del camino utilizado desde algún punto de origen hasta este tile en particular.
    public int HCost; // epresenta el costo estimado total para llegar desde este tile hasta un destino (o "goal").

    public int FCost // propiedad calcula el costo total para llegar desde el punto de origen hasta este tile.
    {
        get { return GCost + HCost; } // Esta propiedad se implementa como un getter, por lo que calcula su valor automáticamente cuando se accede a ella.
    }

    public Tile ParentTile; // es una referencia a otro objeto Tile que representa el tile desde el cual se llegó a este tile en el camino más corto encontrado hasta el momento.

    public int PositionX; // Estos campos almacenan las coordenadas (posición) del tile en el plano en los ejes X y Z 
    public int PositionZ;
    public int State; // 0 = Tile Free, 1 = Obstacule, 2 = Start Position, 3 = goal, 4 = PacMan

    public bool IsWalkable;

    public Tile(int _positionX, int _positionZ, int _state, bool _isWalkable) // constructor
    {
        PositionX = _positionX;
        PositionZ = _positionZ;
        State = _state;
        IsWalkable = _isWalkable;
    }

}
