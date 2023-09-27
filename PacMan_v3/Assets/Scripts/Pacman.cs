using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public float speed = 5f;

    Vector3 up = Vector3.zero;
    Vector3 right = new Vector3(0, 90, 0);
    Vector3 down = new Vector3(0, 180, 0);
    Vector3 left = new Vector3(0, 270, 0);
    Vector3 currentDirection = Vector3.zero;

    //calcular desde que punto a q punto de la grilla podemos morvernos 
    Vector3 nextPosition;
    Vector3 destination;
    Vector3 direction;

    bool canMove;
    public LayerMask unWalkable;

    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        currentDirection = up;
        nextPosition = Vector3.forward;
        destination = transform.position;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            nextPosition = Vector3.forward;
            currentDirection = up;
        } 
        else if (Input.GetKeyDown(KeyCode.S))
        {
            nextPosition = Vector3.back;
            currentDirection = down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            nextPosition = Vector3.left;
            currentDirection = left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            nextPosition = Vector3.right;
            currentDirection = right;
        }

        if(Vector3.Distance(destination, transform.position) < 0.00001f)
        {
            transform.localEulerAngles = currentDirection; // asegura que el jugador este correctamente orientado en la direccion en la que se esta moviendo. 
            //if (canMove)
            {
                if (ValidMove())
                {
                    destination = transform.position + nextPosition;
                    direction = nextPosition;
                    //canMove = true;
                }
            }
        }
    }

    bool ValidMove() //la direccion hacia adelante es valida o no
    {
        Ray myRay = new Ray(transform.position + new Vector3(0,0.25f,0), transform.forward); //Dibujo un rayo
        RaycastHit myHit; // se utilizara para almacenar info. sobre el resultado de la colision del rayo con otro objeto del juego

        if (Physics.Raycast(myRay, out myHit, 1f, unWalkable)) // out myhit parametro de salida que almacenara info. sobre el punto de colision si es que la hay 
        {
            if(myHit.collider.tag == "Wall")
            {
                return false; //movimiento no valido
            }
        }
        return true; //movimiento valido. 

    }

}
