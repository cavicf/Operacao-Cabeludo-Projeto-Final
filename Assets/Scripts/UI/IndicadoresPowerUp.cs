using UnityEngine;
using UnityEngine.UI;
using TMPro;

// sinaliza no HUD, ao lado da pontuacao, os power-ups que mudam a pontuacao/cabelo:
// - Shampoo: um "2x" azul (pontos em dobro)
// - Tonico: frasco + seta verde e uma barrinha que enche a cada ponto;
//   quando enche, o cabelo cresce (com o tonico ela tem so 2 quadradinhos em vez de 5)
// Os indicadores aparecem com um "pulo", e piscam nos ultimos segundos do efeito.
public class IndicadoresPowerUp : MonoBehaviour
{
    [SerializeField] Professor professor;

    // os indicadores ficam logo depois do numero da pontuacao
    [SerializeField] RectTransform placar;
    [SerializeField] TMP_Text textoPlacar;
    [SerializeField] float distanciaDoPlacar = 24f;

    [SerializeField] RectTransform indicadorShampoo;
    [SerializeField] RectTransform indicadorTonico;

    // quadradinhos da barra de crescimento do tonico
    [SerializeField] Image[] segmentos;
    [SerializeField] Sprite segmentoCheio;
    [SerializeField] Sprite segmentoVazio;

    [SerializeField] float tempoPiscando = 2f;

    RectTransform grupo;
    bool shampooAntes;
    bool tonicoAntes;
    float tempoEntradaShampoo = 10;
    float tempoEntradaTonico = 10;
    int contadorAntes;
    float tempoPulso = 10;

    void Awake()
    {
        grupo = (RectTransform)transform;
        indicadorShampoo.gameObject.SetActive(false);
        indicadorTonico.gameObject.SetActive(false);
    }

    void Update()
    {
        // acompanha o tamanho do numero da pontuacao (1, 10, 100...)
        float larguraNumero = textoPlacar.preferredWidth;
        grupo.anchoredPosition = new Vector2(placar.anchoredPosition.x + larguraNumero / 2 + distanciaDoPlacar, placar.anchoredPosition.y);

        bool shampoo = professor.MultiplicadorPontos > 1;
        bool tonico = professor.PontosPorNivel < Professor.PontosPorNivelPadrao;

        if (shampoo && !shampooAntes) tempoEntradaShampoo = 0;
        if (tonico && !tonicoAntes) tempoEntradaTonico = 0;
        shampooAntes = shampoo;
        tonicoAntes = tonico;

        tempoEntradaShampoo += Time.unscaledDeltaTime;
        tempoEntradaTonico += Time.unscaledDeltaTime;

        Mostrar(indicadorShampoo, shampoo, tempoEntradaShampoo);
        Mostrar(indicadorTonico, tonico, tempoEntradaTonico);

        if (tonico)
        {
            AtualizarBarra();
        }
    }

    void Mostrar(RectTransform indicador, bool ativo, float tempoDesdeEntrada)
    {
        indicador.gameObject.SetActive(ativo);
        if (!ativo)
        {
            return;
        }

        // entrada com "pulo": cresce, passa um pouco do tamanho e volta (em degraus)
        float escala = 1;
        if (tempoDesdeEntrada < 0.3f)
        {
            float t = tempoDesdeEntrada / 0.3f;
            escala = Mathf.Round(Mathf.Sin(t * Mathf.PI * 0.75f) * 1.35f * 8) / 8;
        }
        indicador.localScale = Vector3.one * escala;

        // pisca nos ultimos segundos para avisar que o efeito vai acabar
        float restante = professor.TempoRestantePowerUp;
        bool visivel = restante > tempoPiscando || Mathf.Repeat(restante, 0.3f) > 0.12f;
        foreach (Graphic g in indicador.GetComponentsInChildren<Graphic>(true))
        {
            g.enabled = visivel;
        }
    }

    void AtualizarBarra()
    {
        int total = professor.PontosPorNivel;
        int cheios = professor.ContadorNivel;

        // a barra zerou: o cabelo acabou de crescer, entao o frasco da um pulinho
        if (cheios < contadorAntes)
        {
            tempoPulso = 0;
        }
        contadorAntes = cheios;

        for (int i = 0; i < segmentos.Length; i++)
        {
            segmentos[i].gameObject.SetActive(i < total);
            segmentos[i].sprite = i < cheios ? segmentoCheio : segmentoVazio;
        }

        tempoPulso += Time.unscaledDeltaTime;
        float pulo = tempoPulso < 0.25f ? 1.25f : 1f;
        indicadorTonico.GetChild(0).localScale = Vector3.one * pulo;
    }
}
