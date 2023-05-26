using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;


public class merodear : MonoBehaviour
{
    // Start is called before the first frame update
    public int rutin=1;
    float timer;


    //la layer donde se colisionan 
    public LayerMask layer_;
    float grado;
    Quaternion angulo;
    NavMeshAgent nv;
    EnemyController danio;
    Vector3 finalPosition;
    bool colision;

    public float probabilityOfOne = 0.7f;

    void Start()
    {
        nv = GetComponent<NavMeshAgent>();
        nv.isStopped = false;
        Debug.Log("heyeyeyjnfjushbuirg");
        danio = GetComponent<EnemyController>();
        rutin = GenerateRandomNumber();

    }
    //creamos un generador de numeros aleatorios en el que tenga preferencia el numero 1
    private int GenerateRandomNumber()
    {
        if (UnityEngine.Random.value < probabilityOfOne)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public void merodeos()
    {
        timer += 1 * Time.deltaTime;
        if (timer >= 4)
        {
            rutin = GenerateRandomNumber();
            timer = 0;
        
        }
        switch(rutin)
        {
            //si el numero random es 0 se queda quieto y hace la anaimcion de idle
            case 0:
                if(nv!=null) 
                nv.isStopped = true;
                break;
            //si el numero random es 1 calcula el angulo aleatoriamente 
            case 1:
                if (nv != null)
                    nv.isStopped = false;

                //vector forward del objeto

                Vector3 forward = transform.forward;

                // Genera un ángulo aleatorio entre -45 y 45 grados
                grado = UnityEngine.Random.Range(-90f, 90f);

                // Crea un Quaternion que rota alrededor del eje de rotación en el ángulo aleat
                angulo = Quaternion.Euler(0, grado, 0);
                rutin++;
                break;
            case 2:
                nv.isStopped = false;
                Vector3 currentPosition = gameObject.transform.position;
                Vector3 direction = angulo * Vector3.forward;
                float distance = 5f; // distancia deseada
                finalPosition = currentPosition + direction * distance;
                if (nv != null)
                    nv.SetDestination(finalPosition);
                rutin++;
                break;
              case 3:
                
                Vector3 final=finalPosition;
                Vector3 raycastDirection = transform.forward;
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, raycastDirection, out hit, 3))
                {
                    // Calcular una nueva dirección para evitar el obstáculo
                    Vector3 avoidDirection = Vector3.Reflect(transform.forward, hit.normal);

                    // Asignar la nueva dirección al NavMeshAgent
                    final = (transform.position + avoidDirection);
                    Vector3 puntoFinal = transform.position + raycastDirection * 3;
                    Debug.DrawLine(transform.position, puntoFinal, Color.red);
                 
                }
                else
                {
                    Vector3 current = gameObject.transform.position;
                    Vector3 dire = angulo * Vector3.forward;
                    float diste = 5f; // distancia deseada
                    finalPosition = current + dire * diste;
                    final = finalPosition;
                    Vector3 puntoFinal = transform.position + raycastDirection * 3;
                    Debug.DrawLine(transform.position, puntoFinal, Color.green);
                   
                }
                if (nv != null)
                    nv.SetDestination(final);
                break;
        }
    }
    float GradosRadianes(float grados)
    { return grados * (Mathf.PI / 180); }

    // Funcion para calcular el producto escalar
    float ScalarProduct(Vector3 v1, Vector3 v2)
    { return (v1.x * v2.z - v2.x * v1.z); }
    
}
