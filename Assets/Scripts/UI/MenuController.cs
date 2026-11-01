using UnityEngine;
using TMPro;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] TMP_Text title;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text playButton;
        void Start()
        {
            // se ja jogou, troca o titulo e mostra a pontuacao
            if (GameData.jogou)
            {
                title.text = "Game Over";
                scoreText.text = "Sua pontuação: " + GameData.pontuacao;
                playButton.text = "Tente novamente";
            }
        }
    }
}
