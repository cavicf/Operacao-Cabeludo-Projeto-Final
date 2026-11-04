using System.Collections.Generic;
using UnityEngine;

// Cria pequenos sprites em volta do objeto pai (professor ou item):
// - Subir: bolhas de sabao que sobem balancando e estouram
// - Piscar: estrelinhas que aparecem, crescem e somem no lugar
public class EfeitoAura : MonoBehaviour
{
    public enum Movimento { Subir, Piscar }

    [SerializeField] Movimento movimento;
    [SerializeField] Sprite[] sprites;
    [SerializeField] int quantidade = 6;
    [SerializeField] float raio = 1.5f;
    [SerializeField] float vidaParticula = 1.2f;
    [SerializeField] float velocidadeSubida = 1f;
    [SerializeField] Color cor = Color.white;

    // quantas posicoes na frente do sprite do pai as particulas ficam
    [SerializeField] int ordemNaFrente = 1;

    class Particula
    {
        public Transform transform;
        public SpriteRenderer sr;
        public float tempo;
        public float vida;
        public float fase;
        public Vector2 inicio;
    }

    List<Particula> particulas = new List<Particula>();

    void Start()
    {
        // usa a mesma camada de desenho do pai para aparecer junto com ele
        SpriteRenderer pai = GetComponentInParent<SpriteRenderer>();

        for (int i = 0; i < quantidade; i++)
        {
            GameObject go = new GameObject("Particula");
            go.transform.SetParent(transform, false);

            Particula p = new Particula();
            p.transform = go.transform;
            p.sr = go.AddComponent<SpriteRenderer>();
            if (pai != null)
            {
                p.sr.sortingLayerID = pai.sortingLayerID;
                p.sr.sortingOrder = pai.sortingOrder + ordemNaFrente;
            }

            Reiniciar(p);

            // cada particula comeca num momento diferente para nao nascerem todas juntas
            p.tempo = -Random.Range(0, p.vida);
            p.sr.enabled = false;
            particulas.Add(p);
        }
    }

    // a cor e definida pela estrategia do power-up (IPowerUp.Cor)
    public void DefinirCor(Color novaCor)
    {
        cor = novaCor;
    }

    void Update()
    {
        foreach (Particula p in particulas)
        {
            p.tempo += Time.deltaTime;

            if (p.tempo < 0)
            {
                continue;
            }

            if (p.tempo >= p.vida)
            {
                Reiniciar(p);
            }

            p.sr.enabled = true;
            float progresso = p.tempo / p.vida;
            Color c = cor;

            if (movimento == Movimento.Subir)
            {
                // sobe balancando de um lado para o outro
                float balanco = Mathf.Sin(p.tempo * 5 + p.fase) * 0.12f;
                p.transform.localPosition = p.inicio + new Vector2(balanco, velocidadeSubida * p.tempo);
                p.transform.localScale = Vector3.one;

                // no final a bolha "estoura": cresce um pouco e some
                if (progresso > 0.85f)
                {
                    p.transform.localScale = Vector3.one * 1.3f;
                    c.a *= 0.5f;
                }
            }
            else
            {
                // cresce e diminui em degraus, para parecer animacao de pixel art
                float brilho = Mathf.Sin(progresso * Mathf.PI);
                float degrau = Mathf.Ceil(brilho * 3) / 3;
                p.transform.localPosition = p.inicio;
                p.transform.localScale = Vector3.one * degrau;
            }

            p.sr.color = c;
        }
    }

    void Reiniciar(Particula p)
    {
        p.tempo = 0;
        p.vida = vidaParticula * Random.Range(0.7f, 1.3f);
        p.fase = Random.Range(0, Mathf.PI * 2);
        p.sr.sprite = sprites[Random.Range(0, sprites.Length)];

        if (movimento == Movimento.Subir)
        {
            // bolhas nascem na metade de baixo e sobem
            p.inicio = new Vector2(Random.Range(-raio, raio), Random.Range(-raio, 0));
        }
        else
        {
            // estrelas aparecem num anel em volta do pai
            p.inicio = Random.insideUnitCircle.normalized * Random.Range(raio * 0.6f, raio);
        }
    }
}
