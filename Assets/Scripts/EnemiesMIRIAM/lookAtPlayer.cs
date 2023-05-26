using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookAtPlayer : MonoBehaviour
{
    // Start is called before the first frame update
     Transform cmaeraaa;
    private void Start()
    {
        cmaeraaa = GameManager.Instance.getPlayer().transform;
    }
    private void LateUpdate()
    {
        transform.LookAt( cmaeraaa );
    }
    // Update is called once per frame
  
}
