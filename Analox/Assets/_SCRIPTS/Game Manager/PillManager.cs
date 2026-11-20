using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

public class PillManager : MonoBehaviour
{
    public static PillManager instance { get; set; }

    [SerializeField] private AudioClip _breathSound; 
    [SerializeField] private Material _SanityEffect;
    private AudioSource _audioSource;
    ///////////////////////////////////////////

    [Header("This Script manages: pill tracker, quickTimeEvent activation, Sanity stuff")]
    [Space(20)]
    [Header("SETTINGS")]
    public int pillsLeft;
    public int currentSanity = 5;
    public int maxSanity = 5;
    // sanity increments visibility of the vignette , pill resets the vignette to 0

    private int _difficultyScale; //adds currrent sanity and current stealth to scale the QTE accordingly :)

    public int maxStealth = 5;
    public int currentStealth = 5;
    public TextMeshProUGUI pillAmount; //debug//
    public CanvasGroup vignetteEffect; //debug temp effect//
    private float _sanityIncrement; // automatically calculated based on maxSanity
    public GameObject pillChoiceHolder;

    ///////////////////////////////////////////

    [Header("QUICK TIME EVENT SETTINGS")]
    // new qte 
    public Transform startPoint;
    public Transform endPoint;

    public RectTransform greenZone;
    public RectTransform yellowZone;
    public float qteSpeed = 200f;
    private float defaultQTEspeed;

    ///////////////////////////////////////////

    public Camera _mainCamera;
    public RectTransform pointerTransform;

    [Header("SHAKE QTE UI")]
    public RectTransform _uiHolderTransfrom;

    [System.Serializable ]
    public struct DifficultySettings
    {
        public float HighAmp, HighFreq, HighUI, MedAmp, MedFreq, MedUi, MildAmp, MildFreq, MildUI, BaseAmp, BaseFreq, BaseUI;
    }

    public DifficultySettings QTESettings;

    #region private refs
    private PlayerController _pCC;
    private CinemachineBasicMultiChannelPerlin _cmNoise;
    private Coroutine _fadeCamRoutine;
    private float _qteDirection;
    private Vector2 _restPostQTEUI;
    private float _QTEshakeIntensity;
    private Coroutine _uiFadeRoutine; 

    private Vector3 _QteTargetPos;
    private bool _qteHitGreen;
    private bool _qteHitYellow;
    private bool _qteFailed;
    [HideInInspector] public bool _qteActive = false;
    private CinemachineBrain _brain;
    #endregion

    public CanvasGroup qteGroup;

    private void Start()
    {
        currentSanity = maxSanity;
        currentStealth = maxStealth;
        qteGroup.alpha = 0;
        _pCC = FindAnyObjectByType<PlayerController>();
        
        _SanityEffect.SetFloat("_Blend", 0);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0;
        _QteTargetPos = endPoint.localPosition;
        _difficultyScale = 10; //max
        defaultQTEspeed = qteSpeed;
        pillChoiceHolder.SetActive(false);
        // move to onsceneload //

       
        _mainCamera = Camera.main;
        _brain = _mainCamera.GetComponent<CinemachineBrain>();
        _restPostQTEUI = _uiHolderTransfrom.anchoredPosition;

    }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    #region Yarn Commands

    /// <summary>
    /// AMT SHOULD ONLY BE EITHER 1 or -1. if bool is true then amt does not matter
    /// </summary>
    /// <param name="amt"></param>
    /// <param name="isReset"></param>
    public void SetSanity(int amt, bool isReset) //amt doesn't matter if we're resetting
    {
        float alphaamt = (amt) / 3f;

        Debug.Log("alpha amount is " + alphaamt);
        Debug.Log("amt is " + amt);
        Debug.Log(vignetteEffect.alpha);
        // split either 1 or -1 by 5 to equally increase alpha
        

        if (isReset == true) // reset it/ its cleared
        {
            vignetteEffect.alpha = 0;
            _SanityEffect.SetFloat("_Blend", 0);
            _audioSource.volume = 0;
        }
        else //if we're NOT reseting sanity
        { 
            currentSanity += amt;
            vignetteEffect.alpha -= alphaamt;
            _SanityEffect.SetFloat("_Blend", -0.2f);
            Debug.Log(_SanityEffect.GetFloat("_Blend"));
            _audioSource.volume = 1;

            //debug
            //_audioSource.Play();

        }

    }

