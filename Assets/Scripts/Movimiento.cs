using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody2D rigidbody;
    private bool MirandoDeercha = true;
    public float fuerzaSalto = 10f;
    public float fuerzaGolpe = 2f;  
    private BoxCollider2D boxCollider2D;
    public LayerMask CapaSuelo;
    public int SaltosMaximos = 2;
    public int SaltosRestantes;
    private Animator animator;
    private bool puedeMoverse = true;

    [Header("Rebote")]
    [SerializeField] private float velocidadRebote = 3f;  
    [SerializeField] private float velocidadMaxima = 7f;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.mass = 1f;  
        boxCollider2D = GetComponent<BoxCollider2D>();
        SaltosRestantes = SaltosMaximos;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ProcesarMovimiento();
        ProcesarSalto();
    }

    bool EstaEnSuelo()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider2D.bounds.center,
            new Vector2(boxCollider2D.bounds.size.x, boxCollider2D.bounds.size.y),
            0f,
            Vector2.down,
            0.2f,
            CapaSuelo
        );
        return hit.collider != null;
    }

    void ProcesarSalto()
    {
        if (EstaEnSuelo())
        {
            SaltosRestantes = SaltosMaximos;
        }
        if (Input.GetKeyDown(KeyCode.Space) && SaltosRestantes > 0)
        {
            SaltosRestantes--;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0f);
            rigidbody.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
    }

    void ProcesarMovimiento()
    {
        if (!puedeMoverse)
        {
            return;
        }
        float InputMovimiento = Input.GetAxis("Horizontal");
        animator.SetBool("IsRunnig", InputMovimiento != 0);

        rigidbody.velocity = new Vector2(InputMovimiento * velocidad, rigidbody.velocity.y);
        Orientacion(InputMovimiento);
    }

    void Orientacion(float InputMovimiento)
    {
        if ((MirandoDeercha && InputMovimiento < 0) || (!MirandoDeercha && InputMovimiento > 0))
        {
            MirandoDeercha = !MirandoDeercha;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    public void AplicarGolpe()
    {
        puedeMoverse = false;
        Vector2 direccionGolpe = rigidbody.velocity.x > 0 ? new Vector2(-1, 1) : new Vector2(1, 1);

        rigidbody.velocity = Vector2.zero;  
        rigidbody.AddForce(direccionGolpe * fuerzaGolpe, ForceMode2D.Impulse);
        LimitarVelocidad();  
        StartCoroutine(EsperarYActivarMovimiento());
    }

    IEnumerator EsperarYActivarMovimiento()
    {
        yield return new WaitForSeconds(0.2f); 
        while (!EstaEnSuelo())
        {
            yield return null;
        }
        puedeMoverse = true;
    }

    public void Rebote()
    {
        rigidbody.velocity = Vector2.zero;  
        rigidbody.AddForce(new Vector2(0, velocidadRebote), ForceMode2D.Impulse);  
        LimitarVelocidad();  
    }

    private void LimitarVelocidad()
    {
        // Limita la velocidad en ambas direcciones (horizontal y vertical)
        rigidbody.velocity = new Vector2(
            Mathf.Clamp(rigidbody.velocity.x, -velocidadMaxima, velocidadMaxima),
            Mathf.Clamp(rigidbody.velocity.y, -velocidadMaxima, velocidadMaxima)
        );
    }
}

