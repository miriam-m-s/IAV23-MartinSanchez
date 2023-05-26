using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerEscenas : MonoBehaviour
{
    public static ManagerEscenas instance;
    public static ManagerEscenas Instance { get; private set; }
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

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
