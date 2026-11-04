using UnityEngine;

// tipos de power-up que podem ser escolhidos no Inspector
public enum TipoPowerUp
{
    Shampoo,
    Tonico,
    Peruca,
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
            case TipoPowerUp.Shampoo:
                return new Shampoo();
            case TipoPowerUp.Tonico:
                return new Tonico();
            case TipoPowerUp.Peruca:
                return new Peruca();
            default:
                return null;
        }
    }
}