    [YarnFunction("CheckSanity")]
    public static int SanityIs(int sanity)
    {
        return PillManager.instance.currentSanity;
    }

    #endregion
   
    /* old QTE VOIDS
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
            //vignetteEffect.alpha = 0;
            SetSanity(1, true);
            pillAmount.text = pillsLeft.ToString();
        }
        else
        { 
            Debug.Log("QTE FAILED D:"); 
        }
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
    */ 

    private void Update()
    {
        Debug.Log("CURRENT SANITY IS " + currentSanity);
        
        if (_qteActive)
        {
            StartQTE();
            
        }


        // DEBUG STUFF
       
        if (Keyboard.current.commaKey.wasPressedThisFrame)
        {
            currentSanity -= 1;
            Debug.Log("CURRENT SANITY INCREAED " + currentSanity);
        }

    }


    //can add animations to this
    public void OpenQTEMenu()
    {
        if (_pCC.currentState == PlayerController.playerState.isDisabled) return;
        _pCC.GetComponent<NavMeshAgent>().ResetPath();
        // when click menu 
        pillChoiceHolder.SetActive(true);
        if(_pCC == null)
        {
             _pCC = FindFirstObjectByType<PlayerController>();

        }
        _pCC.setPlayerState(PlayerController.playerState.isDisabled);
    }

    //called by button and when qte starts
    public void CloseQTEMenu()
    {
        pillChoiceHolder.SetActive(false);
        _pCC.setPlayerState(PlayerController.playerState.isIdle);
    }


    /// <summary>
    /// Called by a button to start the qte.
    /// </summary>
    public void EnableQTE()
    {
        float cAmp = 0;
        float cFreq = 0;
        CloseQTEMenu();
        _pCC.setPlayerState(PlayerController.playerState.isDisabled);

        float uiShakeAmt = 0;

        if (!_qteActive)
        {
            _difficultyScale = (currentSanity + currentStealth);
            _qteActive = true;


            pointerTransform.localPosition = startPoint.localPosition;
            _QteTargetPos = endPoint.localPosition;

            switch (_difficultyScale)
            {
                case >= 0 and <= 1:
                    Debug.Log("Hardest QTE difficulty");
                    cAmp = QTESettings.HighAmp;
                    cFreq = QTESettings.HighFreq;
                    uiShakeAmt = QTESettings.HighUI;
                    qteSpeed += 100f;
                    break;
                case >= 2 and <= 4:
                    cAmp = QTESettings.MedAmp;
                    cFreq = QTESettings.MedFreq;
                    uiShakeAmt = QTESettings.MedUi;
                    Debug.Log("Medium QTE difficulty");
                    qteSpeed += 50f;
                    break;
                case >= 5 and <= 8:
                    cAmp = QTESettings.MildAmp;
                    cFreq = QTESettings.MildFreq;
                    uiShakeAmt = QTESettings.MildUI;
                    Debug.Log("Mild QTE difficulty");
                    qteSpeed = defaultQTEspeed;
                    break;
                case >= 9 and <= 10:
                    Debug.Log("No difficulty, default");
                    cAmp = QTESettings.BaseAmp;
                    cFreq = QTESettings.BaseFreq;
                    uiShakeAmt = QTESettings.BaseUI;
                    qteSpeed = defaultQTEspeed;
                    break;
                default:
                    Debug.LogWarning("Difficulty scale out of range!" + _difficultyScale
                        + "\n" + "falling back to default no difficulty");
                    qteSpeed = defaultQTEspeed;
                    break;
            }

            StartCameraShake(cAmp, cFreq);
            StartUIShake(uiShakeAmt);
            
        }
        
    }




