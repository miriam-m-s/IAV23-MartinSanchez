using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class VisionEnemy : MonoBehaviour
{
   
   
    //la layer donde se colisionan 
    public LayerMask layer_;
    public int resoll_ = 160;
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;
    public Material material_;
    public float rangoo=12;
    public float angulo;
    public float damageArea_ = 6;
    bool visto = false;
    Vector3 playerpos;
    EnemyController contoller_;
    void Start()
    {
        //convertimos a radianes
        angulo *= Mathf.Deg2Rad;
        //añadimos el material al cono
        transform.AddComponent<MeshRenderer>().material = material_;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        contoller_ = this.GetComponentInParent<EnemyController>();
        
    }


    void Update()
    {
        contoller_.SetAtaque(false);
        contoller_.setPerseguir(false);
        if (contoller_.isDeath())
        {
            Destroy(gameObject);
            return;
        }
        //ponemos en el vector el numero de vertices que se compondra nuestro cono de vista,estrar formnado por triasngulos ,formando una especie de "poligono"
        int[] triangulos = new int[(resoll_ - 1) * 3];
        Vector3[] Vertices = new Vector3[resoll_ + 1];
        //primer vertice es el del origen de nuestro enemigo
        Vertices[0] = Vector3.zero;
        //el primer angulo es este
        float Currentangle = -angulo / 2;
        // incremento de angulos en cada iteracion
        float incrermento = angulo / (resoll_ - 1f);
        float seno,coseno;
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
                 Vertices[i + 1] = VertForward * (hit.distance*2);

                visto = true;
                GameObject hitObject = hit.collider.gameObject;
                int layerIndex = hitObject.layer;
                string layerName = LayerMask.LayerToName(layerIndex);
                if (layerName!="Wall")
                {
                    contoller_.setPlayer(hitObject);
                    if (hit.distance <= damageArea_)
                        contoller_.SetAtaque(true);
                    else contoller_.setPerseguir(true);

                }




            }
            else
            {
                Vertices[i + 1] = VertForward * rangoo*2;
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