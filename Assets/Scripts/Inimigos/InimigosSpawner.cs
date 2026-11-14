using UnityEngine;

// cria piolhos e navalhas na direita da tela, so nas fases que tem inimigos
public class InimigosSpawner : MonoBehaviour
{
    [SerializeField] PhaseController gerenciadorFases;
    [SerializeField] Inimigo[] inimigosPrefabs;

    // tempo sorteado entre um inimigo e outro
    [SerializeField] float intervaloMin = 2.2f;
    [SerializeField] float intervaloMax = 3.8f;

    // faixa de altura onde os inimigos podem aparecer (entre o chao e o teto)
    float alturaMin = -3.2f;
    float alturaMax = 4.2f;

    float tempo;

    void Update()
    {
        if (!gerenciadorFases.FaseAtual.temInimigos)
        {
            // quando a fase com inimigos comecar, o primeiro demora um pouco para aparecer
            tempo = intervaloMin;
            return;
        }

        tempo -= Time.deltaTime;
        if (tempo <= 0)
        {
            SpawnInimigo();
            tempo = Random.Range(intervaloMin, intervaloMax);
        }
    }

    void SpawnInimigo()
    {
        // sorteia qual inimigo vai aparecer e em que altura
        Inimigo prefab = inimigosPrefabs[Random.Range(0, inimigosPrefabs.Length)];
        Vector3 posicao = new Vector3(transform.position.x, Random.Range(alturaMin, alturaMax), 0);
        Instantiate(prefab, posicao, Quaternion.identity);
    }
}
