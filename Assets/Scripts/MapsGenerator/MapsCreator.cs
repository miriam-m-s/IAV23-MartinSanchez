using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.AI;
public class MapsCreator : MonoBehaviour
{
    public int mapWidth, mapLength;
    public int cellWidthMin, cellLengthMin;
    public int divisions;
    public int corridorWidth;
    public Material material;
    [Range(0.1f, 0.4f)]
    public float bottomLeftCornerFactor;
    [Range(0.6f, 1.0f)]
    public float topRightCornerFactor;
    [Range(0, 2)]
    public int cellsOffset;
    string layerName="Wall";
    int layer;
    public GameObject wallPrefab;
    public GameObject roofPrefab;
    private NavMeshSurface navMeshSurface;

    // Start is called before the first frame update
    void Start()
    {
        layer = LayerMask.NameToLayer(layerName);
        CreateMap();
    }

    private void CreateMap()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        MapGenerator gen = new MapGenerator(mapWidth, mapLength, bottomLeftCornerFactor, topRightCornerFactor, cellsOffset, corridorWidth);
        var listCellsAndCorridors = gen.GetListCells(divisions, cellWidthMin, cellLengthMin);

        foreach ( var cell in listCellsAndCorridors )
        {
            CreateMesh(cell.BottomLeftCorner, cell.TopRightCorner, cell.isCell);
        }

        var onlyCells = gen.GetOnlyCells();

        CreateWalls(onlyCells);
        CreateRoofs(onlyCells);

