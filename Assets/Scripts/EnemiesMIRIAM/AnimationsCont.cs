using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationsCont : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    EnemyController damageEnemy;
    Rigidbody rb;
    NavMeshAgent nv;
    void Start()
    {
        animator = GetComponent<Animator>();
        damageEnemy = GetComponent<EnemyController>();
        nv= GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //SI ESTA MUERTO ACTIVAMOS LA ANIMACIÓN DE MUERTE
        if(damageEnemy.isDeath())
        {
           
            animator.SetBool("isShooting", false);
            animator.SetBool("isDeath", true);
        }
        else if (damageEnemy.getAtaque())//SI ATACA ACTIVAMOS LA ANIMACIÓN DE ATAQUE
        {
            animator.SetBool("iswalking", false);
            animator.SetBool("isShooting", true);
        }
        else if (!nv.isStopped)//SI CAMINA ACTIVAMOS LA ANIMACIÓN DE CAMINAR
        {
            animator.SetBool("iswalking", true);
            animator.SetBool("isShooting", false);
        }
        else //SI NO CAMINA ACTIVAMOS LA ANIMACIÓN DE IDLE
        {
            animator.SetBool("isShooting", false);
            animator.SetBool("iswalking", false);
        }
       
    }
}
