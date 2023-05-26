using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    bool ataque = false;
    bool percibido = false;
    bool perseguir_ = false;
    GameObject player = null;
    DamageEnemy dam;
    void Start()
    {
        dam= GetComponent<DamageEnemy>();
      
    }
    // Update is called once per frame
    void Update()
    {
        player = GameManager.Instance.getPlayer();
    }
    public bool isDeath()
    {
        return (dam.getDamage() <= 0);
    }
    //SI EL ENEMIGO PERCIBE ALGO
    public bool isTimePercibir()
    {
        return (player && getPercibir() && !getAtaque() && !getPerseguir());
    }
    //SI EL jugador ESTA EN EL AREA DE VISION  Y TIENE QUE PERSEGUIR
    public bool isTimePerseguir() { return (player && getPerseguir() && !getAtaque() && !isDeath()); }
    //SI EL jugador no ESTA EN EL AREA DE VISION  Y TIENE QUE MERODEAR
    public bool isTimeMerodear()
    {
        return (!getPerseguir() && !getPercibir() && !getAtaque() && !isDeath());
    }
    //SI EL jugador ESTA EN EL AREA DE VISION  Y TIENE QUE ATACR
    public bool isTimeAtacar()
    {
        return (getAtaque() && player && !getPerseguir());
    }
    public void SetAtaque(bool atacando)
    {
        ataque = atacando;
    }
    public void SetPercepcion(bool percebir)
    {
        percibido = percebir;
    }
    public bool getPercibir()
    {
        return percibido;
    }
    public void setPerseguir(bool perseguir)
    {
        perseguir_ = perseguir;
    }
    public bool getPerseguir()
    {
        return perseguir_;
    }
    public bool getAtaque()
    {
        return ataque;
    }
    public void setPlayer(GameObject player_)
    {
        player = player_;
    }
    public GameObject getPlayer() { return player; }
}