        this.GetComponent<Spawner>().StartSpawning(onlyCells);
        navMeshSurface.BuildNavMesh();

    }

    private void CreateWalls(List<Node> Cells)
    {

        Vector3 position = new Vector3();

        Node corridor1 = null;
        Node corridor2 = null;
        Node corridor3 = null;

        float distance, scalex;

        int i = 0;

        List<GameObject> wallsAroundCorridors= new List<GameObject>();

        foreach (Node cell in Cells)
        {

            if (cell.getChildrens().Count != 0)
            {
                //Pasillo del medio
                corridor1 = cell.getChildrens()[0];
                //Pasillo izquierda si el pasillo de medio está arriba o abajo, pasillo de abajo si el pasillo de medio esta a izquierda o derecha
                corridor2 = cell.getChildrens()[1];
                //Pasillo  derecha si el pasillo de medio está arriba o abajo, pasillo de arriba si el pasillo de medio esta a izquierda o derecha
                corridor3 = cell.getChildrens()[2];

                //La mayoría tienen pasillo central
                if (corridor1 != null)
                {

                    //Pasillo del medio abajo
                    if (corridor1.TopLeftCorner.y == cell.BottomLeftCorner.y)
                    {

                        //Primero ponemos las paredes laterales del pasillo

                        //Averiguamos la altura del pasillo
                        distance = Vector2.Distance(corridor1.BottomLeftCorner, corridor1.TopLeftCorner);
                        //Sacamos la escala para ajustar la pared
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(corridor1.TopLeftCorner.x, 0, corridor1.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor1.TopRightCorner.x, 0, corridor1.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        i++;

                        //Si hay pasillo de la izquieda abajo
                        if (corridor2 != null)
                        {
                            distance = Vector2.Distance(corridor2.BottomLeftCorner, corridor2.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de los laterales del pasillo
                            position = new Vector3(corridor2.TopLeftCorner.x, 0, corridor2.getCenter().y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor2.TopRightCorner.x, 0, corridor2.getCenter().y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //Averiguamos el ancho entre pasillos
                            distance = Vector2.Distance(corridor1.TopLeftCorner, corridor2.TopRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados de las celdas
                            position = new Vector3(corridor1.TopLeftCorner.x - distance / 2, 0, cell.BottomLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor1.BottomLeftCorner.x - distance / 2, 0, corridor1.BottomLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        else
                        {
                            distance = Vector2.Distance(corridor1.TopLeftCorner, cell.BottomLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados de las celdas
                            position = new Vector3(corridor1.TopLeftCorner.x - distance / 2, 0, cell.BottomLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                            distance = Vector2.Distance(corridor1.BottomLeftCorner, cell.conectedWith.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            position = new Vector3(corridor1.TopLeftCorner.x - distance / 2, 0, corridor1.BottomLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        //Si hay pasillo de la derecha abajo
                        if (corridor3 != null)
                        {
                            distance = Vector2.Distance(corridor3.BottomLeftCorner, corridor3.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de los laterales del pasillo
                            position = new Vector3(corridor3.TopLeftCorner.x, 0, corridor3.getCenter().y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor3.TopRightCorner.x, 0, corridor3.getCenter().y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //Averiguamos el ancho entre pasillos
                            distance = Vector2.Distance(corridor1.TopRightCorner, corridor3.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados de las celdas
                            position = new Vector3(corridor3.TopLeftCorner.x - distance / 2, 0, cell.BottomLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor3.BottomLeftCorner.x - distance / 2, 0, corridor1.BottomLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        else
                        {
                            distance = Vector2.Distance(corridor1.TopRightCorner, cell.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados de las celdas
                            position = new Vector3(corridor1.TopRightCorner.x + distance / 2, 0, cell.BottomLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                            distance = Vector2.Distance(corridor1.BottomRightCorner, cell.conectedWith.TopRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            position = new Vector3(corridor1.TopRightCorner.x + distance / 2, 0, corridor1.BottomLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }
                    }

                    //Pasillo del medio arriba
                    else if (corridor1.BottomLeftCorner.y == cell.TopLeftCorner.y)
                    {

                        //Primero ponemos las paredes laterales del pasillo

                        //Averiguamos la altura del pasillo
                        distance = Vector2.Distance(corridor1.BottomLeftCorner, corridor1.TopLeftCorner);
                        //Sacamos la escala para ajustar la pared
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(corridor1.TopLeftCorner.x, 0, corridor1.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor1.TopRightCorner.x, 0, corridor1.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Si hay pasillo de arriba a la izquierda
                        if (corridor2 != null)
                        {

                            distance = Vector2.Distance(corridor2.BottomLeftCorner, corridor2.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de los laterales del pasillo
                            position = new Vector3(corridor2.TopLeftCorner.x, 0, corridor2.getCenter().y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor2.TopRightCorner.x, 0, corridor2.getCenter().y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //Sacamos el ancho entre pasillos
                            distance = Vector2.Distance(corridor1.BottomLeftCorner, corridor2.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados de las celdas
                            position = new Vector3(corridor1.BottomLeftCorner.x - distance / 2, 0, cell.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor1.BottomLeftCorner.x - distance / 2, 0, corridor1.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        else
                        {
                            distance = Vector2.Distance(corridor1.BottomLeftCorner, cell.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados de las celdas
                            position = new Vector3(corridor1.BottomLeftCorner.x - distance / 2, 0, cell.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                            distance = Vector2.Distance(corridor1.TopLeftCorner, cell.conectedWith.BottomLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            position = new Vector3(corridor1.TopLeftCorner.x - distance / 2, 0, corridor1.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        //Si hay pasillo de arriba a la derecha
                        if (corridor3 != null)
                        {
                            distance = Vector2.Distance(corridor3.BottomRightCorner, corridor3.TopRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de los laterales del pasillo
                            position = new Vector3(corridor3.TopLeftCorner.x, 0, corridor3.getCenter().y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor3.TopRightCorner.x, 0, corridor3.getCenter().y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //Sacamos el ancho entre pasillos
                            distance = Vector2.Distance(corridor1.BottomRightCorner, corridor3.BottomLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados de las celdas
                            position = new Vector3(corridor3.BottomLeftCorner.x - distance / 2, 0, cell.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor3.TopLeftCorner.x - distance / 2, 0, corridor1.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        else
                        {
                            distance = Vector2.Distance(corridor1.BottomRightCorner, cell.TopRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados de las celdas
                            position = new Vector3(corridor1.BottomRightCorner.x + distance / 2, 0, cell.TopRightCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                            distance = Vector2.Distance(corridor1.TopRightCorner, cell.conectedWith.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            position = new Vector3(corridor1.BottomRightCorner.x + distance / 2, 0, corridor1.TopRightCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }


                    }

                    //Pasillo del medio izquierda
                    else if (corridor1.BottomRightCorner.x == cell.BottomLeftCorner.x)
                    {

                        //Averiguamos el ancho
                        distance = Vector2.Distance(corridor1.BottomRightCorner, corridor1.BottomLeftCorner);
                        //Sacamos la escala para que la pared se ajuste
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Primero ponemos las paredes laterales del pasillo
                        position = new Vector3(corridor1.getCenter().x, 0, corridor1.TopLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor1.getCenter().x, 0, corridor1.BottomRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Si hay pasillo de abajo
                        if (corridor2 != null)
                        {

                            distance = Vector2.Distance(corridor2.BottomLeftCorner, corridor2.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Primero ponemos las paredes laterales del pasillo
                            position = new Vector3(corridor2.getCenter().x, 0, corridor2.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor2.getCenter().x, 0, corridor2.BottomRightCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //Sacamos la altura entre pasillos
                            distance = Vector2.Distance(corridor1.BottomRightCorner, corridor2.TopRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados
                            position = new Vector3(corridor1.BottomRightCorner.x, 0, corridor1.BottomRightCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor1.BottomLeftCorner.x, 0, corridor1.BottomLeftCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                        }

                        else
                        {
                            distance = Vector2.Distance(corridor1.BottomRightCorner, cell.BottomLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados
                            position = new Vector3(corridor1.BottomRightCorner.x, 0, corridor1.BottomRightCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                            distance = Vector2.Distance(corridor1.BottomLeftCorner, cell.conectedWith.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            position = new Vector3(corridor1.BottomLeftCorner.x, 0, corridor1.BottomLeftCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        //Si hay pasillo de arriba
                        if (corridor3 != null)
                        {
                            distance = Vector2.Distance(corridor3.BottomLeftCorner, corridor3.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Primero ponemos las paredes laterales del pasillo
                            position = new Vector3(corridor3.getCenter().x, 0, corridor3.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor3.getCenter().x, 0, corridor3.BottomRightCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //Sacamos la altura entre pasillos
                            distance = Vector2.Distance(corridor1.TopRightCorner, corridor3.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados
                            position = new Vector3(corridor1.TopRightCorner.x, 0, corridor3.BottomRightCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor1.TopLeftCorner.x, 0, corridor3.BottomLeftCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        else
                        {
                            distance = Vector2.Distance(corridor1.TopRightCorner, cell.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados
                            position = new Vector3(corridor1.BottomRightCorner.x, 0, cell.TopLeftCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                            distance = Vector2.Distance(corridor1.TopLeftCorner, cell.conectedWith.TopRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            position = new Vector3(corridor1.BottomLeftCorner.x, 0, cell.conectedWith.TopLeftCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }


                    }

                    //Pasillo del medio a la derecha
                    else if (corridor1.BottomLeftCorner.x == cell.BottomRightCorner.x)
                    {

                        //Averiguamos el ancho
                        distance = Vector2.Distance(corridor1.BottomRightCorner, corridor1.BottomLeftCorner);
                        //Sacamos la escala para que la pared se ajuste
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Primero ponemos las paredes laterales del pasillo
                        position = new Vector3(corridor1.getCenter().x, 0, corridor1.TopLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor1.getCenter().x, 0, corridor1.BottomRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Si hay pasillo de abajo
                        if (corridor2 != null)
                        {
                            distance = Vector2.Distance(corridor2.BottomLeftCorner, corridor2.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Primero ponemos las paredes laterales del pasillo
                            position = new Vector3(corridor2.getCenter().x, 0, corridor2.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor2.getCenter().x, 0, corridor2.BottomRightCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //Sacamos la altura entre pasillos
                            distance = Vector2.Distance(corridor1.BottomLeftCorner, corridor2.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados
                            position = new Vector3(corridor1.BottomLeftCorner.x, 0, corridor1.BottomLeftCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor1.BottomRightCorner.x, 0, corridor1.BottomRightCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                        }

                        else
                        {
                            distance = Vector2.Distance(corridor1.BottomLeftCorner, cell.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados
                            position = new Vector3(corridor1.BottomLeftCorner.x, 0, corridor1.BottomLeftCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                            distance = Vector2.Distance(corridor1.BottomRightCorner, cell.conectedWith.BottomLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            position = new Vector3(corridor1.BottomRightCorner.x, 0, corridor1.BottomRightCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        //Si hay pasillo de arriba
                        if (corridor3 != null)
                        {
                            distance = Vector2.Distance(corridor3.BottomLeftCorner, corridor3.BottomRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Primero ponemos las paredes laterales del pasillo
                            position = new Vector3(corridor3.getCenter().x, 0, corridor3.TopLeftCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor3.getCenter().x, 0, corridor3.BottomRightCorner.y);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //Sacamos la altura entre pasillos
                            distance = Vector2.Distance(corridor1.TopLeftCorner, corridor3.BottomLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados
                            position = new Vector3(corridor1.TopLeftCorner.x, 0, corridor3.BottomLeftCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                            position = new Vector3(corridor1.TopRightCorner.x, 0, corridor3.BottomRightCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                        else
                        {
                            distance = Vector2.Distance(corridor1.TopLeftCorner, cell.TopRightCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            //Instanciamos los muros de ambos lados
                            position = new Vector3(corridor1.TopLeftCorner.x, 0, cell.TopRightCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;

                            //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                            distance = Vector2.Distance(corridor1.TopRightCorner, cell.conectedWith.TopLeftCorner);
                            scalex = distance / wallPrefab.transform.localScale.x;

                            position = new Vector3(corridor1.TopRightCorner.x, 0, cell.conectedWith.TopRightCorner.y - distance / 2);
                            wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                            wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                            wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                            i++;
                        }

                    }
                }

                //Algunos solo tienen un pasillo en los extremos pero solo tendrán uno porque si tuviesen corridor2 y 3 también deberián tener
                //corridor1 que es el del medio

                else if (corridor2 != null)
                {
                    //Abajo
                    if (corridor2.TopLeftCorner.y == cell.BottomLeftCorner.y)
                    {
                        //Altura del pasillo
                        distance = Vector2.Distance(corridor2.BottomLeftCorner, corridor2.TopLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de los laterales del pasillo
                        position = new Vector3(corridor2.TopLeftCorner.x, 0, corridor2.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor2.TopRightCorner.x, 0, corridor2.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Averiguamos la distancia hasta el final de la celda
                        distance = Vector2.Distance(corridor2.TopRightCorner, cell.BottomRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de ambos lados de las celdas
                        position = new Vector3(cell.BottomRightCorner.x - distance / 2, 0, cell.BottomRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                        distance = Vector2.Distance(corridor2.BottomRightCorner, cell.conectedWith.TopRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(cell.conectedWith.BottomRightCorner.x - distance / 2, 0, corridor2.BottomRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                    }

                    //Arriba
                    else if (corridor2.BottomLeftCorner.y == cell.TopLeftCorner.y)
                    {
                        distance = Vector2.Distance(corridor2.BottomLeftCorner, corridor2.TopLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de los laterales del pasillo
                        position = new Vector3(corridor2.TopLeftCorner.x, 0, corridor2.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor2.TopRightCorner.x, 0, corridor2.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Averiguamos la distancia hasta el final de la celda
                        distance = Vector2.Distance(corridor2.BottomRightCorner, cell.TopRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de ambos lados de las celdas
                        position = new Vector3(cell.TopRightCorner.x - distance / 2, 0, cell.TopRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                        distance = Vector2.Distance(corridor2.TopRightCorner, cell.conectedWith.BottomRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(cell.conectedWith.TopRightCorner.x - distance / 2, 0, corridor2.TopRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                    }

                    //Izquierda
                    else if (corridor2.BottomRightCorner.x == cell.BottomLeftCorner.x)
                    {
                        distance = Vector2.Distance(corridor2.BottomLeftCorner, corridor2.BottomRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Primero ponemos las paredes laterales del pasillo
                        position = new Vector3(corridor2.getCenter().x, 0, corridor2.TopLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor2.getCenter().x, 0, corridor2.BottomLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Sacamos la distancia hasta el final de la celda
                        distance = Vector2.Distance(corridor2.TopRightCorner, cell.TopLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de ambos lados
                        position = new Vector3(cell.BottomLeftCorner.x, 0, cell.TopLeftCorner.y - distance / 2);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                        distance = Vector2.Distance(corridor2.TopLeftCorner, cell.conectedWith.TopRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(corridor2.TopLeftCorner.x, 0, cell.conectedWith.TopLeftCorner.y - distance / 2);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                    }

                    //Derecha
                    else if (corridor2.BottomLeftCorner.x == cell.BottomRightCorner.x)
                    {
                        distance = Vector2.Distance(corridor2.BottomLeftCorner, corridor2.BottomRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Primero ponemos las paredes laterales del pasillo
                        position = new Vector3(corridor2.getCenter().x, 0, corridor2.BottomLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor2.getCenter().x, 0, corridor2.TopLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Sacamos la altura entre pasillos
                        distance = Vector2.Distance(corridor2.TopLeftCorner, cell.TopRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de ambos lados
                        position = new Vector3(cell.BottomRightCorner.x, 0, cell.TopRightCorner.y - distance / 2);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                        distance = Vector2.Distance(corridor2.TopRightCorner, cell.conectedWith.TopLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(corridor2.TopRightCorner.x, 0, cell.conectedWith.TopRightCorner.y - distance / 2);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                    }

                }

                else if (corridor3 != null)
                {
                    //Abajo
                    if (corridor3.TopLeftCorner.y == cell.BottomLeftCorner.y)
                    {
                        //Altura del pasillo
                        distance = Vector2.Distance(corridor3.BottomLeftCorner, corridor3.TopLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de los laterales del pasillo
                        position = new Vector3(corridor3.TopLeftCorner.x, 0, corridor3.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor3.TopRightCorner.x, 0, corridor3.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Averiguamos la distancia hasta el final de la celda
                        distance = Vector2.Distance(corridor3.TopLeftCorner, cell.BottomLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de ambos lados de las celdas
                        position = new Vector3(cell.BottomLeftCorner.x + distance / 2, 0, cell.BottomRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                        distance = Vector2.Distance(corridor3.BottomLeftCorner, cell.conectedWith.TopLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(cell.conectedWith.BottomLeftCorner.x + distance / 2, 0, corridor3.BottomRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                    }

                    //Arriba
                    else if (corridor3.BottomLeftCorner.y == cell.TopLeftCorner.y)
                    {
                        distance = Vector2.Distance(corridor3.BottomLeftCorner, corridor3.TopLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de los laterales del pasillo
                        position = new Vector3(corridor3.TopLeftCorner.x, 0, corridor3.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor3.TopRightCorner.x, 0, corridor3.getCenter().y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Averiguamos la distancia hasta el final de la celda
                        distance = Vector2.Distance(corridor3.BottomLeftCorner, cell.TopLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de ambos lados de las celdas
                        position = new Vector3(cell.TopLeftCorner.x + distance / 2, 0, cell.TopRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                        distance = Vector2.Distance(corridor3.TopLeftCorner, cell.conectedWith.BottomLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(cell.conectedWith.TopLeftCorner.x + distance / 2, 0, corridor3.TopRightCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                    }

                    //Izquierda
                    else if (corridor3.BottomRightCorner.x == cell.BottomLeftCorner.x)
                    {
                        distance = Vector2.Distance(corridor3.BottomLeftCorner, corridor3.BottomRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Primero ponemos las paredes laterales del pasillo
                        position = new Vector3(corridor3.getCenter().x, 0, corridor3.TopLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor3.getCenter().x, 0, corridor3.BottomLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Sacamos la distancia hasta el final de la celda
                        distance = Vector2.Distance(corridor3.BottomRightCorner, cell.BottomLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de ambos lados
                        position = new Vector3(cell.BottomLeftCorner.x, 0, cell.BottomLeftCorner.y + distance / 2);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                        distance = Vector2.Distance(corridor3.BottomLeftCorner, cell.conectedWith.BottomRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(corridor3.TopLeftCorner.x, 0, cell.conectedWith.BottomLeftCorner.y + distance / 2);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                    }

                    //Derecha
                    else if (corridor3.BottomLeftCorner.x == cell.BottomRightCorner.x)
                    {
                        distance = Vector2.Distance(corridor3.BottomLeftCorner, corridor3.BottomRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Primero ponemos las paredes laterales del pasillo
                        position = new Vector3(corridor3.getCenter().x, 0, corridor3.BottomLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                        position = new Vector3(corridor3.getCenter().x, 0, corridor3.TopLeftCorner.y);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //Sacamos la altura entre pasillos
                        distance = Vector2.Distance(corridor3.BottomLeftCorner, cell.BottomRightCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        //Instanciamos los muros de ambos lados
                        position = new Vector3(cell.BottomRightCorner.x, 0, cell.BottomRightCorner.y + distance / 2);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;

                        //La pared contraria puede tener otro ancho por lo que recalculamos la distancia 
                        distance = Vector2.Distance(corridor3.BottomRightCorner, cell.conectedWith.BottomLeftCorner);
                        scalex = distance / wallPrefab.transform.localScale.x;

                        position = new Vector3(corridor3.TopRightCorner.x, 0, cell.conectedWith.BottomRightCorner.y + distance / 2);
                        wallsAroundCorridors.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                        wallsAroundCorridors[i].transform.Rotate(0, 90, 0);
                        wallsAroundCorridors[i].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                        i++;
                    }
                }

            }

        }

        List<GameObject> wallsAroundCells = new List<GameObject>();
        int j = 0;

        //Ponemos las cuatro paredes de cada celda
        foreach (Node cell in Cells)
        {
            if (cell.isCell)
            {
                //Comprobamos si tiene conexiones por abajo ya que si tiene no podemos poner pared
                if (cell.down == false)
                {
                    position = new Vector3(cell.getCenter().x, 0, cell.BottomLeftCorner.y);
                    wallsAroundCells.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                    distance = Vector2.Distance(cell.TopLeftCorner, cell.TopRightCorner);
                    scalex = distance / wallPrefab.transform.localScale.x;
                    wallsAroundCells[j].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                    j++;
                }
                //Comprobamos si tiene conexiones por arriba ya que si tiene no podemos poner pared
                if (cell.up == false)
                {
                    position = new Vector3(cell.getCenter().x, 0, cell.TopLeftCorner.y);
                    wallsAroundCells.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                    distance = Vector2.Distance(cell.TopLeftCorner, cell.TopRightCorner);
                    scalex = distance / wallPrefab.transform.localScale.x;
                    wallsAroundCells[j].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                    j++;
                }
                //Comprobamos si tiene conexiones por izquierda ya que si tiene no podemos poner pared
                if (cell.left == false)
                {
                    position = new Vector3(cell.BottomLeftCorner.x, 0, cell.getCenter().y);
                    wallsAroundCells.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                    distance = Vector2.Distance(cell.BottomLeftCorner, cell.TopLeftCorner);
                    scalex = distance / wallPrefab.transform.localScale.x;
                    wallsAroundCells[j].transform.localScale = new Vector3(scalex / 5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                    wallsAroundCells[j].transform.Rotate(0, 90, 0);
                    j++;
                }
                //Comprobamos si tiene conexiones por derecha ya que si tiene no podemos poner pared
                if (cell.right == false)
                {
                    position = new Vector3(cell.BottomRightCorner.x, 0, cell.getCenter().y);
                    wallsAroundCells.Add(Instantiate(wallPrefab, position, Quaternion.identity));
                    distance = Vector2.Distance(cell.BottomRightCorner, cell.TopRightCorner);
                    scalex = distance / wallPrefab.transform.localScale.x;
                    wallsAroundCells[j].transform.localScale = new Vector3(scalex/5, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
                    wallsAroundCells[j].transform.Rotate(0, 90, 0);
                    j++;
                }
            }
        }

    }

    private void CreateRoofs(List<Node> Cells)
    {

        Vector3 position = Vector3.zero;
        Vector2 aux = Vector2.zero;

        float distance;
        float scalex, scaley;

        //Cada celda tendrá 4 techos
        foreach (Node cell in Cells)
        {
            //Escalas de las partes del techo
            distance = Vector2.Distance(cell.TopLeftCorner, cell.BottomLeftCorner);
            scaley = (distance / 2) / roofPrefab.transform.localScale.x;

            distance = Vector2.Distance(cell.BottomLeftCorner, cell.BottomRightCorner);
            scalex = (distance / 2) / roofPrefab.transform.localScale.x;

            //Techo de arriba a la izquierda
            aux = CalculateCenter(cell.TopLeftCorner, cell.getCenter());
            position = new Vector3(aux.x, 2, aux.y);

            Instantiate(roofPrefab, position, Quaternion.identity).transform.localScale = new Vector3(scalex/ 5, roofPrefab.transform.localScale.y, scaley/ 5);

            //Techo de arriba a la derecha
            aux = CalculateCenter(cell.TopRightCorner, cell.getCenter());
            position = new Vector3(aux.x, 2, aux.y);

            //distance = Vector2.Distance(cell.TopRightCorner, cell.getCenter());
            //scale = distance / roofPrefab.transform.localScale.x;

            Instantiate(roofPrefab, position, Quaternion.identity).transform.localScale = new Vector3(scalex / 5, roofPrefab.transform.localScale.y, scaley / 5);

            //Techo de abajo a la izquierda
            aux = CalculateCenter(cell.BottomLeftCorner, cell.getCenter());
            position = new Vector3(aux.x, 2, aux.y);

            //distance = Vector2.Distance(cell.BottomLeftCorner, cell.getCenter());
            //scale = distance / roofPrefab.transform.localScale.x;

            Instantiate(roofPrefab, position, Quaternion.identity).transform.localScale = new Vector3(scalex / 5, roofPrefab.transform.localScale.y, scaley / 5);

            //Techo de abajo a la derecha
            aux = CalculateCenter(cell.BottomRightCorner, cell.getCenter());
            position = new Vector3(aux.x, 2, aux.y);

            //distance = Vector2.Distance(cell.BottomRightCorner, cell.getCenter());
            //scale = distance / roofPrefab.transform.localScale.x;

            Instantiate(roofPrefab, position, Quaternion.identity).transform.localScale = new Vector3(scalex / 5, roofPrefab.transform.localScale.y, scaley / 5);

        }

    }

    private Vector2 CalculateCenter(Vector2 pointA, Vector2 pointB)
    {
        Vector3 center = (pointA + pointB) / 2f;
        return center;
    }

    //Representacion gráfica de las celdas
    private void CreateMesh(Vector2 bottomLeftCorner_, Vector2 topRightCorner_, bool isCell)
    {

        Vector3 bottomLeftCorner = new Vector3(bottomLeftCorner_.x, 0, bottomLeftCorner_.y);
        Vector3 bottomRightCorner = new Vector3(topRightCorner_.x, 0, bottomLeftCorner_.y);
        Vector3 topLeftCorner = new Vector3(bottomLeftCorner_.x, 0, topRightCorner_.y);
        Vector3 topRightCorner = new Vector3(topRightCorner_.x, 0, topRightCorner_.y);

        // Crear los vértices del Mesh
        Vector3[] vertexs = new Vector3[4]
        {
            topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner
        };

        Vector2[] uvs = new Vector2[vertexs.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertexs[i].x, vertexs[i].z);
        }

        // Crear los triángulos del Mesh
        int[] triangles = new int[]
        {
            0, 1, 2, // Primer triángulo
            2, 1, 3  // Segundo triángulo
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertexs;
        mesh.uv= uvs;
        mesh.triangles = triangles;

        string name = "";

        if (isCell) name = "Mesh" + bottomLeftCorner_;
        else name = "Corridor" + bottomLeftCorner_;

        GameObject floor = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(Rigidbody), typeof(NavMesh));

        floor.transform.position= new Vector3(0,-2.5f,0);
        floor.transform.localScale= Vector3.one;
        floor.GetComponent<MeshFilter>().mesh = mesh;
        floor.GetComponent<MeshRenderer>().material= material;
        floor.GetComponent<MeshCollider>().isTrigger = false;
        floor.GetComponent<MeshCollider>().sharedMesh = mesh;
        floor.GetComponent<MeshCollider>().convex = true;
        floor.transform.localScale = new Vector3(floor.transform.localScale.x, 0.01f, floor.transform.localScale.z);
        floor.GetComponent<Rigidbody>().isKinematic= true;
        floor.layer = layer;

    }
}
