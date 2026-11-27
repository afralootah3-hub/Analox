using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public string startScene;
    public void Play()
    {
        DataManager.instance.PlayGame();
        SceneManager.LoadScene(startScene);
    }


    public void Exit()
    {
        //for the build
        Application.Quit();

        //for play mode
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
