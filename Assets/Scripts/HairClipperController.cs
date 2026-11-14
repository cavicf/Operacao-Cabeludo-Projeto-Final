using UnityEngine;

public class HairClipperController : MonoBehaviour
{
    Rigidbody2D rb;
    float velocityX = 3.8f;
    
    [SerializeField] Transform maquinaCima;
    [SerializeField] Transform maquinaBaixo;
    [SerializeField] BoxCollider2D zonaPonto;
   
    // tempo de um ciclo completo da maquina abrir e fechar
    [SerializeField] float periodoMovimento = 2f;
    float vaoBase;
    float amplitude;
    float tempo;
    
    void Awake()
    {
        // pega a referencia do RigidBody das maquinas
        rb = GetComponent<Rigidbody2D>();

        // aplica a velocidade para a esquerda
        rb.linearVelocityX = -velocityX;
    }
    
    public void DefinirVao(float vao, float amplitudeMovimento = 0)
    {
        vaoBase = vao;
        amplitude = amplitudeMovimento;

        // cada par comeca num ponto diferente do ciclo, para nao ficarem todos sincronizados
        tempo = Random.Range(0f, periodoMovimento);

        PosicionarMaquinas(vao);
    }
    
    void Update()
    {
        // sem amplitude, as maquinas ficam paradas
        if (amplitude <= 0)
        {
            return;
        }

        // faz o vao abrir e fechar de forma suave usando seno de -1 a 1
        tempo += Time.deltaTime;
        float onda = Mathf.Sin(tempo * 2 * Mathf.PI / periodoMovimento);
        PosicionarMaquinas(vaoBase + amplitude * onda);
    }

    void PosicionarMaquinas(float vao)
    {
        // a peruca pode destruir uma das maquinas, entao confere se ela ainda existe
        if (maquinaCima != null)
        {
            maquinaCima.localPosition = new Vector3(0, vao / 2, 0);
        }

        if (maquinaBaixo != null)
        {
            maquinaBaixo.localPosition = new Vector3(0, -vao / 2, 0);
        }

        // a zona de ponto acompanha o tamanho do vao
        zonaPonto.size = new Vector2(zonaPonto.size.x, vao);
    }
}