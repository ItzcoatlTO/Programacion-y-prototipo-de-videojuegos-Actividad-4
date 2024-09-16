using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoHongo : MonoBehaviour
{
    [SerializeField] private float velocidad = 1f;
    [SerializeField] private Transform Suelo;
    [SerializeField] private float distanciaSuelo = 0.5f;
    [SerializeField] private float distanciaPared = 1.0f;
    [SerializeField] private bool movimientoDerecha = true;
    [SerializeField] private float fuerzaRebote = 5f;  
    [SerializeField] private float intervaloDano = 1f;  

    private Rigidbody2D rb;
    private bool jugadorEnContacto = false;
    private bool aplastado = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
    }

    private void FixedUpdate()
    {
        MoverHongo();
    }

    private void MoverHongo()
    {
        RaycastHit2D InfromacionSuelo = Physics2D.Raycast(Suelo.position, Vector2.down, distanciaSuelo);
        
        Vector2 direccion = movimientoDerecha ? Vector2.right : Vector2.left;
        RaycastHit2D InfromacionPared = Physics2D.Raycast(Suelo.position, direccion, distanciaPared);

        if (InfromacionPared.collider != null && !InfromacionPared.collider.CompareTag("Player"))
        {
            Girar();
        }

        rb.velocity = new Vector2(velocidad, rb.velocity.y);
    }

    private void Girar()
    {
        movimientoDerecha = !movimientoDerecha;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        velocidad *= -1;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetContact(0).normal.y <= -0.5f)
            {
                aplastado = true;
                other.gameObject.GetComponent<Movimiento>().Rebote(); 
            }
            else
            {
                jugadorEnContacto = true;
                StartCoroutine(InfligirDanoYDesaparecer(other.gameObject));

                Vector2 direccionRebote = other.transform.position.x < transform.position.x ? new Vector2(-1f, 1f) : new Vector2(1f, 1f);
                Rigidbody2D jugadorRb = other.gameObject.GetComponent<Rigidbody2D>();
                jugadorRb.velocity = Vector2.zero;  
                jugadorRb.AddForce(direccionRebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            jugadorEnContacto = false;
            aplastado = false;
        }
    }

    private IEnumerator InfligirDanoYDesaparecer(GameObject jugador)
    {
        int vidasRestar = 2;  

        while (jugadorEnContacto && !aplastado && vidasRestar > 0)
        {
            GameManager.instance.PerderVida();
            vidasRestar--;

            yield return new WaitForSeconds(intervaloDano);

            if (vidasRestar == 0)
            {
               
                Destroy(gameObject);
            }
        }
    }
}
