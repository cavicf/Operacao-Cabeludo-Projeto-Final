using UnityEngine;

// Shampoo: o professor ganha pontos em dobro enquanto o efeito dura
public class Shampoo : IPowerUp
{
    public float Duracao { get { return 8f; } }
    public Color Cor { get { return new Color(0.7f, 0.9f, 1f); } }

    public void Ativar(Professor professor)
    {
        professor.MultiplicadorPontos = 2;
    }

    public void Desativar(Professor professor)
    {
        professor.MultiplicadorPontos = 1;
    }
}
