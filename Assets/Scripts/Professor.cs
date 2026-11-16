using System.Collections;
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

    // recorde da sessao (canto superior direito) e a animacao de novo recorde
    [SerializeField] TMP_Text recordeText;
    [SerializeField] NovoRecordeAnimacao animacaoNovoRecorde;

    // a cabeca explode quando o professor morre
    [SerializeField] ExplosaoCabeca explosaoCabecaPrefab;
    [SerializeField] float esperaDepoisExplosao = 1.2f;
    
    [SerializeField] PhaseController gerenciadorFases;

    // sprites do professor, do careca (0) ate o cabelo maximo (20)
    [SerializeField] Sprite[] calveludo;
    int nivelCabelo;

    // quantos pontos sao necessarios para subir um nivel de cabelo
    public const int PontosPorNivelPadrao = 5;
    public int PontosPorNivel { get; set; } = PontosPorNivelPadrao;

    // pontos acumulados desde o ultimo nivel de cabelo
    // (contador separado para funcionar mesmo quando ganha mais de 1 ponto por vez)
    int contadorNivel;

    // quantos pontos o professor ganha ao passar por um par de maquinas
    public int MultiplicadorPontos { get; set; } = 1;

    // power-up ativo no momento (padrao Strategy) e o tempo que falta para acabar
    IPowerUp powerUpAtual;
    float tempoPowerUp;

    // efeito visual do power-up (bolhas, brilho, peruca) preso no professor
    GameObject efeitoAtual;

    // escudo da peruca: aguenta uma batida em maquina ou 2 encostadas em inimigos
    public bool TemEscudo { get; set; }

    // logo depois da peruca quebrar uma maquina, a outra maquina do mesmo par
    // ainda nao mata (para nao morrer encostando nas duas ao mesmo tempo)
    [SerializeField] float tempoProtecaoPar = 0.3f;
    HairClipperController parQuebrado;
    float fimProtecaoPar;
    public int DefesasContraInimigos { get; set; }

    // ataque: frascos de minoxidil disparados com ENTER (so nas fases com inimigos)
    [SerializeField] GameObject minoxidilPrefab;
    [SerializeField] float intervaloAtaque = 0.35f;
    float tempoUltimoAtaque = -10;

    // com a peruca o cabelo de verdade fica escondido (mostra o careca por baixo)
    bool esconderCabelo;
    public bool EsconderCabelo
    {
        get { return esconderCabelo; }
        set
        {
            esconderCabelo = value;
            AtualizarCabelo();
        }
    }

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
        recordeText.text = GameData.recorde.ToString();

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

        // durante a explicacao dos inimigos o professor nao pula
        if (gerenciadorFases.Pausado)
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
            
            // mostra o aviso da primeira fase
            gerenciadorFases.MostrarAviso();
        }

        rb.linearVelocityY = impulsoSubida;
    }

    // chamado pelo PlayerInput quando aperta ENTER
    void OnAttack()
    {
        // ENTER fecha a explicacao dos inimigos (sem disparar)
        if (gerenciadorFases.Pausado)
        {
            gerenciadorFases.FecharTutorial();

            // zera a queda para o jogador ter tempo de voltar a pular
            rb.linearVelocityY = 0;
            return;
        }

        // so ataca com o jogo rodando e nas fases que tem inimigos
        if (!comecou || morreu || gerenciadorFases.Pausado || !gerenciadorFases.FaseAtual.temInimigos)
        {
            return;
        }

        // um tempinho entre um disparo e outro para nao virar metralhadora
        if (Time.time - tempoUltimoAtaque < intervaloAtaque)
        {
            return;
        }

        tempoUltimoAtaque = Time.time;
        Instantiate(minoxidilPrefab, transform.position + new Vector3(0.6f, 0, 0), Quaternion.identity);
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
            AplicarPowerUp(item.CriarPowerUp(), item.EfeitoNoProfessor);
            Destroy(item.gameObject);
        }
        else if (other.CompareTag("Inimigo"))
        {
            // o inimigo explode ao encostar no professor
            Inimigo inimigo = other.GetComponentInParent<Inimigo>();
            LevarDanoInimigo();
            inimigo.Explodir();
        }
    }

    void LevarDanoInimigo()
    {
        // a peruca aguenta 2 inimigos antes de cair
        if (TemEscudo && DefesasContraInimigos > 0)
        {
            DefesasContraInimigos--;
            if (DefesasContraInimigos == 0)
            {
                DesativarPowerUp();
            }
            return;
        }

        PerderNivel();
    }

    // o cabelo volta 1 nivel, mas a pontuacao continua a mesma
    void PerderNivel()
    {
        if (nivelCabelo > 0)
        {
            nivelCabelo--;
            AtualizarCabelo();
        }

        // pisca avermelhado rapidinho para mostrar que levou dano
        sr.color = new Color(1f, 0.5f, 0.5f);
        CancelInvoke("VoltarCor");
        Invoke("VoltarCor", 0.2f);
    }

    void VoltarCor()
    {
        if (!morreu)
        {
            sr.color = Color.white;
        }
    }

    void GanharPontos(int quantidade)
    {
        pontos += quantidade;
        contadorNivel += quantidade;
        ScoreText.text = pontos.ToString();

        // passou do recorde: o marcador do canto acompanha a pontuacao
        if (pontos > GameData.recorde)
        {
            recordeText.text = pontos.ToString();
        }
        
        // verifica se os pontos ja alcancaram a proxima fase
        gerenciadorFases.AtualizarFase(pontos);

        // a cada "PontosPorNivel" pontos o professor ganha mais cabelo
        while (contadorNivel >= PontosPorNivel)
        {
            contadorNivel -= PontosPorNivel;
            SubirNivel();
        }
    }

    public void AplicarPowerUp(IPowerUp powerUp, GameObject efeitoPrefab = null)
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

        // coloca o efeito visual como filho do professor, para acompanhar ele
        if (efeitoPrefab != null)
        {
            efeitoAtual = Instantiate(efeitoPrefab, transform);
            efeitoAtual.transform.localPosition = Vector3.zero;

            // bolhas e estrelas ficam com a cor do power-up
            EfeitoAura aura = efeitoAtual.GetComponentInChildren<EfeitoAura>();
            if (aura != null)
            {
                aura.DefinirCor(powerUp.Cor);
            }
        }
    }

    public void DesativarPowerUp()
    {
        if (powerUpAtual == null)
        {
            return;
        }

        powerUpAtual.Desativar(this);
        powerUpAtual = null;

        if (efeitoAtual != null)
        {
            Destroy(efeitoAtual);
            efeitoAtual = null;
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
        // com a peruca mostra o careca (0); sem ela, o nivel de cabelo atual
        sr.sprite = esconderCabelo ? calveludo[0] : calveludo[nivelCabelo];
    }

    // chamado quando o professor bate no chão, no teto ou nas maquinas
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (morreu)
        {
            return;
        }

        HairClipperController par = collision.collider.GetComponentInParent<HairClipperController>();

        // com a peruca, a batida em maquina quebra a maquina e gasta o escudo
        if (TemEscudo && par != null)
        {
            QuebrarMaquina(collision);
            DesativarPowerUp();

            parQuebrado = par;
            fimProtecaoPar = Time.time + tempoProtecaoPar;
            return;
        }

        // encostou na outra maquina do mesmo par logo em seguida: quebra ela tambem
        if (par != null && par == parQuebrado && Time.time < fimProtecaoPar)
        {
            QuebrarMaquina(collision);
            return;
        }

        morreu = true;
        gerenciadorFases.CancelarTutorial();
        StartCoroutine(Morrer());
    }

    void QuebrarMaquina(Collision2D collision)
    {
        // a maquina passa para o sprite destruido com animacao (em vez de sumir)
        MaquinaDestruivel maquina = collision.collider.GetComponent<MaquinaDestruivel>();
        if (maquina != null)
        {
            maquina.Destruir(collision.GetContact(0).point);
        }
        else
        {
            Destroy(collision.collider.gameObject);
        }
    }

    IEnumerator Morrer()
    {
        // o jogo entra em camera lenta e a cabeca explode
        Time.timeScale = 0.3f;
        ExplodirCabeca();

        // espera em tempo real, porque o jogo esta em camera lenta
        yield return new WaitForSecondsRealtime(esperaDepoisExplosao);

        // bateu o recorde da sessao: mostra a estrela antes do game over
        if (pontos > GameData.recorde)
        {
            GameData.recorde = pontos;
            yield return animacaoNovoRecorde.Mostrar(pontos);
        }

        GameOver();
    }

    void ExplodirCabeca()
    {
        // tira os efeitos de power-up (bolhas, brilho, peruca) para nao ficarem flutuando
        if (efeitoAtual != null)
        {
            efeitoAtual.SetActive(false);
        }

        ExplosaoCabeca explosao = Instantiate(explosaoCabecaPrefab);
        explosao.Explodir(sr);

        // o professor some: agora so os pedacos da cabeca aparecem
        sr.enabled = false;
        rb.simulated = false;
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
