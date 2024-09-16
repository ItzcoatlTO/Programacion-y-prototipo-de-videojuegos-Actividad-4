using UnityEngine;

public class GanarJuego : MonoBehaviour
{
    public GameObject textoGanaste; 

    private void Start()
    {
        if (textoGanaste != null)
        {
            textoGanaste.SetActive(false);  
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            Debug.Log("Colisión detectada con el jugador");

            if (textoGanaste != null)
            {
                Debug.Log("Activando texto '¡Ganaste!'");
                textoGanaste.SetActive(true); 
            }

            Time.timeScale = 0f;  
        }
    }
}