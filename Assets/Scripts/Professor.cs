using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Professor : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    float impulsoSubida;
    float gravidadeSubida;
    float gravidadeQueda;

    int pontos;
    [SerializeField] TMP_Text ScoreText;

    // sprites do professor, do careca (0) ate o cabelo maximo (20)
    [SerializeField] Sprite[] calveludo;
    int nivelCabelo;
    int pontosPorNivel = 5;

    // tela de "prepare-se": o jogo so comeca no primeiro pulo
    bool comecou;
    [SerializeField] GameObject textoInicio;
    [SerializeField] HairClippersSpawner spawner;

    bool morreu;

    void Awake()
    {
        // Pegando a referencia do Rigidbody do professor
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        impulsoSubida = 7.5f;
        gravidadeSubida = 2.5f;
        gravidadeQueda = 4;
        pontos = 0;
        nivelCabelo = 0;
        AtualizarCabelo();

        // professor parado no ar ate o jogador apertar espaco
        rb.simulated = false;
        spawner.enabled = false;
        textoInicio.SetActive(true);
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
        if (morreu)
        {
            return;
        }

        if (!comecou)
        {
            // primeiro pulo: liga a fisica e comeca a gerar maquinas
            comecou = true;
            rb.simulated = true;
            spawner.enabled = true;
            textoInicio.SetActive(false);
        }

        rb.linearVelocityY = impulsoSubida;
    }

    // chamado quando o professor passa pelas maquinas
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ponto") && !morreu)
        {
            pontos++;
            ScoreText.text = pontos.ToString();

            // a cada 5 pontos o professor ganha mais cabelo
            if (pontos % pontosPorNivel == 0)
            {
                SubirNivel();
            }
        }
    }

    void SubirNivel()
    {
        // nao passa do ultimo sprite
        if (nivelCabelo < calveludo.Length - 1)
        {
            nivelCabelo++;
            AtualizarCabelo();
        }
    }

    void AtualizarCabelo()
    {
        sr.sprite = calveludo[nivelCabelo];
    }

    // chamado quando o professor bate no chão, no teto ou nas maquinas
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (morreu)
        {
            return;
        }

        // feedback da batida: fica vermelho e o jogo entra em camera lenta
        morreu = true;
        sr.color = Color.red;
        Time.timeScale = 0.3f;

        // Invoke conta o tempo do jogo, entao 0.3s aqui sao ~1s de verdade
        Invoke("GameOver", 0.3f);
    }

    void GameOver()
    {
        // volta o tempo ao normal antes de trocar de cena
        Time.timeScale = 1;

        GameData.jogou = true;
        GameData.pontuacao = pontos;

        SceneManager.LoadScene("MainMenu");
    }
}
