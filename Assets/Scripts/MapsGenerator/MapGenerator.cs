using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using Random = UnityEngine.Random;

public enum Orientation
{
    Horizontal = 0, Vertical = 1
}

public class MapGenerator
{
    CellNode rootNode;
    List<CellNode> allNodes = new List<CellNode>();
    List<Node> onlyCells;

    private int mapWidth;
    private int mapLength;
    private int corridorWidth;
    private int offsetNearCenter = 5;

    private float bottomLeftCornerFactor;
    private float topRightCornerFactor;

    private int cellsOffset;

    public MapGenerator(int mapWidth_, int mapLength_, float bottomLeftCornerFactor_, float topRightCornerFactor_, int cellsOffset_, int corridorWidth_)
    {
        mapWidth = mapWidth_;
        mapLength = mapLength_;
        corridorWidth= corridorWidth_;
        bottomLeftCornerFactor= bottomLeftCornerFactor_;
        topRightCornerFactor = topRightCornerFactor_;
        cellsOffset = cellsOffset_;

        //Creamos el nodo raiz con tamaño maximo y con layaerIndex = 0
        rootNode = new CellNode(new Vector2Int(0, 0), new Vector2Int(mapWidth, mapLength), null, 0);
    }

    public MapGenerator() { }

    //Metodo que devuelve la lista de celdas generadas
    public List<Node> GetListCells(int divisions, int cellWidthMin, int cellLengthMin)
    {

        //Creamos una cola para recorrer todos los nodos 
        Queue<CellNode> graph = new Queue<CellNode>();
        allNodes.Add(rootNode);
        graph.Enqueue(rootNode);

        int i = 0;

        //Hacemos la divisiones necesarias
        while (i < divisions)
        {
            i++;
            //Sacamos el nodo primero que este en la cola
            CellNode currentNode = graph.Dequeue();

            //Si la celda del nodo cumple alguna de las restricciones de tamaño
            bool constrainWidth = currentNode.Width() >= cellWidthMin * 2;
            bool constrainLength = currentNode.Length() >= cellLengthMin * 2;

            if ( constrainWidth || constrainLength)
            {
                //Dividimos la celda
                DivideCell(constrainWidth, constrainLength, currentNode, cellWidthMin, cellLengthMin, graph);
            }

        }

        //Lista solo con los nodos que no tienen hijos ya que son los que realmente representan la forma final de las celdas
        List<Node> finaCells = extractNodesWithOutChildren(rootNode);
        onlyCells = new List<Node>(finaCells);

        //Devolveremos la lista con los pasillos incluidos
        return PutCorridorsInFinalCells(finaCells);

    }

    public List<Node> GetOnlyCells()
    {
        return onlyCells;
    }

    private List<Node> PutCorridorsInFinalCells(List<Node> finalCells)
    {

        List<Node> auxFinalCells = new List<Node>(finalCells);
        List<Node> cellsUnVisited = new List<Node>();

        //Partimos de una celda aleatoria de entre todas las finales
        Node currentCell = auxFinalCells[Random.Range(0, auxFinalCells.Count)];
        auxFinalCells.Remove(currentCell);

        Node lastCurrent = null;

        while (auxFinalCells.Count > 0)
        {
            //Buscamos la celda más cercana a la actual para crear el pasillo
            Node nearest = getNearestCell(currentCell, auxFinalCells);
            auxFinalCells.Remove(nearest);

            if(lastCurrent != null) Debug.Log("Actual: " + currentCell.BottomLeftCorner + " Cercana: " + nearest.BottomLeftCorner + " Ultima: " + lastCurrent.BottomLeftCorner);

            if (lastCurrent == null || nearestValid(currentCell, lastCurrent, nearest))
            {
                //Cada celda tres posibles pasillos explicados dentro del metodo
                List<CellNode> corridors = GenerateNewCorridor(currentCell, nearest);
                lastCurrent = currentCell;
                currentCell = nearest;

                foreach (CellNode corridor in corridors)
                {
                    //Si existe pasillo
                    if (corridor != null)
                    {
                        //Agregamos el pasillo
                        finalCells.Add(corridor);
                    }
                }
            }

            else if (lastCurrent != null)
            {
                cellsUnVisited.Add(nearest);
            }
        }

        //Para asegurarnos que no queden celdas aisladas hacemos la segunda pasada
        if (cellsUnVisited.Count > 0)
        {
            auxFinalCells = new List<Node>(finalCells);

            foreach (Node cell in cellsUnVisited)
            {
                auxFinalCells.Remove(cell);

                //Buscamos la celda más cercana a la actual para crear el pasillo
                Node nearest = getNearestCell(cell, auxFinalCells);

                //Cada celda tres posibles pasillos explicados dentro del metodo
                List<CellNode> corridors = GenerateNewCorridor(cell, nearest);

                foreach (CellNode corridor in corridors)
                {
                    //Si existe pasillo
                    if (corridor != null)
                    {
                        //Agregamos el pasillo
                        finalCells.Add(corridor);
                    }
                }
            }
        }

        return finalCells;
    }

