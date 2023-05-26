using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject blod;
    float damage = 1f;
    public Slider slide;
    bool ataque = false;
    bool percibido = false;
    public GameObject player;
    bool perseguir_=false;
    void Start()
    {
        slide.value = damage;
        player = GameManager.Instance.getPlayer();
    }
    public float getDamage()
    {
        return damage;
    }
    private void OnCollisionEnter(Collision collision)
    {
      
        
            if (collision.gameObject.GetComponent<Projectile>())
            {
                //instancia sangre y baja la vida
                Instantiate(blod, collision.transform.position, Quaternion.identity);
                damage -= 0.1f;
                slide.value = damage;
                transform.LookAt(player.transform);

            }

            if (damage <= 0)
            {
                //activar animacion de muerte y desactivar collider
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
                this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                slide.enabled = false;
                    Enemyscount.Instance.enemis--;
                


            }

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("holaaa");
        if (other.gameObject.GetComponent<Projectile>())
        {
            Instantiate(blod, other.transform.position, Quaternion.identity);
            damage -= 0.1f;
            slide.value = damage;
        }

    }
  
}
