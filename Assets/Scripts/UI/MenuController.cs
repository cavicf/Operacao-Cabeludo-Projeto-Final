using UnityEngine;
using TMPro;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] TMP_Text title;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text recordeText;
        [SerializeField] TMP_Text playButton;
        void Start()
        {
            // a linha do recorde so aparece na tela de game over
            recordeText.gameObject.SetActive(false);

            // se ja jogou, troca o titulo e mostra a pontuacao
            if (GameData.jogou)
            {
                title.text = "Game Over";
                scoreText.text = "Sua pontuação: " + GameData.pontuacao;
                recordeText.text = "Pontuação recorde: " + GameData.recorde;
                recordeText.gameObject.SetActive(true);
                playButton.text = "Tente novamente";
            }
        }
    }
}
