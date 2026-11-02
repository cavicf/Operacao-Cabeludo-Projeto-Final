using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    Material mat;
    // fundo mais lento que o chao para dar efeito de profundidade (parallax)
    [SerializeField] float velocity = 0.2f;
    float offsetX;
    void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        offsetX += Time.deltaTime * velocity;
        mat.mainTextureOffset = new Vector2(offsetX, 0);
    }
}