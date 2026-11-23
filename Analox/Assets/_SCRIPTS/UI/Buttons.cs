using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(1);
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
