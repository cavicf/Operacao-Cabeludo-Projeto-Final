using UnityEngine;

public class HairClippersSpawner : MonoBehaviour
{
    [SerializeField] GameObject HairClipperPrefab;
    float intervalo = 1.8f;
    float alturaMin = -2.2f;
    float alturaMax = 3.2f;

    // power-ups que podem aparecer entre as maquinas
    [SerializeField] GameObject[] powerUpPrefabs;
    [SerializeField, Range(0, 1)] float chancePowerUp = 0.25f;

    // altura do vao do proximo par de maquinas (sorteada com antecedencia)
    float proximaAltura;
    float alturaPowerUp;

    void Start()
    {
        proximaAltura = Random.Range(alturaMin, alturaMax);

        // invoca o spawn de maquinas no instante 0 e depois a cada intervalo
        InvokeRepeating("SpawnHairClipper", 0, intervalo);
    }

    void SpawnHairClipper()
    {
        // usa a altura ja sorteada e sorteia a do proximo vao
        float alturaAtual = proximaAltura;
        proximaAltura = Random.Range(alturaMin, alturaMax);

        // Instancia um novo par de maquinas no jogo
        Vector3 posicao = new Vector3(transform.position.x, alturaAtual, 0);
        Instantiate(HairClipperPrefab, posicao, Quaternion.identity);

        // as vezes aparece um power-up no meio do caminho ate o proximo par
        if (powerUpPrefabs.Length > 0 && Random.value < chancePowerUp)
        {
            // altura media entre o vao atual e o proximo
            alturaPowerUp = (alturaAtual + proximaAltura) / 2;
            Invoke("SpawnPowerUp", intervalo / 2);
        }
    }

    void SpawnPowerUp()
    {
        // sorteia qual power-up vai aparecer
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        Vector3 posicao = new Vector3(transform.position.x, alturaPowerUp, 0);
        Instantiate(prefab, posicao, Quaternion.identity);
    }
}
