using System.Collections.Generic;
using UnityEngine;

// explosao da cabeca do professor quando ele morre:
// o sprite atual (com o cabelo do nivel em que ele estava) e cortado em pedacos
// que voam para os lados, junto com a mesma animacao de explosao dos inimigos.
// Usa o tempo real (unscaled), porque na morte o jogo entra em camera lenta.
public class ExplosaoCabeca : MonoBehaviour
{
    [SerializeField] Sprite[] quadrosExplosao;
    [SerializeField] float tempoQuadro = 0.08f;
    [SerializeField] float escalaExplosao = 2.5f;
    [SerializeField] AudioClip somExplosao;

    // a cabeca e dividida em uma grade de pedacos x pedacos
    [SerializeField] int pedacos = 5;
    [SerializeField] float forca = 7f;
    [SerializeField] float gravidade = 18f;
    [SerializeField] float duracaoPedacos = 1.2f;

    class Pedaco
    {
        public Transform transform;
        public SpriteRenderer sr;
        public Vector2 velocidade;
        public float giro;
        public float angulo;
    }

    List<Pedaco> lista = new List<Pedaco>();
    SpriteRenderer srExplosao;
    int quadroAtual;
    float tempoAnimacao;
    float tempoVida;

    // chamado pelo professor: copia o visual da cabeca e explode
    public void Explodir(SpriteRenderer cabeca)
    {
        transform.position = cabeca.transform.position;

        if (somExplosao != null)
        {
            AudioSource.PlayClipAtPoint(somExplosao, Camera.main.transform.position);
        }

        CriarPedacos(cabeca);

        // animacao de explosao no centro, por cima dos pedacos
        GameObject go = new GameObject("Explosao");
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one * escalaExplosao;
        srExplosao = go.AddComponent<SpriteRenderer>();
        srExplosao.sprite = quadrosExplosao[0];
        srExplosao.sortingLayerID = cabeca.sortingLayerID;
        srExplosao.sortingOrder = cabeca.sortingOrder + 10;
    }

    void CriarPedacos(SpriteRenderer cabeca)
    {
        Sprite sprite = cabeca.sprite;
        Rect area = sprite.rect;
        float ppu = sprite.pixelsPerUnit;
        Vector3 escala = cabeca.transform.lossyScale;

        // tamanho de cada pedaco em pixels, alinhado na grade de 4px da pixel art
        float largura = Mathf.Floor(area.width / pedacos / 4) * 4;
        float altura = Mathf.Floor(area.height / pedacos / 4) * 4;

        for (int i = 0; i < pedacos; i++)
        {
            for (int j = 0; j < pedacos; j++)
            {
                Rect recorte = new Rect(area.x + i * largura, area.y + j * altura, largura, altura);
                Sprite parte = Sprite.Create(sprite.texture, recorte, new Vector2(0.5f, 0.5f), ppu);

                // posicao do pedaco em relacao ao centro (pivo) do sprite original
                Vector2 centroPedaco = new Vector2(i * largura + largura / 2, j * altura + altura / 2);
                Vector2 deslocamento = (centroPedaco - sprite.pivot) / ppu;
                deslocamento.Scale(escala);

                GameObject go = new GameObject("Pedaco");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = deslocamento;
                go.transform.localScale = escala;

                Pedaco p = new Pedaco();
                p.transform = go.transform;
                p.sr = go.AddComponent<SpriteRenderer>();
                p.sr.sprite = parte;
                p.sr.sortingLayerID = cabeca.sortingLayerID;
                p.sr.sortingOrder = cabeca.sortingOrder + 1;

                // cada pedaco voa para longe do centro, um pouco para cima
                Vector2 direcao = deslocamento.sqrMagnitude > 0.001f ? deslocamento.normalized : Random.insideUnitCircle.normalized;
                p.velocidade = (direcao + Vector2.up * 0.6f) * forca * Random.Range(0.6f, 1.2f);
                p.giro = Random.Range(-720f, 720f);
                lista.Add(p);
            }
        }
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        tempoVida += dt;

        AnimarExplosao(dt);

        foreach (Pedaco p in lista)
        {
            p.velocidade.y -= gravidade * dt;
            p.transform.localPosition += (Vector3)(p.velocidade * dt);

            // gira em degraus de 90 graus, para continuar parecendo pixel art
            p.angulo += p.giro * dt;
            p.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Round(p.angulo / 90) * 90);

            // no final os pedacos piscam e somem
            if (tempoVida > duracaoPedacos * 0.7f)
            {
                p.sr.enabled = Mathf.Repeat(tempoVida, 0.1f) < 0.05f;
            }
        }

        if (tempoVida > duracaoPedacos)
        {
            Destroy(gameObject);
        }
    }

    void AnimarExplosao(float dt)
    {
        if (srExplosao == null)
        {
            return;
        }

        tempoAnimacao += dt;
        if (tempoAnimacao < tempoQuadro)
        {
            return;
        }

        tempoAnimacao = 0;
        quadroAtual++;

        if (quadroAtual >= quadrosExplosao.Length)
        {
            Destroy(srExplosao.gameObject);
            return;
        }

        srExplosao.sprite = quadrosExplosao[quadroAtual];
    }
}
