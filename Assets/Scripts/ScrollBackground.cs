using UnityEngine;

public class scrollBackground : MonoBehaviour
{
    Material mat;
    float velocity = 0.2f;
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