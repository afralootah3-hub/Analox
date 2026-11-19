using Unity.Cinemachine;
using UnityEngine;

public class CameraSwap : MonoBehaviour
{
    public Transform playerTransform;
   
    public Camera activeCam;
   
    public Camera swapCam;
    public PlayerController playercc;
    public GameObject camHolder;
    public GameObject mainCamHolder;
    public UiManager _ui;

    public Transform mainholdpoint;
    public Transform swapHoldpoint;

    //

    public CinemachineCamera mainCM_Cam;
    public CinemachineCamera swapCM_Cam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //activeCam.enabled = false;
            //swapCam.enabled = true;
            //playercc._pActiveCam = swapCam;
            playercc._cameraHolder = camHolder.transform;

            swapCM_Cam.Priority = 2;
            _ui.holdpoint = swapHoldpoint;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //activeCam.enabled = true;
            //swapCam.enabled = false;
            //playercc._pActiveCam = activeCam;
            playercc._cameraHolder = mainCamHolder.transform;
            _ui.holdpoint = mainholdpoint;

            swapCM_Cam.Priority = -1;
        }
    }

}
