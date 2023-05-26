
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{

    public GameObject enemie;
    public int maxEnemiesPerCell;

    private List<Node> cells;

    public Spawner() { }

    public void StartSpawning(List<Node> cells_)
    {
        Debug.Log("heyeye");
        cells = cells_;
        if (maxEnemiesPerCell == 0) maxEnemiesPerCell = cells.Count;
        Spawn();
    }

    private void Spawn()
    {

        int randomAmount;
        Vector3 position;

        float minX, maxX, minY, maxY;

        float randomX;
        float randomY;

        foreach (Node cell in cells)
        {

            randomAmount = Random.Range(maxEnemiesPerCell/2, maxEnemiesPerCell);
            minX = cell.BottomLeftCorner.x;
            maxX = cell.BottomRightCorner.x;
            minY = cell.BottomLeftCorner.y;
            maxY = cell.TopLeftCorner.y;

            for(int i = 0; i < randomAmount; i++)
            {
                // Genera una posición aleatoria dentro del rectángulo
                randomX= Random.Range(minX, maxX);
                randomY= Random.Range(minY, maxY);
                float randomRotation = Random.Range(0f, 360f);
                Quaternion rotation = Quaternion.Euler(0f, randomRotation, 0f);


                // Aplicar la rotación al objeto
                Quaternion rot = rotation;
                position = new Vector3(randomX, -2, randomY);
                Instantiate(enemie, position, rot);
                Enemyscount.Instance.enemis++;
            }

        }
    }
}
