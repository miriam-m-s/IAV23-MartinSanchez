using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DisparaEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform lugarDisparo;
    public GameObject bala;
    bool ataca = false;
    [SerializeField]
    float tiempoEntreDisparo = 0.002f;
    float tiempo;
    EnemyController danio;
    NavMeshAgent nv;
    Vector3 positions;
    bool move = false;
    private void Start()
    {
        // Iniciar la rutina de temporizador
        danio = GetComponent<EnemyController>();
        StartCoroutine(TimerCoroutine());
        nv = GetComponent<NavMeshAgent>();
    }

    private System.Collections.IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // Esperar dos segundos

            // Imprimir un mensaje en la consola
            if(danio.getAtaque())
            Dispara();
        }
    }
    //si el enemigo esta en estado atacar atacará
    public void setAtaca(bool act)
    {
        ataca = act;
    }
    public void Dispara()
    {
        Transform muzzleSocket = lugarDisparo;

        Quaternion rotation = Quaternion.LookRotation(transform.forward * 1000.0f - muzzleSocket.position);


        if (Physics.Raycast(new Ray(this.gameObject.transform.position, transform.forward),
            out RaycastHit hit, 500.0f))
            rotation = Quaternion.LookRotation(hit.point - muzzleSocket.position);
        float angleInRadians = 90 * Mathf.Deg2Rad;

        // Crear el cuaternión de rotación
        Quaternion rotationQuaternion = Quaternion.AngleAxis(angleInRadians, Vector3.right);

        GameObject projectile = Instantiate(bala, muzzleSocket.position, muzzleSocket.rotation);

        projectile.GetComponent<Rigidbody>().velocity = -projectile.transform.up * 400;
    }
    // Update is called once per frame
    void Update()
    {
       // ataque();

    }

    public void ataque()
    {
        GameObject player = danio.getPlayer();
        if (danio.getAtaque() && player && !danio.getPerseguir())
        {
            if (!move) positions = transform.position;
            nv.isStopped = true;
            transform.LookAt(player.transform);
            transform.position = positions;
            move = true;
        }
        else
        {
            move = false;
        }
    }
}
