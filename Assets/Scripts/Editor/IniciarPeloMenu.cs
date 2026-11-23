using UnityEditor;
using UnityEditor.SceneManagement;

// faz o Play do Editor sempre comecar pelo menu principal, igual ao jogo buildado,
// mesmo quando a cena aberta for a GameScene
[InitializeOnLoad]
public static class IniciarPeloMenu
{
    static IniciarPeloMenu()
    {
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/MainMenu.unity");
    }
}
