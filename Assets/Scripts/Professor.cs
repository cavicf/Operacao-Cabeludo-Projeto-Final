using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Professor : MonoBehaviour
{
    Rigidbody2D rb;
    
    float impulsoSubida;
    float gravidadeSubida;
    float gravidadeQueda;

    int pontos;
    [SerializeField] TMP_Text ScoreText;
    void Awake()
    {
        // Pegando a referencia do Rigidbody do professor
        rb = GetComponent<Rigidbody2D>();
        impulsoSubida = 7.5f;
        gravidadeSubida = 2.5f;
        gravidadeQueda = 4;
    }

    void FixedUpdate()
    {
        AjustarGravidade();
    }

    void AjustarGravidade()
    {
        if(rb.linearVelocityY > 0)
        {
            // está subindo
            rb.gravityScale = gravidadeSubida;
        }
        else
        {
            // está caindo
            rb.gravityScale = gravidadeQueda;
        }
    }
    
    void OnJump()
    {
        rb.linearVelocityY = impulsoSubida;
    }
    
    // chamado quando o professor passa pelas maquinas
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ponto"))
        {
            pontos++;
            ScoreText.text = pontos.ToString();
        }
    }
    
    // chamado quando o professor bate no chão ou nas maquinas
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Roof"))
        {
            return;
        }
        
        SceneManager.LoadScene("MainMenu");
    }
}
