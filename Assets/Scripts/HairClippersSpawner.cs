using UnityEngine;

public class HairClippersSpawner : MonoBehaviour
{
    [SerializeField] HairClipperController HairClipperPrefab;
    [SerializeField] PhaseController gerenciadorFases;
    
    float intervalo = 1.8f;
    float limiteLaminaBaixo = -3.8f;
    float limiteLaminaCima = 4.4f;
   

    // power-ups que podem aparecer entre as maquinas
    [SerializeField] GameObject[] powerUpPrefabs;
    [SerializeField, Range(0, 1)] float chancePowerUp = 0.25f;

    // altura do vao do proximo par de maquinas (sorteada com antecedencia)
    float proximaAltura;
    float alturaPowerUp;

    // power-up que esta na tela agora (so pode existir um por vez)
    GameObject powerUpNaTela;
    bool powerUpAgendado;
    
    // sorteia o centro do vao sem deixar as laminas sairem da area visivel
    float SortearAltura(float vao)
    {
        float alturaMin = limiteLaminaBaixo + vao / 2;
        float alturaMax = limiteLaminaCima - vao / 2;
        return Random.Range(alturaMin, alturaMax);
    }

    void Start()
    {
        proximaAltura = SortearAltura(gerenciadorFases.FaseAtual.vao);

        // invoca o spawn de maquinas no instante 0 e depois a cada intervalo
        InvokeRepeating("SpawnHairClipper", 0, intervalo);
    }

    
    
    void SpawnHairClipper()
    {
        // le as configuracoes da fase atual
        Fase fase = gerenciadorFases.FaseAtual;
        
        // usa a altura ja sorteada e sorteia a do proximo vao
        float alturaAtual = proximaAltura;
        proximaAltura = SortearAltura(fase.vao);

        // Instancia um novo par de maquinas no jogo e ajusta o vao dele
        Vector3 posicao = new Vector3(transform.position.x, alturaAtual, 0);
        HairClipperController par = Instantiate(HairClipperPrefab, posicao, Quaternion.identity);
        par.DefinirVao(fase.vao);
        
        // as vezes aparece um power-up no meio do caminho ate o proximo par,
        // mas so se nao tiver nenhum outro na tela (ou para aparecer)
        if (PodeSpawnarPowerUp() && Random.value < chancePowerUp)
        {
            // altura media entre o vao atual e o proximo
            alturaPowerUp = (alturaAtual + proximaAltura) / 2;
            powerUpAgendado = true;
            Invoke("SpawnPowerUp", intervalo / 2);
        }
    }

    bool PodeSpawnarPowerUp()
    {
        // o power-up anterior some quando e coletado ou destruido pelo Shredder
        return powerUpPrefabs.Length > 0 && powerUpNaTela == null && !powerUpAgendado;
    }

    void SpawnPowerUp()
    {
        powerUpAgendado = false;

        // sorteia qual power-up vai aparecer
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        Vector3 posicao = new Vector3(transform.position.x, alturaPowerUp, 0);
        powerUpNaTela = Instantiate(prefab, posicao, Quaternion.identity);
    }
}
