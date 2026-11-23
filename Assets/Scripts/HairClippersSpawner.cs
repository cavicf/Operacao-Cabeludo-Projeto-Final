using UnityEngine;

public class HairClippersSpawner : MonoBehaviour
{
    [SerializeField] HairClipperController HairClipperPrefab;
    [SerializeField] PhaseController gerenciadorFases;
   
    // quanto o vao abre e fecha nas fases com movimento
    [SerializeField] float amplitudeMovimento = 0.5f;

    // power-ups que podem aparecer entre as maquinas
    [SerializeField] GameObject[] powerUpPrefabs;
    [SerializeField, Range(0, 1)] float chancePowerUp = 0.25f;

    // o power-up aparece numa altura sorteada, longe do caminho "facil" entre os vaos,
    // para o jogador ter que arriscar para pegar
    [SerializeField] float distanciaMinimaDoCaminho = 1.2f;
    // folga em relacao ao chao e ao teto, para o item nunca ficar impossivel de pegar
    [SerializeField] float margemPowerUp = 0.6f;
    
    // power-up que esta na tela agora (so pode existir um por vez)
    GameObject powerUpNaTela;
    bool powerUpAgendado;
    
    float intervalo = 1.8f;
    // até pnde as laminas podem chegar
    float limiteLaminaBaixo = -3.8f;
    float limiteLaminaCima = 4.4f;
    // altura do vao do proximo par de maquinas (sorteada com antecedencia)
    float proximaAltura;
    float alturaPowerUp;
    
    // altura do meio da area onde as maquinas podem ficar
    public float AlturaCentro => (limiteLaminaBaixo + limiteLaminaCima) / 2;

    // sorteia o centro do vao sem deixar as laminas sairem da area visivel
    float SortearAltura(float vao)
    {
        float alturaMin = limiteLaminaBaixo + vao / 2;
        float alturaMax = limiteLaminaCima - vao / 2;
        return Random.Range(alturaMin, alturaMax);
    }

    void Start()
    {
        Fase faseInicial = gerenciadorFases.FaseAtual;
        proximaAltura = SortearAltura(faseInicial.vao + amplitudeMovimento);

        // invoca o spawn de maquinas no instante 0 e depois a cada intervalo
        InvokeRepeating("SpawnHairClipper", 0, intervalo);
    }

    
    
    void SpawnHairClipper()
    {
        // le as configuracoes da fase atual
        Fase fase = gerenciadorFases.FaseAtual;
        
        // usa a altura ja sorteada e sorteia a do proximo vao
        float alturaAtual = proximaAltura;
        proximaAltura = SortearAltura(fase.vao + amplitudeMovimento);

        // Instancia um novo par de maquinas no jogo e ajusta o vao dele
        Vector3 posicao = new Vector3(transform.position.x, alturaAtual, 0);
        HairClipperController par = Instantiate(HairClipperPrefab, posicao, Quaternion.identity);
        
        float amplitude = 0;
        if (fase.maquinasMovem)
        {
            amplitude = amplitudeMovimento;
        }

        par.DefinirVao(fase.vao, amplitude);
        
        // as vezes aparece um power-up no meio do caminho ate o proximo par,
        // mas so se nao tiver nenhum outro na tela (ou para aparecer)
        if (PodeSpawnarPowerUp() && Random.value < chancePowerUp)
        {
            // sorteia a altura, fugindo do caminho medio entre o vao atual e o proximo
            float caminho = (alturaAtual + proximaAltura) / 2;
            alturaPowerUp = SortearAlturaPowerUp(caminho);
            powerUpAgendado = true;
            Invoke("SpawnPowerUp", intervalo / 2);
        }
    }

    // coloca no meio da tela o primeiro par de maquinas que esta na frente do professor
    // (se ainda nao tiver nenhum na frente, o proximo a nascer ja vem no meio)
    public void CentralizarProximoPar(float posicaoProfessor)
    {
        HairClipperController maisPerto = null;
        foreach (HairClipperController par in FindObjectsByType<HairClipperController>(FindObjectsSortMode.None))
        {
            // os power-ups tambem usam esse script para andar, entao ficam de fora
            if (par.GetComponent<PowerUpItem>() != null)
            {
                continue;
            }

            float x = par.transform.position.x;
            if (x > posicaoProfessor && (maisPerto == null || x < maisPerto.transform.position.x))
            {
                maisPerto = par;
            }
        }

        if (maisPerto != null)
        {
            Vector3 posicao = maisPerto.transform.position;
            maisPerto.transform.position = new Vector3(posicao.x, AlturaCentro, posicao.z);
        }
        else
        {
            proximaAltura = AlturaCentro;
        }
    }

    float SortearAlturaPowerUp(float caminho)
    {
        float alturaMin = limiteLaminaBaixo + margemPowerUp;
        float alturaMax = limiteLaminaCima - margemPowerUp;

        // tenta algumas vezes uma altura qualquer que fique longe do caminho
        for (int tentativa = 0; tentativa < 10; tentativa++)
        {
            float altura = Random.Range(alturaMin, alturaMax);
            if (Mathf.Abs(altura - caminho) >= distanciaMinimaDoCaminho)
            {
                return altura;
            }
        }

        // nao achou (caminho bem no meio): vai para o lado que tiver mais espaco
        return caminho - alturaMin > alturaMax - caminho ? alturaMin : alturaMax;
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
