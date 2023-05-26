using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Node
{
    private List<Node> myChildrens;

    //Hijos del nodo
    public List<Node> getChildrens() {
        return myChildrens;
    }

    //Como son celdas rectangulares necesitamos las coordenadas de las 4 esquinas que forman la celda
    public Vector2Int BottomLeftCorner { get; set; }
    public Vector2Int BottomRightCorner { get; set; }
    public Vector2Int TopLeftCorner { get; set; }
    public Vector2Int TopRightCorner { get; set; }

    public bool up { get; set; }
    public bool down { get; set; }
    public bool left { get; set; }
    public bool right { get; set; }

    public bool isCell { get; set; }

    //Para saber la celda con la que me conecto
    public Node conectedWith{ get; set; }

    public Vector2Int getCenter()
    {
        return (BottomLeftCorner + TopRightCorner) / 2;
    }

    //Nodo padre del nodo
    public Node myParent { get; set; }

    //Posicion en el arbol de nodos
    public int LayerIndex { get; set; }

    //Cuando creamos un nuevo nodo tenemos que pasarle el padre
    public Node(Node parent) {

        //Creamos la lista para los posibles hijos del nodo
        myChildrens = new List<Node>();
        //Asignamos el padre
        myParent = parent;

        if (parent != null)
        {
            //Si hay padre entonces añadimos al nodo como hijo del padre
            parent.addNewChild(this);
        }
    }

    //Metodos para añadir y quitar hijos
    public void addNewChild(Node child) {
        myChildrens.Add(child);
    }
}