using UnityEngine;
using UnityEngine.InputSystem;

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

    public KeyCode keyToPress = KeyCode.Space;
    public CanvasGroup qteGroup;
    private bool _qteSuccess = false;
    private bool _qteActive = false;
    public float targetAngle = 90f; //this gets randomized each time

    private void Start()
    {
        qteGroup.alpha = 0;
        
    }

    void StartQTE()
    {
        _qteActive = true;
        _needleAngle = 0;
        _qteSuccess = false;
        targetAngle = Random.Range(60f, 300f);
        targetZone.localRotation = Quaternion.Euler(0, 0, -targetAngle);
        qteGroup.alpha = 1; 

    }

    private void Update()
    {
        //temp test trigger
        if (!_qteActive && Keyboard.current.spaceKey.wasPressedThisFrame) StartQTE();

        if (!_qteActive) return;

        //spin needle 
        _needleAngle += needleSpeed * Time.deltaTime;
        needle.localRotation = Quaternion.Euler(0, 0, -_needleAngle);

        


    }



}
