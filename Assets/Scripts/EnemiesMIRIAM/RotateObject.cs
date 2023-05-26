using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 90f; // Velocidad de rotación en grados por segundo

    private void Update()
    {
        // Calcula el ángulo de rotación para este frame
        float rotationAngle = rotationSpeed * Time.deltaTime;

        // Rota el objeto alrededor del eje Y
        transform.Rotate(Vector3.up, rotationAngle);
    }
}
