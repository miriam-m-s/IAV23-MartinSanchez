using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyBlod : MonoBehaviour
{
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        Destroy(this.gameObject, 1f);
    }
   
}
