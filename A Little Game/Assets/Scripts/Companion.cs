using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Destroy(this.gameObject);
        }
    }

}
