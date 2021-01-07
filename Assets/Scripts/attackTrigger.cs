using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackTrigger : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.isTrigger != true && col.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Hit");
        }
        if (col.isTrigger != true && !col.CompareTag("Player"))
        {
            Debug.Log("Hit");
        }
    }
}
