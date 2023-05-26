using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class mirarPlayer : MonoBehaviour
{

    NavMeshAgent nv;
    EnemyController danio;
    void Start()
    {
      
        nv = GetComponent<NavMeshAgent>();
        nv.isStopped = true;
        danio = GetComponent<EnemyController>();
    
    }


private void mira()
{
        GameObject player = danio.getPlayer();
        //si pasados los x segundos el jugador esta alli ,el enemigo se gira y comenzará a atacar
        if (player && danio.getPercibir() && !danio.getAtaque() && !danio.getPerseguir())
        {
            transform.LookAt(player.transform);

        }
       
}
    public void percepcion()
    {
        //si el enemigo percibe al jugador suelta un random de tiempo,es el tiempo que tiene el jugador 
        //pARA ESTAR EN EL CONO DE vision de la percepcion
        float s = UnityEngine.Random.Range(3,5);
        Invoke("mira",s);
       
       
    }
}
