using System.Collections;
using UnityEngine;

public class Fantasma : MonoBehaviour
{
    public float radioBusqueda = 5f;
    public Transform transformJugador;

    public float velocidadMovimiento = 2f;
    public float distanciaMaxima = 10f;
    public Vector3 puntoInicial;

    private Rigidbody2D rb;
    private bool jugadorEnContacto = false;
    private Coroutine dañoCoroutine;

    public enum EstadoMovimiento
    {
        Esperando,
        Siguiendo,
        Volviendo,
    }

    public EstadoMovimiento estadoActual;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        puntoInicial = transform.position;
    }

    private void Update()
    {
        switch (estadoActual)
        {
            case EstadoMovimiento.Esperando:
                EstadoEsperando();
                break;
            case EstadoMovimiento.Siguiendo:
                EstadoSiguiendo();
                break;
            case EstadoMovimiento.Volviendo:
                EstadoVolviendo();
                break;
        }
    }

    private void EstadoEsperando()
    {
        Collider2D jugadorCollider = Physics2D.OverlapCircle(transform.position, radioBusqueda);
        if (jugadorCollider != null && jugadorCollider.CompareTag("Player"))
        {
            transformJugador = jugadorCollider.transform;
            estadoActual = EstadoMovimiento.Siguiendo;
        }
    }

    private void EstadoSiguiendo()
    {
        if (transformJugador == null)
        {
            estadoActual = EstadoMovimiento.Volviendo;
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, transformJugador.position, velocidadMovimiento * Time.deltaTime);

        if (Vector2.Distance(transform.position, transformJugador.position) > distanciaMaxima)
        {
            estadoActual = EstadoMovimiento.Volviendo;
            transformJugador = null;
        }
    }

    private void EstadoVolviendo()
    {
        transform.position = Vector2.MoveTowards(transform.position, puntoInicial, velocidadMovimiento * Time.deltaTime);

        if (Vector2.Distance(transform.position, puntoInicial) < 0.1f)
        {
            estadoActual = EstadoMovimiento.Esperando;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugadorEnContacto = true;
            if (dañoCoroutine == null)
            {
                dañoCoroutine = StartCoroutine(InfligirDanoContinuo());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugadorEnContacto = false;
            if (dañoCoroutine != null)
            {
                StopCoroutine(dañoCoroutine);
                dañoCoroutine = null;
            }
        }
    }

    private IEnumerator InfligirDanoContinuo()
    {
        while (jugadorEnContacto)
        {
            GameManager.instance.PerderVida();  
            yield return new WaitForSeconds(1f);  
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioBusqueda);
        Gizmos.DrawWireSphere(puntoInicial, distanciaMaxima);
    }
}
