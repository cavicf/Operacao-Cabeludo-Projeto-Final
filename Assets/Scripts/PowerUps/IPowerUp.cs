using UnityEngine;

// Padrao Strategy: cada power-up (shampoo, tonico, peruca) e uma estrategia
// diferente que o Professor executa sem precisar saber qual e.
public interface IPowerUp
{
    // quanto tempo (em segundos) o efeito dura
    float Duracao { get; }

    // cor do professor enquanto o efeito esta ativo
    Color Cor { get; }

    // liga o efeito no professor
    void Ativar(Professor professor);

    // desliga o efeito e volta o professor ao normal
    void Desativar(Professor professor);
}
