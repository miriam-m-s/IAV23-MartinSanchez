using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class playerVida : MonoBehaviour
{
    float damage = 1f;
    public Slider slide;
 
    public float getDamage()
    {
        return damage;
    }
    private void OnCollisionEnter(Collision collision)
    {

        //Nos dan un disparo
        if (collision.gameObject.GetComponent<Projectile>())
        {
           Destroy(collision.gameObject);
            damage -= 0.05f;
            slide.value = damage;
        }

        //morimos
        if (damage <= 0)
        {
            FindObjectOfType<GameManager>().showLoseText();
            Invoke("ReStartScene", 3);
        }

    }

    private void ReStartScene()
    {
        FindObjectOfType<GameManager>().ReStartScene();
    }
}
