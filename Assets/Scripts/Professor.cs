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

    // pontos acumulados desde o ultimo nivel de cabelo
    // (contador separado para funcionar mesmo quando ganha mais de 1 ponto por vez)
    int contadorNivel;

    // quantos pontos o professor ganha ao passar por um par de maquinas
    public int MultiplicadorPontos { get; set; } = 1;

    // power-up ativo no momento (padrao Strategy) e o tempo que falta para acabar
    IPowerUp powerUpAtual;
    float tempoPowerUp;

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

    void Update()
    {
        ContarTempoPowerUp();
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
        if (morreu)
        {
            return;
        }

        if (other.CompareTag("Ponto"))
        {
            GanharPontos(MultiplicadorPontos);
        }
        else if (other.CompareTag("PowerUp"))
        {
            // pega a estrategia do item e some com ele da tela
            PowerUpItem item = other.GetComponentInParent<PowerUpItem>();
            AplicarPowerUp(item.CriarPowerUp());
            Destroy(item.gameObject);
        }
    }

    void GanharPontos(int quantidade)
    {
        pontos += quantidade;
        contadorNivel += quantidade;
        ScoreText.text = pontos.ToString();

        // a cada "pontosPorNivel" pontos o professor ganha mais cabelo
        while (contadorNivel >= pontosPorNivel)
        {
            contadorNivel -= pontosPorNivel;
            SubirNivel();
        }
    }

    public void AplicarPowerUp(IPowerUp powerUp)
    {
        if (powerUp == null)
        {
            return;
        }

        // so um power-up por vez: o novo substitui o antigo
        DesativarPowerUp();

        powerUpAtual = powerUp;
        tempoPowerUp = powerUp.Duracao;
        powerUpAtual.Ativar(this);
        sr.color = powerUp.Cor;
    }

    public void DesativarPowerUp()
    {
        if (powerUpAtual == null)
        {
            return;
        }

        powerUpAtual.Desativar(this);
        powerUpAtual = null;

        if (!morreu)
        {
            sr.color = Color.white;
        }
    }

    void ContarTempoPowerUp()
    {
        if (powerUpAtual == null || morreu)
        {
            return;
        }

        tempoPowerUp -= Time.deltaTime;
        if (tempoPowerUp <= 0)
        {
            DesativarPowerUp();
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
