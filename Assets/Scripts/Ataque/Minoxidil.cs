using UnityEngine;

// frasco de minoxidil disparado pelo professor: voa para a direita e explode o inimigo que acertar
public class Minoxidil : MonoBehaviour
{
    [SerializeField] float velocidade = 9f;

    // depois desse tempo o frasco ja saiu da tela e pode ser destruido
    [SerializeField] float tempoDeVida = 2f;

    void Awake()
    {
        GetComponent<Rigidbody2D>().linearVelocityX = velocidade;
        Destroy(gameObject, tempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ignora maquinas, power-ups e o proprio professor
        if (other.CompareTag("Inimigo"))
        {
            other.GetComponentInParent<Inimigo>().Explodir();
            Destroy(gameObject);
        }
    }
}
