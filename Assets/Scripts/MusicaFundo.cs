using UnityEngine;

// toca a trilha do jogo em loop. Fica viva entre as cenas (menu e jogo),
// entao a musica nao reinicia toda vez que o jogador morre ou volta ao menu.
[RequireComponent(typeof(AudioSource))]
public class MusicaFundo : MonoBehaviour
{
    static MusicaFundo instancia;

    void Awake()
    {
        // se ja existe uma musica tocando (veio da outra cena), essa copia sai
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        AudioSource fonte = GetComponent<AudioSource>();
        fonte.loop = true;
        if (!fonte.isPlaying)
        {
            fonte.Play();
        }
    }
}
