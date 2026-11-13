using UnityEngine;

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

    // a fase em que o jogo esta agora
    public Fase FaseAtual => fases[indiceFase];

    // chamado pelo professor sempre que ele ganha pontos
    public void AtualizarFase(int pontos)
    {
        // avanca enquanto existir proxima fase e os pontos ja chegaram nela
        while (indiceFase < fases.Length - 1 && pontos >= fases[indiceFase + 1].pontosParaComecar)
        {
            indiceFase++;
        }
    }
}