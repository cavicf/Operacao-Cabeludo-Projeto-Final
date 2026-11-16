using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// animacao de "novo recorde" que aparece no meio da tela depois que o professor explode:
// a estrela cresce dando um "pulo", gira de um lado para o outro e estrelinhas piscam em volta.
// Usa o tempo real (unscaled), porque na morte o jogo esta em camera lenta.
public class NovoRecordeAnimacao : MonoBehaviour
{
    [SerializeField] RectTransform estrela;
    [SerializeField] TMP_Text titulo;
    [SerializeField] TMP_Text pontuacao;
    [SerializeField] Image[] brilhos;
    [SerializeField] AudioClip somNovoRecorde;
    [SerializeField] float duracao = 2.2f;

    // o objeto comeca desativado na cena e so aparece quando um recorde e batido
    public IEnumerator Mostrar(int recorde)
    {
        gameObject.SetActive(true);
        pontuacao.text = recorde.ToString();

        if (somNovoRecorde != null)
        {
            AudioSource.PlayClipAtPoint(somNovoRecorde, Camera.main.transform.position);
        }

        // posicoes sorteadas para as estrelinhas em volta da estrela grande
        Vector2[] posicoes = new Vector2[brilhos.Length];
        float[] fases = new float[brilhos.Length];
        for (int i = 0; i < brilhos.Length; i++)
        {
            posicoes[i] = Random.insideUnitCircle.normalized * Random.Range(170f, 260f);
            fases[i] = Random.value;
            brilhos[i].rectTransform.anchoredPosition = estrela.anchoredPosition + posicoes[i];
        }

        float tempo = 0;
        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            // estrela: aparece com um "pulo" (passa do tamanho e volta), em degraus
            float entrada = Mathf.Clamp01(tempo / 0.35f);
            float escala = entrada < 1 ? Mathf.Sin(entrada * Mathf.PI * 0.75f) * 1.35f : 1f;
            escala = Mathf.Round(escala * 8) / 8;
            estrela.localScale = Vector3.one * escala;

            // balanca de um lado para o outro
            float balanco = Mathf.Round(Mathf.Sin(tempo * 6) * 2) * 6;
            estrela.localRotation = Quaternion.Euler(0, 0, balanco);

            // textos aparecem logo depois da estrela
            bool mostrarTexto = tempo > 0.25f;
            titulo.enabled = mostrarTexto;
            pontuacao.enabled = mostrarTexto;
            float pulso = 1 + Mathf.Round((Mathf.Sin(tempo * 10) + 1) * 1.5f) * 0.03f;
            titulo.rectTransform.localScale = Vector3.one * pulso;

            // estrelinhas piscando em volta
            for (int i = 0; i < brilhos.Length; i++)
            {
                float ciclo = Mathf.Repeat(tempo * 1.6f + fases[i], 1f);
                float tamanho = Mathf.Ceil(Mathf.Sin(ciclo * Mathf.PI) * 3) / 3;
                brilhos[i].rectTransform.localScale = Vector3.one * tamanho;
            }

            yield return null;
        }
    }
}
