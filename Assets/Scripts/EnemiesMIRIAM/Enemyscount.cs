using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Enemyscount : MonoBehaviour
{
    // Start is called before the first frame update
    public static Enemyscount instance;
    public static Enemyscount Instance { get; private set; }

    public int enemis = 0;

    public TextMeshProUGUI remainingEnemiesText;

    void Awake()
    {
        // Verificar si ya existe una instancia del GameManager
        if (Instance != null && Instance != this)
        {
            // Si existe, destruir esta instancia para mantener solo una
            Destroy(gameObject);
        }
        else
        {
            // Si no existe, asignar esta instancia como la única
            Instance = this;
        }

        // Mantener el objeto del GameManager en todas las escenas
        // DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        
    }
    private void ReStartScene()
    {
        FindObjectOfType<GameManager>().ReStartScene();
    }

    // Update is called once per frame
    void Update()
    {
        remainingEnemiesText.text = enemis.ToString();

        //hemos ganado
        if (enemis<=0) {
            FindObjectOfType<GameManager>().showWinText();
            Invoke("ReStartScene", 3);
        
        }
    }
}
