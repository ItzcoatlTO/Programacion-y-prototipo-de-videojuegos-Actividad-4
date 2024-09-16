using System.Collections;
using UnityEngine;

public class BolaDeNieve : MonoBehaviour
{
    [SerializeField] private float velocidadCaida = 5f;  
    [SerializeField] private float fuerzaRebote = 8f;  
    [SerializeField] private float intervaloDano = 1f;
    [SerializeField] private Transform detectorSuelo;  
    [SerializeField] private int vidasQueQuita = 2; 

    private Rigidbody2D rb;
    private bool jugadorEnContacto = false;
    private Vector3 posicionInicial;  

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = velocidadCaida;  
        posicionInicial = transform.position;  

        rb.mass = 1000f;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(detectorSuelo.position, Vector2.down, 0.1f);
        if (hit.collider != null && hit.collider.gameObject != gameObject)  
        {
            ReiniciarPosicion();  
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Movimiento>() != null) 
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
        if (other.gameObject.GetComponent<Movimiento>() != null)
        {
            jugadorEnContacto = false;
        }
    }

    private void ReiniciarPosicion()
    {
        transform.position = posicionInicial;
        rb.velocity = Vector2.zero;  
    }

    private Vector2 DeterminarDireccionRebote(Collision2D other)
    {
        Vector2 direccionRebote;

        if (other.transform.position.y < transform.position.y)
        {
            direccionRebote = new Vector2(0f, 1f); 
        }
        else
        {
            direccionRebote = new Vector2(0f, -1f);  
        }

        return direccionRebote;
    }

    private IEnumerator InfligirDanoContinuo(GameObject jugador)
    {
        int vidasQuitadas = 0;

        while (jugadorEnContacto && vidasQuitadas < vidasQueQuita)
        {
    
            GameManager.instance.PerderVida();
            vidasQuitadas++;

            if (vidasQuitadas >= vidasQueQuita)
            {
                Destroy(gameObject); 
            }

            yield return new WaitForSeconds(intervaloDano);  
        }
    }
}
