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
            Application.Quit();
        }
    }
}

