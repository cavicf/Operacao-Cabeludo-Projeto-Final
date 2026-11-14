using UnityEngine;

// animacao curta de explosao: troca os sprites em sequencia e some no final
public class Explosao : MonoBehaviour
{
    [SerializeField] Sprite[] quadros;
    [SerializeField] float tempoQuadro = 0.07f;

    SpriteRenderer sr;
    int quadroAtual;
    float tempo;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = quadros[0];
    }

    void Update()
    {
        tempo += Time.deltaTime;
        if (tempo < tempoQuadro)
        {
            return;
        }

        tempo = 0;
        quadroAtual++;

        // acabaram os quadros: a explosao termina
        if (quadroAtual >= quadros.Length)
        {
            Destroy(gameObject);
            return;
        }

        sr.sprite = quadros[quadroAtual];
    }
}