    private bool nearestValid(Node currentCell, Node lastCurrent, Node nearest)
    {
        //Como vamos eliminando celdas a medida que añadimos pasillos puede pasar que la celda más cercana a la actual este muy lejos
        //y por tanto que ponga pasillos atravesando a otra celda

        bool valid = true;

        if (lastCurrent != null)
        {
            //Si la anterior currentCell estaba a la derecha
            if (currentCell.getCenter().x < lastCurrent.getCenter().x && 
                (currentCell.getCenter().y + offsetNearCenter >= lastCurrent.getCenter().y && currentCell.getCenter().y - offsetNearCenter <= lastCurrent.getCenter().y) )
            {
                //Si la nueva más cercana está más a la derecha que la anterior entones los pasillos atravesarían a lasCurrent
                if(nearest.getCenter().x > lastCurrent.getCenter().x)valid = false;
            }

            //Si la anterior currentCell estaba a la izquierda
            else if (currentCell.getCenter().x > lastCurrent.getCenter().x &&
                (currentCell.getCenter().y + offsetNearCenter >= lastCurrent.getCenter().y && currentCell.getCenter().y - offsetNearCenter <= lastCurrent.getCenter().y))
            {
                //Si la nueva más cercana está más a la izquierda que la anterior entones los pasillos atravesarían a lasCurrent
                if (nearest.getCenter().x < lastCurrent.getCenter().x) valid = false;
            }

            //Si la anterior currentCell estaba abajo
            else if (currentCell.getCenter().y > lastCurrent.getCenter().y &&
                (currentCell.getCenter().x + offsetNearCenter >= lastCurrent.getCenter().x && currentCell.getCenter().x - offsetNearCenter <= lastCurrent.getCenter().x))
            {
                //Si la nueva más cercana está más abajo  que la anterior entones los pasillos atravesarían a lasCurrent
                if (nearest.getCenter().y < lastCurrent.getCenter().y) valid = false;
            }

            //Si la anterior currentCell estaba arriba
            else if (currentCell.getCenter().y < lastCurrent.getCenter().y &&
                (currentCell.getCenter().x + offsetNearCenter >= lastCurrent.getCenter().x && currentCell.getCenter().x - offsetNearCenter <= lastCurrent.getCenter().x))
            {
                //Si la nueva más cercana está más arriba que la anterior entones los pasillos atravesarían a lasCurrent
                if (nearest.getCenter().y > lastCurrent.getCenter().y) valid = false;
            }

        }

        return valid;
    }

