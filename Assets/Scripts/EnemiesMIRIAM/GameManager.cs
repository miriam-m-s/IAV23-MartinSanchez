using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject player;
    public static GameManager Instance { get; private set; }

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

    public float tiempoInicial = 60f; // Tiempo inicial en segundos
    public TextMeshProUGUI tiempoText; // Referencia al Text que mostrará el tiempo restante
    public TextMeshProUGUI winText; // Referencia al Text que mostrará si hemos ganado
    public TextMeshProUGUI loseText; // Referencia al Text que mostrará si hemos perdido

    private float tiempoRestante; // Tiempo restante en segundos

    void Start()
    {
        tiempoRestante = tiempoInicial; // Inicializar el tiempo restante al tiempo inicial
    }

    void Update()
    {
        // Actualizar el tiempo restante
        tiempoRestante -= Time.deltaTime;

        // Actualizar el texto del tiempo restante en pantalla
        tiempoText.text = tiempoRestante.ToString("0");
        // Verificar si el tiempo ha llegado a cero
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            // Cambiar a la escena de "GameOver"
            showLoseText();
            Invoke("ReStartScene", 3f);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            // Obtener el índice de la escena actual
            ReStartScene();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
        }
    }

    public void showWinText()
    {
        winText.gameObject.SetActive(true);
    }

    public void showLoseText()
    {
        loseText.gameObject.SetActive(true);
    }

    public void ReStartScene()
    {
        SceneManager.LoadScene("game");
    }
    public GameObject getPlayer() { return player; }
}