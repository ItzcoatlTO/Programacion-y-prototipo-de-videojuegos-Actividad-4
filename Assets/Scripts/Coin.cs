using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Coin : MonoBehaviour
{
    public int valor = 1; 
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.SumarPuntos(valor);
            Destroy(this.gameObject);

        }

    }
}