    private List<CellNode> GenerateNewCorridor(Node currentCell, Node nearest)
    {

        List<CellNode> result = new List<CellNode>();

        //El corridor1 corresponde con el que conecta las celdas que más o menos estén alineadas por sus puntos medios, lugar desde el cual partira este pasillo
        CellNode corridor1 = new CellNode(currentCell.BottomLeftCorner, currentCell.TopRightCorner, currentCell, currentCell.LayerIndex + 1);

        //Si las celdas están más o menos alineadas en el eje x entonces sabesmos que la más cercana está arriba o abajo
        if (nearest.getCenter().x <= currentCell.getCenter().x + offsetNearCenter && nearest.getCenter().x >= currentCell.getCenter().x - offsetNearCenter)
        {

            //Construimos puente hacia abajo
            if (currentCell.getCenter().y > nearest.getCenter().y)
            {
                corridor1.BottomLeftCorner = new Vector2Int(currentCell.getCenter().x, nearest.TopLeftCorner.y);
                corridor1.BottomRightCorner = new Vector2Int(currentCell.getCenter().x + corridorWidth / 2, nearest.TopLeftCorner.y);
                corridor1.TopLeftCorner = new Vector2Int(currentCell.getCenter().x, currentCell.BottomLeftCorner.y);
                corridor1.TopRightCorner = new Vector2Int(currentCell.getCenter().x + corridorWidth / 2, currentCell.BottomLeftCorner.y);

                //Marcamos que la casilla actual tiene conexion por abajo y la cercana por arriba
                currentCell.down = true;
                nearest.up = true;
            }

            //Construimos puente hacia arriba
            else if (currentCell.getCenter().y < nearest.getCenter().y)
            {
                corridor1.BottomLeftCorner = new Vector2Int(currentCell.getCenter().x, currentCell.TopLeftCorner.y);
                corridor1.BottomRightCorner = new Vector2Int(currentCell.getCenter().x + corridorWidth / 2, currentCell.TopLeftCorner.y);
                corridor1.TopLeftCorner = new Vector2Int(currentCell.getCenter().x, nearest.BottomLeftCorner.y);
                corridor1.TopRightCorner = new Vector2Int(currentCell.getCenter().x + corridorWidth / 2, nearest.BottomLeftCorner.y);

                //Marcamos que la casilla actual tiene conexion por arriba y la cercana por abajo
                currentCell.up = true;
                nearest.down = true;
            }

            corridor1.isCell = false;

        }

        //Si las celdas están más o menos alineadas en el eje y entonces sabesmos que la más cercana está a la derecha o izquierda
        else if (currentCell.getCenter().y + offsetNearCenter >= nearest.getCenter().y && nearest.getCenter().y >= currentCell.getCenter().y - offsetNearCenter)
        {
            //Construimos puente hacia la izquierda
            if (currentCell.getCenter().x > nearest.getCenter().x)
            {
                corridor1.BottomLeftCorner = new Vector2Int(nearest.BottomRightCorner.x, currentCell.getCenter().y);
                corridor1.BottomRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x, currentCell.getCenter().y);
                corridor1.TopLeftCorner = new Vector2Int(nearest.BottomRightCorner.x, currentCell.getCenter().y + corridorWidth / 2);
                corridor1.TopRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x, currentCell.getCenter().y + corridorWidth / 2);

                //Marcamos que la casilla actual tiene conexion por la izquierda y la cercana por la derecha
                currentCell.left = true;
                nearest.right = true;
            }

            //Construimos puente hacia ala derecha
            else if (currentCell.getCenter().x < nearest.getCenter().x)
            {
                corridor1.BottomLeftCorner = new Vector2Int(currentCell.BottomRightCorner.x, currentCell.getCenter().y);
                corridor1.BottomRightCorner = new Vector2Int(nearest.TopLeftCorner.x, currentCell.getCenter().y);
                corridor1.TopLeftCorner = new Vector2Int(currentCell.BottomRightCorner.x, currentCell.getCenter().y + corridorWidth / 2);
                corridor1.TopRightCorner = new Vector2Int(nearest.TopLeftCorner.x, currentCell.getCenter().y + corridorWidth / 2);

                //Marcamos que la casilla actual tiene conexion por la derecha y la cercana por la izquierda
                currentCell.right = true;
                nearest.left = true;
            }

