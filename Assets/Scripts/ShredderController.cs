using UnityEngine;

public class ShredderController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // destroi as maquinas
        Destroy(other.transform.parent.gameObject);
    }
}