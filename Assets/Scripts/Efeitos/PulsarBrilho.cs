using UnityEngine;

// faz o brilho dourado atras dos power-ups pulsar (em degraus, estilo pixel art)
public class PulsarBrilho : MonoBehaviour
{
    [SerializeField] float velocidade = 5f;
    [SerializeField] float alphaMin = 0.4f;
    [SerializeField] float alphaMax = 1f;

    SpriteRenderer sr;
    Color corOriginal;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        corOriginal = sr.color;
    }

    void Update()
    {
        float onda = (Mathf.Sin(Time.time * velocidade) + 1) / 2;
        float degrau = Mathf.Round(onda * 3) / 3;

        Color c = corOriginal;
        c.a = corOriginal.a * Mathf.Lerp(alphaMin, alphaMax, degrau);
        sr.color = c;
    }
}
