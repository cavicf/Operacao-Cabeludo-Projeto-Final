using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fica em cada maquina (de cima e de baixo) do par.
// Quando o professor bate com a peruca, a maquina nao some: ela treme, pisca,
// racha, perde dentes e fica destruida (soltando fumaca) ate sair da tela.
public class MaquinaDestruivel : MonoBehaviour
{
    [SerializeField] Sprite spriteRachada;
    [SerializeField] Sprite spriteDestruida;
    [SerializeField] AudioClip somQuebrando;

    // mesma explosao dos inimigos, pequena, no ponto da batida
    [SerializeField] GameObject explosaoPrefab;

    // quadros cinza da explosao, usados como fumacinha saindo da maquina quebrada
    [SerializeField] Sprite[] quadrosFumaca;

    // a maquina de baixo tem os dentes na parte de cima do sprite; a de cima, embaixo
    [SerializeField] bool dentesEmCima = true;

    [SerializeField] float duracaoTremida = 0.3f;
    [SerializeField] float intervaloFumaca = 0.35f;

    SpriteRenderer sr;
    Collider2D col;
    float tremida;

    public bool Destruida { get; private set; }

    class Pedaco
    {
        public Transform transform;
        public SpriteRenderer sr;
        public Vector2 velocidade;
        public float giro;
        public float angulo;
        public float vida;
    }

    List<Pedaco> pedacos = new List<Pedaco>();

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void Destruir(Vector2 pontoBatida)
    {
        if (Destruida)
        {
            return;
        }

        Destruida = true;

        // o professor passa direto pela maquina quebrada
        col.enabled = false;

        if (somQuebrando != null)
        {
            AudioSource.PlayClipAtPoint(somQuebrando, Camera.main.transform.position);
        }

        if (explosaoPrefab != null)
        {
            Instantiate(explosaoPrefab, pontoBatida, Quaternion.identity);
        }

        SoltarDentes();
        StartCoroutine(Quebrar());
    }

    IEnumerator Quebrar()
    {
        // pisca e treme, passa para rachada e depois para destruida
        tremida = duracaoTremida;
        sr.color = new Color(1f, 0.55f, 0.45f);
        sr.sprite = spriteRachada;
        yield return new WaitForSeconds(0.06f);

        sr.color = Color.white;
        yield return new WaitForSeconds(0.06f);

        sr.color = new Color(1f, 0.55f, 0.45f);
        sr.sprite = spriteDestruida;
        yield return new WaitForSeconds(0.06f);

        sr.color = Color.white;

        // fumacinha saindo da cabeca quebrada enquanto a maquina estiver na tela
        while (true)
        {
            SoltarFumaca();
            yield return new WaitForSeconds(intervaloFumaca);
        }
    }

    void LateUpdate()
    {
        // tremida em degraus (depois do HairClipperController posicionar a maquina)
        if (tremida > 0)
        {
            tremida -= Time.deltaTime;
            float lado = Mathf.Repeat(tremida, 0.06f) < 0.03f ? 1 : -1;
            Vector3 pos = transform.localPosition;
            pos.x = tremida > 0 ? lado * 0.06f : 0;
            transform.localPosition = pos;
        }

        AnimarPedacos();
    }

    // converte um pixel do sprite (origem embaixo a esquerda) em posicao no mundo,
    // respeitando o pivo do sprite (a maquina de baixo tem o pivo em cima e a de cima, embaixo)
    Vector3 PixelParaMundo(float x, float y)
    {
        Sprite s = sr.sprite;
        Vector2 local = (new Vector2(x, y) - s.pivot) / s.pixelsPerUnit;
        return transform.TransformPoint(local);
    }

    // cria alguns dentes da maquina voando para longe
    void SoltarDentes()
    {
        Sprite original = sr.sprite;
        float ppu = original.pixelsPerUnit;
        Rect area = original.rect;
        float sinal = dentesEmCima ? 1 : -1;

        // dentes: 12 x 32 pixels, comecando na coluna 16 e repetindo a cada 16 pixels
        int[] colunas = { 16, 64, 96, 128 };
        foreach (int x in colunas)
        {
            float y = dentesEmCima ? area.yMax - 32 : area.yMin;
            Sprite dente = Sprite.Create(original.texture, new Rect(area.x + x, y, 12, 32), new Vector2(0.5f, 0.5f), ppu);

            GameObject go = new GameObject("Dente");
            go.transform.position = PixelParaMundo(x + 6, dentesEmCima ? area.height - 16 : 16);

            Pedaco p = new Pedaco();
            p.transform = go.transform;
            p.sr = go.AddComponent<SpriteRenderer>();
            p.sr.sprite = dente;
            p.sr.sortingLayerID = sr.sortingLayerID;
            p.sr.sortingOrder = sr.sortingOrder + 1;
            p.velocidade = new Vector2(Random.Range(-3f, 3f), sinal * Random.Range(3f, 6f));
            p.giro = Random.Range(-900f, 900f);
            pedacos.Add(p);
        }
    }

    void SoltarFumaca()
    {
        if (quadrosFumaca == null || quadrosFumaca.Length == 0)
        {
            return;
        }

        // sai da "cabeca" da maquina, logo depois dos dentes (uns 96 pixels da ponta)
        float altura = sr.sprite.rect.height;
        Vector3 origem = PixelParaMundo(80 + Random.Range(-40f, 40f), dentesEmCima ? altura - 96 : 96);

        GameObject go = new GameObject("Fumaca");
        go.transform.position = origem;
        go.transform.localScale = Vector3.one * 0.6f;

        Pedaco p = new Pedaco();
        p.transform = go.transform;
        p.sr = go.AddComponent<SpriteRenderer>();
        p.sr.sprite = quadrosFumaca[Random.Range(0, quadrosFumaca.Length)];
        p.sr.sortingLayerID = sr.sortingLayerID;
        p.sr.sortingOrder = sr.sortingOrder + 1;

        // a fumaca sobe devagar e acompanha a maquina andando para a esquerda
        p.velocidade = new Vector2(-3.8f, 0.8f);
        p.giro = 0;
        pedacos.Add(p);
    }

    void AnimarPedacos()
    {
        for (int i = pedacos.Count - 1; i >= 0; i--)
        {
            Pedaco p = pedacos[i];
            p.vida += Time.deltaTime;

            // dentes caem com gravidade e giram em degraus de 90 graus; fumaca so sobe
            if (p.giro != 0)
            {
                p.velocidade.y -= 20f * Time.deltaTime;
                p.angulo += p.giro * Time.deltaTime;
                p.transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(p.angulo / 90) * 90);
            }

            p.transform.position += (Vector3)(p.velocidade * Time.deltaTime);

            float duracao = p.giro != 0 ? 1f : 0.7f;
            if (p.vida > duracao * 0.6f)
            {
                p.sr.enabled = Mathf.Repeat(p.vida, 0.1f) < 0.05f;
            }

            if (p.vida > duracao)
            {
                Destroy(p.transform.gameObject);
                pedacos.RemoveAt(i);
            }
        }
    }

    void OnDestroy()
    {
        // quando o Shredder destroi o par, os pedacos que sobraram somem junto
        foreach (Pedaco p in pedacos)
        {
            if (p.transform != null)
            {
                Destroy(p.transform.gameObject);
            }
        }
    }
}