            corridor1.isCell = false;

        }

        else
        {
            currentCell.getChildrens()[0] = null;
            corridor1 = null;
        }

        //Si hay conexion
        if(corridor1 != null)
        {
            currentCell.conectedWith = nearest;
        }

        result.Add(corridor1);

        //Los siguientes 2 pasillos se hacen con el fin de asegurar la conexion de todas las celdas, así como la diversidad de caminos entre estas

        //El corridor2 corresponde con el que conecta las celdas que más o menos estén alineadas por sus pivotes de abajo a la izquierda, lugar desde el cual partira este pasillo
        CellNode corridor2 = new CellNode(currentCell.BottomLeftCorner, currentCell.TopRightCorner, currentCell, currentCell.LayerIndex + 1);

        //Mismo prodecimiento en el if y el else if para saber donde está la celda nearest

        if (nearest.BottomLeftCorner.x <= currentCell.BottomLeftCorner.x + offsetNearCenter && nearest.BottomLeftCorner.x >= currentCell.BottomLeftCorner.x - offsetNearCenter)
        {
            //Construimos puente hacia abajo
            if (currentCell.BottomLeftCorner.y > nearest.BottomLeftCorner.y)
            {
                corridor2.BottomLeftCorner = new Vector2Int(currentCell.BottomLeftCorner.x, nearest.TopLeftCorner.y);
                corridor2.BottomRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x + corridorWidth / 2, nearest.TopLeftCorner.y);
                corridor2.TopLeftCorner = new Vector2Int(currentCell.BottomLeftCorner.x, currentCell.BottomLeftCorner.y);
                corridor2.TopRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x + corridorWidth / 2, currentCell.BottomLeftCorner.y);

                //Marcamos que la casilla actual tiene conexion por abajo y la cercana por arriba
                currentCell.down = true;
                nearest.up = true;
            }

            //Construimos puente hacia arriba
            else if (currentCell.BottomLeftCorner.y < nearest.BottomLeftCorner.y)
            {
                corridor2.BottomLeftCorner = new Vector2Int(currentCell.BottomLeftCorner.x, currentCell.TopLeftCorner.y);
                corridor2.BottomRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x + corridorWidth / 2, currentCell.TopLeftCorner.y);
                corridor2.TopLeftCorner = new Vector2Int(currentCell.BottomLeftCorner.x, nearest.BottomLeftCorner.y);
                corridor2.TopRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x + corridorWidth / 2, nearest.BottomLeftCorner.y);

                //Marcamos que la casilla actual tiene conexion por abajo y la cercana por arriba
                currentCell.up = true;
                nearest.down = true;
            }

            corridor2.isCell = false;


        }

        else if (nearest.BottomLeftCorner.y <= currentCell.BottomLeftCorner.y + offsetNearCenter && nearest.BottomLeftCorner.y >= currentCell.BottomLeftCorner.y - offsetNearCenter)
        {
            //Construimos puente hacia la izquierda
            if (currentCell.BottomLeftCorner.x > nearest.BottomLeftCorner.x)
            {
                corridor2.BottomLeftCorner = new Vector2Int(nearest.BottomRightCorner.x, currentCell.BottomLeftCorner.y);
                corridor2.BottomRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x, currentCell.BottomLeftCorner.y);
                corridor2.TopLeftCorner = new Vector2Int(nearest.BottomRightCorner.x, currentCell.BottomLeftCorner.y + corridorWidth / 2);
                corridor2.TopRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x, currentCell.BottomLeftCorner.y + corridorWidth / 2);

                //Marcamos que la casilla actual tiene conexion por la izquierda y la cercana por la derecha
                currentCell.left = true;
                nearest.right = true;
            }

            //Construimos puente hacia ala derecha
            else if (currentCell.BottomLeftCorner.x < nearest.BottomLeftCorner.x)
            {
                corridor2.BottomLeftCorner = new Vector2Int(currentCell.BottomRightCorner.x, currentCell.BottomLeftCorner.y);
                corridor2.BottomRightCorner = new Vector2Int(nearest.TopLeftCorner.x, currentCell.BottomLeftCorner.y);
                corridor2.TopLeftCorner = new Vector2Int(currentCell.BottomRightCorner.x, currentCell.BottomLeftCorner.y + corridorWidth / 2);
                corridor2.TopRightCorner = new Vector2Int(nearest.TopLeftCorner.x, currentCell.BottomLeftCorner.y + corridorWidth / 2);

                //Marcamos que la casilla actual tiene conexion por la izquierda y la cercana por la derecha
                currentCell.right = true;
                nearest.left = true;
            }

            corridor2.isCell = false;
        }

        else
        {
            currentCell.getChildrens()[1] = null;
            corridor2 = null; 
        }

        //Si hay conexion
        if (corridor2 != null)
        {
            currentCell.conectedWith = nearest;
        }

        result.Add(corridor2);

        //El corridor3 corresponde con el que conecta las celdas que más o menos estén alineadas por sus pivotes de arriba a la derecha, lugar desde el cual partira este pasillo
        CellNode corridor3 = new CellNode(currentCell.BottomLeftCorner, currentCell.TopRightCorner, currentCell, currentCell.LayerIndex + 1);

        //Mismo prodecimiento en el if y el else if para saber donde está la celda nearest

        if (nearest.TopRightCorner.x <= currentCell.TopRightCorner.x + offsetNearCenter && nearest.TopRightCorner.x >= currentCell.TopRightCorner.x - offsetNearCenter)
        {
            //Construimos puente hacia abajo
            if (currentCell.TopRightCorner.y > nearest.TopRightCorner.y)
            {
                corridor3.BottomLeftCorner = new Vector2Int(currentCell.TopRightCorner.x - corridorWidth / 2, nearest.TopLeftCorner.y);
                corridor3.BottomRightCorner = new Vector2Int(currentCell.TopRightCorner.x, nearest.TopLeftCorner.y);
                corridor3.TopLeftCorner = new Vector2Int(currentCell.TopRightCorner.x - corridorWidth / 2, currentCell.BottomLeftCorner.y);
                corridor3.TopRightCorner = new Vector2Int(currentCell.TopRightCorner.x, currentCell.BottomLeftCorner.y);

                //Marcamos que la casilla actual tiene conexion por abajo y la cercana por arriba
                currentCell.down = true;
                nearest.up = true;
            }

            //Construimos puente hacia arriba
            else if (currentCell.TopRightCorner.y < nearest.TopRightCorner.y)
            {
                corridor3.BottomLeftCorner = new Vector2Int(currentCell.TopRightCorner.x - corridorWidth / 2, currentCell.TopLeftCorner.y);
                corridor3.BottomRightCorner = new Vector2Int(currentCell.TopRightCorner.x, currentCell.TopLeftCorner.y);
                corridor3.TopLeftCorner = new Vector2Int(currentCell.TopRightCorner.x - corridorWidth / 2, nearest.BottomLeftCorner.y);
                corridor3.TopRightCorner = new Vector2Int(currentCell.TopRightCorner.x, nearest.BottomLeftCorner.y);

                //Marcamos que la casilla actual tiene conexion por abajo y la cercana por arriba
                currentCell.up = true;
                nearest.down = true;
            }

            corridor3.isCell = false;
        }

        else if (nearest.TopRightCorner.y <= currentCell.TopRightCorner.y + offsetNearCenter && nearest.TopRightCorner.y >= currentCell.TopRightCorner.y - offsetNearCenter)
        {
            //Construimos puente hacia la izquierda
            if (currentCell.TopRightCorner.x > nearest.TopRightCorner.x)
            {
                corridor3.BottomLeftCorner = new Vector2Int(nearest.BottomRightCorner.x, currentCell.TopRightCorner.y - corridorWidth / 2);
                corridor3.BottomRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x, currentCell.TopRightCorner.y - corridorWidth / 2);
                corridor3.TopLeftCorner = new Vector2Int(nearest.BottomRightCorner.x, currentCell.TopRightCorner.y );
                corridor3.TopRightCorner = new Vector2Int(currentCell.BottomLeftCorner.x, currentCell.TopRightCorner.y);

                //Marcamos que la casilla actual tiene conexion por la izquierda y la cercana por la derecha
                currentCell.left = true;
                nearest.right = true;
            }

            //Construimos puente hacia ala derecha
            else if (currentCell.TopRightCorner.x < nearest.TopRightCorner.x)
            {
                corridor3.BottomLeftCorner = new Vector2Int(currentCell.BottomRightCorner.x, currentCell.TopRightCorner.y - corridorWidth / 2);
                corridor3.BottomRightCorner = new Vector2Int(nearest.TopLeftCorner.x, currentCell.TopRightCorner.y - corridorWidth / 2);
                corridor3.TopLeftCorner = new Vector2Int(currentCell.BottomRightCorner.x, currentCell.TopRightCorner.y);
                corridor3.TopRightCorner = new Vector2Int(nearest.TopLeftCorner.x, currentCell.TopRightCorner.y);

                //Marcamos que la casilla actual tiene conexion por la izquierda y la cercana por la derecha
                currentCell.right = true;
                nearest.left = true;
            }

            corridor3.isCell = false;
        }

        else
        {
            currentCell.getChildrens()[2] = null;
            corridor3 = null;
        }

        result.Add(corridor3);

        //Si hay conexion
        if (corridor3 != null)
        {
            currentCell.conectedWith = nearest;
        }

        return result;
    }

    private Node getNearestCell(Node currentCell, List<Node> auxFinalCells)
    {

        Node nearest = null;
        float minDistance = float.MaxValue;

        foreach (Node cell in auxFinalCells)
        {

            float currentDistance = Vector2.Distance(currentCell.getCenter(), cell.getCenter());
            if (currentDistance <= minDistance)
            {
                minDistance = currentDistance;
                nearest = cell;
            }

        }

        return nearest;
    }

    private List<Node> extractNodesWithOutChildren(CellNode rootNode)
    {

        Queue<CellNode> auxiliarQueue = new Queue<CellNode>();
        List<Node> nodes = new List<Node>();

        //Si el padre no tiene hijos devolvemos el padre
        if(rootNode.getChildrens().Count == 0)
        {
            return new List<Node> { rootNode };
        }

        //Cada hijo del padre con encolamos en la cola
        foreach (CellNode childNode in rootNode.getChildrens())
        {
            auxiliarQueue.Enqueue(childNode);
        }

        //Mientras haya nodos en la cola
        while (auxiliarQueue.Count > 0)
        {

            //Sacamos el nodo
            CellNode currenNode = auxiliarQueue.Dequeue();

            //Si hemos llegado a un nodo sin hijos lo metemos en la lista definitva
            if(currenNode.getChildrens().Count == 0)
            {
                //Tomamos las dimensiones de la celda y las modificamos a placer con los factors, asi como la sepracion entre ellas para dar mas sensacion de aleatoriedad

                KeyValuePair<int,int> minMaxposX = new KeyValuePair<int, int>(currenNode.BottomLeftCorner.x + cellsOffset, currenNode.BottomRightCorner.x - cellsOffset); 
                KeyValuePair<int,int> minMaxposY = new KeyValuePair<int, int>(currenNode.BottomLeftCorner.y + cellsOffset, currenNode.TopLeftCorner.y - cellsOffset);

                Vector2Int newBottomLeftCorner, newTopRightCorner;

                newBottomLeftCorner = new Vector2Int(
                    Random.Range(minMaxposX.Key, (int)(minMaxposX.Key + (minMaxposX.Value - minMaxposX.Key) * bottomLeftCornerFactor)), 
                    Random.Range(minMaxposY.Key, (int)(minMaxposY.Key + (minMaxposY.Value - minMaxposY.Key) * bottomLeftCornerFactor)));


                newTopRightCorner = new Vector2Int(
                    Random.Range((int)(minMaxposX.Key + (minMaxposX.Value - minMaxposX.Key) * topRightCornerFactor), minMaxposX.Value), 
                    Random.Range((int)(minMaxposY.Key + (minMaxposY.Value - minMaxposY.Key) * topRightCornerFactor), minMaxposY.Value));

                currenNode.BottomLeftCorner= newBottomLeftCorner;
                currenNode.TopRightCorner= newTopRightCorner;
                currenNode.BottomRightCorner = new Vector2Int(newTopRightCorner.x, newBottomLeftCorner.y);
                currenNode.TopLeftCorner = new Vector2Int(newBottomLeftCorner.x, newTopRightCorner.y);

                currenNode.isCell = true;
                nodes.Add(currenNode);
            }

            else
            {
                //Si sigue teniendo hijos los encolamos en la cola
                foreach (CellNode child in currenNode.getChildrens())
                {
                    auxiliarQueue.Enqueue(child);
                }
            }

        }

        return nodes;
    }

    private void DivideCell(bool constrainwidth, bool constrainLength, CellNode currentNode, int cellWidthMin, int cellLengthMin, Queue<CellNode> graph)
    {

        Orientation divisionOrientation;
        Vector2Int coordinatesOfDivision = new Vector2Int();

        //Creamos dos nodos ya que la celda se divide en dos
        CellNode node1, node2;

        //Si cumple las dos restricciones de tamaño
        if (constrainwidth && constrainLength)
        {
            //La division de la celda sera aleatoria, Vertical u Horizontal
            divisionOrientation = (Orientation)Random.Range(0,2);
        }
        //Si solo cumple la restriccion de anchura
        else if (constrainwidth)
        {
            //La division sera vertical
            divisionOrientation = Orientation.Vertical;
        }

        //Si solo cumple la restriccion de altura, la divison sera horizontal
        else divisionOrientation = Orientation.Horizontal;

        //Dependiendo de la division, el corte imaginario de la celda tendra distintas coordenadas
        switch (divisionOrientation)
        {
            case Orientation.Vertical:
                //La division tendra coordenadas y = 0 y x = Valor random entre los extremos +/- el offset del ancho minimo de la celda para asegurar que no haya celdas
                //más pequeñas que el minimo
                coordinatesOfDivision = new Vector2Int(Random.Range((currentNode.TopLeftCorner.x + cellWidthMin), (currentNode.TopRightCorner.x - cellWidthMin)) ,0);
                break;
            case Orientation.Horizontal:
                //La division tendra coordenadas x = 0 y y = Valor random entre los extremos +/- el offset de la altura minima de la celda para asegurar que no haya celdas
                //más pequeñas que el minimo
                coordinatesOfDivision = new Vector2Int(0, Random.Range((currentNode.BottomLeftCorner.y + cellLengthMin), (currentNode.TopLeftCorner.y - cellLengthMin)));
                break;
        }

        //Si la division es horizontal
        if (divisionOrientation == Orientation.Horizontal)
        {
            //El nodo 1 tendra la misma esquina inferior izquierda que su padre pero como esquina superior derecha tendra x = topRightCorner del padre
            //y = posicion "y" donde se haya realizado el corte imaginario 
            node1 = new CellNode(currentNode.BottomLeftCorner, new Vector2Int(currentNode.TopRightCorner.x, coordinatesOfDivision.y), currentNode, currentNode.LayerIndex + 1);

            //El nodo 2 tendra la misma esquina superior derecha que su padre pero como esquina inferior izquierda tendra x = bottomLeftCorner del padre
            //y = posicion "y" donde se haya realizado el corte imagionario 
            node2 = new CellNode(new Vector2Int(currentNode.BottomLeftCorner.x, coordinatesOfDivision.y), currentNode.TopRightCorner, currentNode, currentNode.LayerIndex + 1);
        }

        //Si es Vertical, ocurre lo mismo pero a la inversa
        else
        {
            node1 = new CellNode(currentNode.BottomLeftCorner, new Vector2Int(coordinatesOfDivision.x, currentNode.TopRightCorner.y), currentNode, currentNode.LayerIndex + 1);

            node2 = new CellNode(new Vector2Int(coordinatesOfDivision.x, currentNode.BottomLeftCorner.y), currentNode.TopRightCorner, currentNode, currentNode.LayerIndex + 1);
        }

        allNodes.Add(node1);
        graph.Enqueue(node1);

        allNodes.Add(node2);
        graph.Enqueue(node2);
    }
}