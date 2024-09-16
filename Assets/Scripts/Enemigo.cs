using System.Collections;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [SerializeField] private float fuerzaRebote = 8f;  
    [SerializeField] private float intervaloDano = 1f;
    private bool jugadorEnContacto = false;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;  
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            jugadorEnContacto = true;
            StartCoroutine(InfligirDanoContinuo(other.gameObject));

            
            Vector2 direccionRebote = DeterminarDireccionRebote(other);

            Rigidbody2D jugadorRb = other.gameObject.GetComponent<Rigidbody2D>();
            jugadorRb.velocity = Vector2.zero;  // Resetea la velocidad actual
            jugadorRb.AddForce(direccionRebote * fuerzaRebote, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            jugadorEnContacto = false;
        }
    }

    private Vector2 DeterminarDireccionRebote(Collision2D other)
    {
        Vector2 direccionRebote;

        if (Mathf.Abs(other.transform.position.x - transform.position.x) > Mathf.Abs(other.transform.position.y - transform.position.y))
        {
            if (other.transform.position.x < transform.position.x)
            {
                direccionRebote = new Vector2(-1f, 0f); 
            }
            else
            {
                direccionRebote = new Vector2(1f, 0f); 
            }
        }
        else
        {
            direccionRebote = new Vector2(0f, 1f); 
        }

        return direccionRebote;
    }

    private IEnumerator InfligirDanoContinuo(GameObject jugador)
    {
        while (jugadorEnContacto)
        {
            GameManager.instance.PerderVida();
            yield return new WaitForSeconds(intervaloDano);  
        }
    }
}
