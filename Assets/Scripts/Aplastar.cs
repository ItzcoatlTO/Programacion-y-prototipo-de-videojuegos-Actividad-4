using System.Collections.Generic;
using UnityEngine;

public class Aplastar : MonoBehaviour
{
    [SerializeField] private GameObject efecto;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetContact(0).normal.y <= -0.5f)  
            {
                other.gameObject.GetComponent<Movimiento>().Rebote();
                Golpe();
            }
            else
            {
                GameManager.instance.PerderVida();
                other.gameObject.GetComponent<Movimiento>().AplicarGolpe();
            }
        }
    }

    public void Golpe()
    {
        if (efecto != null)
        {
            Instantiate(efecto, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}