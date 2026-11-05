using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PillManager : MonoBehaviour
{
    [Header("This Script manages: pill tracker, quickTimeEvent activation")]
    [Space(20)]
    [Header("SETTINGS")]
    public int pillsLeft;

    [Header("QUICK TIME EVENT SETTINGS")]

    public RectTransform needle;
    public RectTransform targetZone;
    public float needleSpeed = 20f;
    private float _needleAngle = 0f;

    public CanvasGroup qteGroup;
    private bool _qteSuccess = false;
    public float zoneSize = 20f;
    private bool _qteActive = false;
    public float targetAngle = 90f; //this gets randomized each time

    [Header("DEBUG STUFF")]
    public TextMeshProUGUI pillAmount; //debug//
    public CanvasGroup vignetteEffect; //debug temp effect//

    private void Start()
    {
        qteGroup.alpha = 0;
        pillAmount.text = pillsLeft.ToString();
        vignetteEffect.alpha = 1; 
    }

    void StartQTE()
    {
        targetZone.GetComponent<Image>().fillAmount = zoneSize / 360f;
        _qteActive = true;
        _needleAngle = 0;
        _qteSuccess = false;
        targetAngle = Random.Range(60f, 300f);
        targetZone.localRotation = Quaternion.Euler(0, 0, -targetAngle);
        qteGroup.alpha = 1; 

    }

    private void EndQTE(bool success)
    {
        _qteActive = false;
        _qteSuccess = success;
        qteGroup.alpha = 0;

        if (success)
        {
            Debug.Log("QTE SUCCESS :D");
            // do visual stuff here for hallucinations //
            pillsLeft--;
            vignetteEffect.alpha = 0;
            pillAmount.text = pillsLeft.ToString();
        }
        else
        { 
            Debug.Log("QTE FAILED D:"); 
        }
    }

    private void Update()
    {
        //temp test trigger
        if (!_qteActive && Keyboard.current.pKey.wasPressedThisFrame) StartQTE();

        if (!_qteActive) return;

        //spin needle 
        _needleAngle += needleSpeed * Time.deltaTime;
        needle.localRotation = Quaternion.Euler(0, 0, -_needleAngle);

        // input check //

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            float diff = Mathf.DeltaAngle(_needleAngle, targetAngle);
            float distanceFromCenter = Mathf.Abs(diff);
            float halfzone = zoneSize / 2f;

            bool hit = distanceFromCenter <= halfzone;
            EndQTE(hit);
        }
        else if (_needleAngle >= 360)
        {
            EndQTE(false);
        }


    }



}
