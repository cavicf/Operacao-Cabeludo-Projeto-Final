using UnityEngine;

// Tonico capilar: o cabelo do professor cresce mais rapido,
// subindo um nivel a cada 2 pontos enquanto o efeito dura
public class Tonico : IPowerUp
{
    public float Duracao { get { return 8f; } }
    public Color Cor { get { return new Color(0.5f, 1f, 0.5f); } }

    public void Ativar(Professor professor)
    {
        professor.PontosPorNivel = 2;
    }

    public void Desativar(Professor professor)
    {
        professor.PontosPorNivel = Professor.PontosPorNivelPadrao;
    }
}
