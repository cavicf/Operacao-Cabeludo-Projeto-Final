using UnityEngine;

// inimigo (piolho ou navalha) que atravessa a tela da direita para a esquerda.
// se encostar no professor o cabelo dele volta 1 nivel;
// se for atingido por um frasco de minoxidil ele explode
public class Inimigo : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    [SerializeField] float velocidade = 5f;

    // sobe e desce enquanto anda (piolho). Com 0 anda reto
    [SerializeField] float amplitudeOnda = 0f;
    [SerializeField] float velocidadeOnda = 4f;
    float tempo;

    // gira enquanto anda (navalha). Com 0 nao gira
    [SerializeField] float velocidadeGiro = 0f;

    // quadros da animacao (piolho mexendo as pernas)
    [SerializeField] Sprite[] quadros;
    [SerializeField] float tempoQuadro = 0.15f;
    int quadroAtual;
    float tempoAnimacao;

    [SerializeField] GameObject explosaoPrefab;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // anda para a esquerda, como as maquinas, so que mais rapido
        rb.linearVelocityX = -velocidade;
        rb.angularVelocity = velocidadeGiro;
    }

    void Update()
    {
        if (amplitudeOnda > 0)
        {
            // velocidade em Y seguindo uma onda: o inimigo sobe e desce suavemente
            tempo += Time.deltaTime;
            rb.linearVelocityY = Mathf.Cos(tempo * velocidadeOnda) * amplitudeOnda * velocidadeOnda;
        }

        Animar();
    }

    void Animar()
    {
        if (quadros.Length < 2)
        {
            return;
        }

        // troca o sprite a cada "tempoQuadro" segundos
        tempoAnimacao += Time.deltaTime;
        if (tempoAnimacao >= tempoQuadro)
        {
            tempoAnimacao = 0;
            quadroAtual = (quadroAtual + 1) % quadros.Length;
            sr.sprite = quadros[quadroAtual];
        }
    }

    // chamado quando o frasco acerta o inimigo ou quando ele encosta no professor
    public void Explodir()
    {
        Instantiate(explosaoPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
