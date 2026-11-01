using UnityEngine;

public class HairClippersSpawner : MonoBehaviour
{
    [SerializeField] GameObject HairClipperPrefab;
    float intervalo = 1.8f;
    float alturaMin = -2.2f;
    float alturaMax = 3.2f;

    void Start()
    {
        // invoca o spawn de maquinas no instante 0 e depois a cada intervalo
        InvokeRepeating("SpawnHairClipper", 0, intervalo);
    }

    void SpawnHairClipper()
    {
        // sorteia a altura do vão
        float alturaAleatoria = Random.Range(alturaMin, alturaMax);
        
        // Instancia um novo par de maquinas no jogo
        Vector3 posicao = new Vector3(transform.position.x, alturaAleatoria, 0);
        Instantiate(HairClipperPrefab, posicao, Quaternion.identity);
    }
}