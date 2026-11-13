using UnityEngine;
using TMPro;

[System.Serializable]
public class Fase
{
    public string nome;
    public int pontosParaComecar;
    public float vao;
}

public class PhaseController : MonoBehaviour
{
    [SerializeField] Fase[] fases;
    int indiceFase;
    
    [SerializeField] TMP_Text avisoFase;
    [SerializeField] float duracaoAviso = 2f;
    
    // a fase em que o jogo esta agora
    public Fase FaseAtual => fases[indiceFase];

    void Awake()
    {
        // o aviso so aparece quando a fase muda
        avisoFase.gameObject.SetActive(false);
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
            MostrarAviso();
        }
    }
    
    public void MostrarAviso()
    {
        // indiceFase comeca em 0, entao soma 1 para mostrar "Fase 2", "Fase 3"...
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