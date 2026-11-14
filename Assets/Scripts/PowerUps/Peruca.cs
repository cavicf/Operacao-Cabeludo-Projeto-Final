using UnityEngine;

// Peruca: funciona como um escudo que aguenta uma batida em maquina ou 2 inimigos.
// Na batida a maquina e destruida e o efeito acaba na hora.
// Cada inimigo que encosta gasta uma defesa; sem defesas o efeito acaba.
// Se o professor nao bater em nada, a peruca cai depois da duracao.
// Enquanto isso o cabelo de verdade fica escondido embaixo da peruca.
public class Peruca : IPowerUp
{
    public float Duracao { get { return 10f; } }
    public Color Cor { get { return new Color(1f, 0.85f, 0.3f); } }

    public void Ativar(Professor professor)
    {
        professor.TemEscudo = true;
        professor.DefesasContraInimigos = 2;
        professor.EsconderCabelo = true;
    }

    public void Desativar(Professor professor)
    {
        professor.TemEscudo = false;
        professor.DefesasContraInimigos = 0;

        // a peruca sai e o cabelo volta a aparecer como estava
        professor.EsconderCabelo = false;
    }
}
