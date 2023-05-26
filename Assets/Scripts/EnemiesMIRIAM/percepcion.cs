using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class percepcion : MonoBehaviour
{
    //la layer donde se colisionan 
    public LayerMask layer_;
    public int resoll_ = 160;
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;
    public Material material_;
    public float rangoo = 12;
    public float angulo;
    bool visto = false;
    Vector3 playerpos;
    EnemyController danio;
    public Image inter;
    void Start()
    {
        //convertimos a radianes
        angulo *= Mathf.Deg2Rad;
        //añadimos el material al cono
        transform.AddComponent<MeshRenderer>().material = material_;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        danio = this.GetComponentInParent<EnemyController>();

    }


    void Update()
    {

        if (danio.isDeath())
        {
            Destroy(gameObject);
            return;
        }

        percepcionVista();
    }

    private void percepcionVista()
    {
        inter.enabled = false;

        danio.SetPercepcion(false);
        //ponemos en el vector el numero de vertices que se compondra nuestro cono de vista,estrar formnado por triasngulos ,formando una especie de "poligono"
        int[] triangulos = new int[(resoll_ - 1) * 3];
        Vector3[] Vertices = new Vector3[resoll_ + 1];
        //primer vertice es el del origen de nuestro enemigo
        Vertices[0] = Vector3.zero;
        //el primer angulo es este
        float Currentangle = -angulo / 2;
        // incremento de angulos en cada iteracion
        float incrermento = angulo / (resoll_ - 1f);
        float seno, coseno;
        for (int i = 0; i < resoll_; i++)
        {
            seno = Mathf.Sin(Currentangle);
            coseno = Mathf.Cos(Currentangle);
            //local
            Vector3 RaycastDirection = (transform.forward * coseno) + (transform.right * seno);
            //global
            Vector3 VertForward = (Vector3.forward * coseno) + (Vector3.right * seno);
            //si colisiona con alguna de las capas se acorta ese poligono
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, rangoo, layer_))
            {
                // Vertices[i + 1] = VertForward * rangoo*2;
              
                Vertices[i + 1] = VertForward * (hit.distance * 2);

                visto = true;
                GameObject hitObject = hit.collider.gameObject;
                int layerIndex = hitObject.layer;
                string layerName = LayerMask.LayerToName(layerIndex);
                if (layerName != "Wall")
                {
                    Debug.Log("aaaaaaaaTAQUEEEa");
                    danio.setPlayer(hitObject);
                    danio.SetPercepcion(true);
                    inter.enabled = true;

                }




            }
            else
            {
                Vertices[i + 1] = VertForward * rangoo * 2;
            }


            Currentangle += incrermento;
        }
        for (int i = 0, j = 0; i < triangulos.Length; i += 3, j++)
        {
            triangulos[i] = 0;
            triangulos[i + 1] = j + 1;
            triangulos[i + 2] = j + 2;
        }
        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangulos;

        MeshFilter_.mesh = VisionConeMesh;
    }
}
