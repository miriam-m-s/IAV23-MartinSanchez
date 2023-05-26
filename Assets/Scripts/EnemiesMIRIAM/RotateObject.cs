using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 90f; // Velocidad de rotaci�n en grados por segundo

    private void Update()
    {
        // Calcula el �ngulo de rotaci�n para este frame
        float rotationAngle = rotationSpeed * Time.deltaTime;

        // Rota el objeto alrededor del eje Y
        transform.Rotate(Vector3.up, rotationAngle);
    }
}
