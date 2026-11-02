using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainUIController : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void QuitGame()
        {
            // na tela de game over, "Sair" volta para o menu inicial
            if (GameData.jogou)
            {
                GameData.jogou = false;
                SceneManager.LoadScene("MainMenu");
                return;
            }

            // no menu inicial fecha o jogo (so funciona no jogo buildado, no Editor nao faz nada)
            Application.Quit();
        }
    }
}

