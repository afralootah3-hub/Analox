using Unity.Cinemachine;
using UnityEngine;

public class CameraSwap : MonoBehaviour
{
    public Transform playerTransform;
    [Tooltip("This is the cam to swap to when entering the trigger")]
    public Camera activeCam;
    [Tooltip("This is the cam that follows the player outside fixed cam areas")]
    public Camera swapCam;
    public PlayerController playercc;
    public GameObject camHolder;
    public GameObject mainCamHolder;
    public UiManager _ui;

    public Transform mainholdpoint;
    public Transform swapHoldpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeCam.enabled = false;
            swapCam.enabled = true;
            playercc._pActiveCam = swapCam;
            playercc._cameraHolder = camHolder.transform;
            _ui.holdpoint = swapHoldpoint;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeCam.enabled = true;
            swapCam.enabled = false;
            playercc._pActiveCam = activeCam;
            playercc._cameraHolder = mainCamHolder.transform;
            _ui.holdpoint = mainholdpoint;
            

        }
    }

}
