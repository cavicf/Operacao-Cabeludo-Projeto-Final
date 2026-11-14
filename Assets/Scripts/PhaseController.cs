using UnityEngine;
using TMPro;

[System.Serializable]
public class Fase
{
    public string nome;
    public int pontosParaComecar;
    public float vao;
    // marca se as maquinas abrem e fecham nessa fase
    public bool maquinasMovem;
    // marca se piolhos e navalhas aparecem nessa fase (e se o professor pode atacar)
    public bool temInimigos;
}

public class PhaseController : MonoBehaviour
{
    [SerializeField] Fase[] fases;
    int indiceFase;
    
    [SerializeField] TMP_Text avisoFase;
    [SerializeField] float duracaoAviso = 3f;

    // caixa que explica os inimigos na primeira fase em que eles aparecem
    [SerializeField] GameObject painelTutorialInimigos;
    bool tutorialMostrado;

    // jogo parado esperando o jogador ler a explicacao
    public bool Pausado { get; private set; }
    
    // a fase em que o jogo esta agora
    public Fase FaseAtual => fases[indiceFase];

    void Awake()
    {
        // o aviso so aparece quando a fase muda
        avisoFase.gameObject.SetActive(false);
        painelTutorialInimigos.SetActive(false);
    }

    // chamado pelo professor sempre que ele ganha pontos
    public void AtualizarFase(int pontos)
    {
        int faseAnterior = indiceFase;
        
        // avanca enquanto existir proxima fase e os pontos ja chegaram nela
        while (indiceFase < fases.Length - 1 && pontos >= fases[indiceFase + 1].pontosParaComecar)
        {
            indiceFase++;
        }
        
        // so mostra o aviso se a fase realmente mudou
        if (indiceFase != faseAnterior)
        {
            // na primeira fase com inimigos o jogo pausa para explicar as novidades
            if (FaseAtual.temInimigos && !tutorialMostrado)
            {
                MostrarTutorial();
            }
            else
            {
                MostrarAviso();
            }
        }
    }

    void MostrarTutorial()
    {
        tutorialMostrado = true;
        Pausado = true;
        painelTutorialInimigos.SetActive(true);

        // para o tempo do jogo (fisica, maquinas e inimigos)
        Time.timeScale = 0;
    }

    // chamado pelo professor quando o jogador aperta SPACE
    public void FecharTutorial()
    {
        Pausado = false;
        painelTutorialInimigos.SetActive(false);
        Time.timeScale = 1;

        MostrarAviso();
    }
    
    public void MostrarAviso()
    {
        // indiceFase comeca em 0
        avisoFase.text = "Fase " + (indiceFase + 1) + "\n" + FaseAtual.nome;
        avisoFase.gameObject.SetActive(true);
        
        // reinicia o tempo, caso um aviso anterior ainda esteja na tela
        CancelInvoke("EsconderAviso");
        Invoke("EsconderAviso", duracaoAviso);
    }
    
    void EsconderAviso()
    {
        avisoFase.gameObject.SetActive(false);
    }
}