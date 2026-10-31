using UnityEngine;

public class Professor : MonoBehaviour
{
    Rigidbody2D rb;
    
    float impulsoSubida;
    float gravidadeSubida;
    float gravidadeQueda;
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
}
