using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

public class PillManager : MonoBehaviour
{
    public static PillManager instance { get; set; }

    [SerializeField] private AudioClip _breathSound; 
    [SerializeField] private Material _SanityEffect;
    private AudioSource _audioSource;

    [Header("This Script manages: pill tracker, quickTimeEvent activation, Sanity stuff")]
    [Space(20)]
    [Header("SETTINGS")]
    public int pillsLeft;
    public int currentSanity = 5;
    public int maxSanity = 5;
    // sanity increments visibility of the vignette , pill resets the vignette to 0
    public TextMeshProUGUI pillAmount; //debug//
    public CanvasGroup vignetteEffect; //debug temp effect//
    private float _sanityIncrement; // automatically calculated based on maxSanity

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

    private void Start()
    {
        currentSanity = maxSanity;
        qteGroup.alpha = 0;
        pillAmount.text = pillsLeft.ToString();
        vignetteEffect.alpha = 0;
        _SanityEffect.SetFloat("_Blend", 0);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0;

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
            _SanityEffect.SetFloat("_Blend", -alphaamt);
            _audioSource.volume = -alphaamt;

            //debug
            //_audioSource.Play();

        }

    }

    [YarnFunction("CheckSanity")]
    public static int SanityIs(int sanity)
    {
        return PillManager.instance.currentSanity;
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
            //vignetteEffect.alpha = 0;
            SetSanity(1, true);
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
