using UnityEngine;

// Peruca: funciona como um escudo que aguenta uma batida em maquina.
// Na batida a maquina e destruida e o efeito acaba na hora.
// Se o professor nao bater em nada, a peruca cai depois da duracao.
public class Peruca : IPowerUp
{
    public float Duracao { get { return 10f; } }
    public Color Cor { get { return new Color(1f, 0.85f, 0.3f); } }

    public void Ativar(Professor professor)
    {
        professor.TemEscudo = true;
    }

    public void Desativar(Professor professor)
    {
        professor.TemEscudo = false;
    }
}
