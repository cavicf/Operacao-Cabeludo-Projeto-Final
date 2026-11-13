using UnityEngine;

public class HairClipperController : MonoBehaviour
{
    Rigidbody2D rb;
    float velocityX = 3.8f;
    
    [SerializeField] Transform maquinaCima;
    [SerializeField] Transform maquinaBaixo;
    [SerializeField] BoxCollider2D zonaPonto;

    void Awake()
    {
        // pega a referencia do RigidBody das maquinas
        rb = GetComponent<Rigidbody2D>();

        // aplica a velocidade para a esquerda
        rb.linearVelocityX = -velocityX;
    }
    
    public void DefinirVao(float vao)
    {
        // maquinas vão pra metade do vao acima e abaixo, aumentando o espaço
        maquinaCima.localPosition = new Vector3(0, vao / 2, 0);
        maquinaBaixo.localPosition = new Vector3(0, -vao / 2, 0);
        
        // a zona de ponto acompanha o vao
        zonaPonto.size =  new Vector2(zonaPonto.size.x, vao);
    }
}