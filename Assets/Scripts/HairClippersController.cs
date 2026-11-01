using UnityEngine;

public class HairClipperController : MonoBehaviour
{
    Rigidbody2D rb;
    float velocityX = 3.8f;

    void Awake()
    {
        // pega a referencia do RigidBody das maquinas
        rb = GetComponent<Rigidbody2D>();

        // aplica a velocidade para a esquerda
        rb.linearVelocityX = -velocityX;
    }
}