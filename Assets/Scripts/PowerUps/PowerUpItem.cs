using UnityEngine;

// tipos de power-up que podem ser escolhidos no Inspector
public enum TipoPowerUp
{
}

// item coletavel que aparece entre as maquinas
public class PowerUpItem : MonoBehaviour
{
    [SerializeField] TipoPowerUp tipo;

    // cria a estrategia correspondente ao tipo escolhido no Inspector
    public IPowerUp CriarPowerUp()
    {
        switch (tipo)
        {
            default:
                return null;
        }
    }
}
