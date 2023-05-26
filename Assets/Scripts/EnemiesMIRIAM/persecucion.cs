using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class persecucion : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody body;
    NavMeshAgent nv;
    EnemyController danio;
    void Start()
    {
        body = GetComponent<Rigidbody>();
        nv = GetComponent<NavMeshAgent>();
        nv.isStopped = true;
        danio = GetComponent<EnemyController>();
    }


    public void persecue()
    {
        GameObject player = danio.getPlayer();
        if (player && danio.getPerseguir() && !danio.getAtaque() && !danio.isDeath())
        {
            //persigue al jugador
            nv.isStopped = false;
            nv.SetDestination(player.transform.position);

        }
    }
}
