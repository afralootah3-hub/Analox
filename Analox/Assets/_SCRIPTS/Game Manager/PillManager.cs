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
    // new qte 
    public Transform startPoint;
    public Transform endPoint;

    public RectTransform greenZone;
    public RectTransform yellowZone;
    public float qteSpeed = 100f;
    private float _qteDirection;
    public RectTransform pointerTransform;
    private Vector3 _QteTargetPos;

    private bool _qteHitGreen;
    private bool _qteHitYellow;
    private bool _qteFailed;
    private bool _qteActive = false; 


    

    public CanvasGroup qteGroup;
 

    private void Start()
    {
        currentSanity = maxSanity;
        qteGroup.alpha = 0;
        pillAmount.text = pillsLeft.ToString();
        vignetteEffect.alpha = 0;
        _SanityEffect.SetFloat("_Blend", 0);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0;
        _QteTargetPos = endPoint.position;

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
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            StartQTE();
            qteGroup.alpha = 1;
        }


        //if (_qteActive)
        //{
        //    StartQTE();
        //}
        StartQTE();
        qteGroup.alpha = 1;
        


    }

    private void StartQTE()
    {
        

        // ping pong across bar 

        if (Vector3.Distance(pointerTransform.position, startPoint.position) < 0.1f)
        {
            _QteTargetPos = endPoint.position;
            _qteDirection = 1f;
        }
        else if (Vector3.Distance(pointerTransform.position, endPoint.position) < 0.1f)
        {
            _QteTargetPos = startPoint.position;
            _qteDirection = -1f;
        }

        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position,
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
        _qteActive = false;
        

    }


}
