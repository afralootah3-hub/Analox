using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ActionWheel : MonoBehaviour
{
    /*
      A radial action wheel that appears when the player holds down the mouse (called by pCC)
        when button is held, moving the mouse away from the center highlights an option
        releasing over a option runs its function (inspect, scan, info)
        releasing in the center cancels the interaction 
        This script handles the wheel ui and input
    */

    public GameObject wheelHolder;
    [SerializeField] private Image[] slices;
    [SerializeField] private RectTransform cursorDot;

    public float deadZone = 30f;
    public float maxRadius = 100f;
    public Color baseColor = new Color(1, 1, 1, 0.3f);
    public Color highlightColor = Color.blue;

    public bool isOpen = false;
    private Vector2 cursorPos;
    private Action<int> onFinished;
    private PlayerController _playercc;
    private Canvas _canvas;
    private NavMeshAgent _agent;

    

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        wheelHolder.SetActive(false);
        _playercc = FindFirstObjectByType<PlayerController>();
        _agent = _playercc.gameObject.GetComponent<NavMeshAgent>();
    }
    public void OpenWheel(Vector2 screenPos, Action<int> callback)
    {
        _agent.ResetPath();
        onFinished = callback;
        cursorPos = Vector2.zero;
        transform.position = screenPos;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isOpen = true;
        wheelHolder.SetActive(true);
        
    }

    public void CloseWheel(int selected)
    {
        if (isOpen == false) return;

        //gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isOpen = false;
        onFinished(selected);
        //_playercc.setPlayerState(PlayerController.playerState.isIdle);
        wheelHolder.SetActive(false);
        
    }

    private void Update()
    {
        if (isOpen) 
        WheelLogic();
    }

    private void WheelLogic()
    {
        //cursorPos = Vector2.ClampMagnitude(cursorPos + Mouse.current.delta.ReadValue(), maxRadius);
        
        //read raw mouse movement since last frame
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        //add movement to current pos, (scale for canvas resolution)
        cursorPos = cursorPos + (mouseDelta / _canvas.scaleFactor);

        //keep cursor inside circular menu boundary for game feel
        cursorPos = Vector2.ClampMagnitude(cursorPos, maxRadius);
        
        //slices is an array of ui elements
        //int selected = -1; // pick slice, -1 is cancel

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseWheel(-1);
            _playercc.setPlayerState(PlayerController.playerState.isIdle);
            return;
        }

        if(cursorDot != null)
        {
            cursorDot.anchoredPosition = cursorPos;
        }

        int selected = GetHoveredSlice();

        //color the selected slice with the highlight color
        //if selected is -1 nothing gets highlighted
        for (int i = 0; i < slices.Length; i++)
        {
            if(i == selected)
            {
                slices[i].color = highlightColor;
            }
            else
            {
                slices[i].color = baseColor;
            }
        }

        //on release either activate an interaction or close wheel
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CloseWheel(selected);
        }

        //determine which slice cursor is pointing at
        //ignore is cursror is too close to center

        /*
        if(cursorPos.magnitude > deadZone)
        {

            //calculate anlge from straigth down direction to cursorPos and return a value in range of -180- +180 (change later)
            float cursorAngle = Vector2.SignedAngle(Vector2.down, cursorPos) - 180f;
            
            //each slice gets an equal wedge of the circle
            float sliceSize = 360f / slices.Length;

            //convert angle into a slice index
            //substact by half to center the selection zone
            //divide by 360 to get fraction of full circle
            //multiply by total slices to get index
            
            selected = Mathf.Abs((int)((cursorAngle - sliceSize / 2f) / 360f * slices.Length));
           
            // if index equals the total reset to 0 
            if (selected == slices.Length) selected = 0;
        }

        //highlight selected slice, dim the others
        for (int i = 0; i < slices.Length; i++)
        {
            slices[i].color = (i == selected) ? highlightColor : baseColor;
        }

        //on release either activate an interaction or close wheel
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CloseWheel(selected);
        }

        */

    }

    private int GetHoveredSlice()
    {
        //too close to center so cancel
        if (cursorPos.magnitude <= deadZone)
        {
            return -1;
        }
        //normalize turns the cursor pos into DIRECTION only 
        // we only car which way the cursor points and not its distance from the center
        Vector2 cursorDirection = cursorPos.normalized;

        //starts at -1/nothing 
        //this is the index of the closest slice
        int closeIndex = -1;

        // each slice gets a check for how well it macthes the cursor direction, 
        // its like a leniency check, so the cursor doesn't focus on EXACTLLY matches the slices and 
        //allows for some space 
        float lenCheck = -2f;

        //check every slice 
        for (int i = 0; i < slices.Length; i++)
        {
            //direction from wheel center to this slice
            Vector2 sliceDir = slices[i].rectTransform.anchoredPosition.normalized;

            //check for leniency
            float dot = Vector2.Dot(cursorDirection, sliceDir);

            // if this slice matches the cursor direction the most out of all slices, store it as the best index
            if(dot > lenCheck)
            {
                lenCheck = dot;
                closeIndex = i;
            }


        }

        //after checking everything return the closest index/slice
        return closeIndex;

    }



}
