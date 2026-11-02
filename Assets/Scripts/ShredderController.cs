using UnityEngine;

public class ShredderController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // so destroi objetos que se movem (maquinas), ignorando teto e chao
        if (other.attachedRigidbody != null)
        {
            // destroi o objeto inteiro (o pai com o Rigidbody)
            Destroy(other.attachedRigidbody.gameObject);
        }
    }
}