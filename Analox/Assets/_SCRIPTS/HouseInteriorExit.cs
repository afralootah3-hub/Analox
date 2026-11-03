using UnityEngine;
using UnityEngine.SceneManagement;

public class HouseInteriorExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //go to next scene
            SceneManager.LoadScene(0);
        }
    }


}