    private void StartQTE()
    {
        qteGroup.alpha = 1;
        

        // ping pong across bar 

        if (Vector3.Distance(pointerTransform.localPosition, startPoint.localPosition) < 0.1f)
        {
            _QteTargetPos = endPoint.localPosition;
            _qteDirection = 1f;
        }
        else if (Vector3.Distance(pointerTransform.localPosition, endPoint.localPosition) < 0.1f)
        {
            _QteTargetPos = startPoint.localPosition;
            _qteDirection = -1f;
        }

        pointerTransform.localPosition = Vector3.MoveTowards(pointerTransform.localPosition,
            _QteTargetPos, qteSpeed * Time.deltaTime);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CheckQTESucess();
        }


    }

    private void CheckQTESucess()
    {
        if(RectTransformUtility.RectangleContainsScreenPoint(greenZone, pointerTransform.position, null))
        {
            Debug.Log("QTE SUCCESS");
            _qteHitGreen = true;
        }
        else if(RectTransformUtility.RectangleContainsScreenPoint(yellowZone, pointerTransform.position, null))
        {
            Debug.Log("QTE HIT YELLOW");
            _qteHitYellow = true;
        }
        else
        {
            Debug.Log("QTE FAIL"); //call function
            _qteFailed = true;
        }
        _pCC.setPlayerState(PlayerController.playerState.isIdle);
        _qteActive = false;
        qteGroup.alpha = 0;
        EndCameraShake();
        EndUIShake();

    }

    public void StartCameraShake(float amplitude = 1f, float frequency = 1f)
    {
        // grab active cinemachine camera //
        ICinemachineCamera activeCam = _brain.ActiveVirtualCamera;
        if(_brain.ActiveVirtualCamera is CinemachineVirtualCameraBase activeVCam)
        {
            _cmNoise = activeVCam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

            if (_cmNoise == null)
            {
                Debug.LogWarning("MISSING CINEMACHINE COMPONENTS UNABLE TO GRAB ACTIVE CAMERA");
                return;
            }
        }


        //

        if(_fadeCamRoutine != null)
        {
            StopCoroutine(_fadeCamRoutine);
            _fadeCamRoutine = null;
        }

        _cmNoise.AmplitudeGain = amplitude;
        _cmNoise.FrequencyGain = frequency;


    }

    public void EndCameraShake(float fadeTime = 2f)
    {
        if (_fadeCamRoutine != null) StopCoroutine(_fadeCamRoutine);
        _fadeCamRoutine = StartCoroutine(FadeOutCam(fadeTime));
    }

    private IEnumerator FadeOutCam(float duration)
    {
        float startAmp = _cmNoise.AmplitudeGain;
        float t = 0f; 

        while (t < duration)
        {
            t += Time.deltaTime;
            _cmNoise.AmplitudeGain = Mathf.Lerp(startAmp, 0f, t / duration);
            yield return null;
        }

        _cmNoise.AmplitudeGain = 0f;
        _fadeCamRoutine = null;
    }

    // ui effects for QTE (shake)

    private void LateUpdate()
    {
        if(_QTEshakeIntensity > 0f)
        {
            _uiHolderTransfrom.anchoredPosition = _restPostQTEUI + Random.insideUnitCircle * _QTEshakeIntensity;
        }


    }

    public void StartUIShake(float amount = 10f)
    {
        if(_uiFadeRoutine != null)
        {
            StopCoroutine(_uiFadeRoutine);
            _uiFadeRoutine = null;
        }
        _uiHolderTransfrom.anchoredPosition = _restPostQTEUI;
        _QTEshakeIntensity = amount;


    }

    public void EndUIShake(float fadeTime = 1f)
    {
        if (_uiFadeRoutine != null) StopCoroutine(_uiFadeRoutine);
        _uiFadeRoutine = StartCoroutine(FadeUIShake(fadeTime));
    }

    private IEnumerator FadeUIShake(float duration)
    {
        float start = _QTEshakeIntensity;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            _QTEshakeIntensity = Mathf.Lerp(start, 0f, t / duration);
            yield return null;
        }

        _QTEshakeIntensity = 0f;
        _uiHolderTransfrom.anchoredPosition = _restPostQTEUI;
        _uiFadeRoutine = null;
    }


    public void UpdateStealth(int amt)
    {
        currentStealth = Mathf.Clamp(currentStealth + amt, 0, maxStealth);

        Debug.Log("current stealth is " + currentStealth);
    }

}
