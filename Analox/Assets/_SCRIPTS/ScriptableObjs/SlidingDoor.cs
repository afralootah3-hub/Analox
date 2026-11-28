using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private Transform _closedPos;
    [SerializeField] private Transform _openPos;
    [SerializeField] private float _duration;
    private Coroutine _slideDoor;
    private bool _doorIsOpen;
    private bool _isOpening;

    private void Start()
    {
        //_openPos = this.transform;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (_slideDoor != null) StopCoroutine(_slideDoor);
            _doorIsOpen = true;
            _slideDoor = StartCoroutine(SlideDoor(_closedPos.transform.position));
            Debug.Log(other.name + "Collided with door trigger");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if (_slideDoor != null) StopCoroutine(_slideDoor);
            _doorIsOpen = false;
            _slideDoor = StartCoroutine(SlideDoor(_openPos.transform.position));
            Debug.Log(other.name + "exited door trigger");
        }
    }

    private IEnumerator SlideDoor(Vector3 destination)
    {
        
        Vector3 startPos = this.transform.position;
        float elapsed = 0;
        
        _isOpening = true;
        while(elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _duration);
            float easeT = Mathf.SmoothStep(0f, 1f, t);
            // interpolate

            this.transform.position = Vector3.Lerp(startPos, destination, easeT);
            yield return null;
        }

        // snap 
        this.transform.position = destination;
        _slideDoor = null;
        _isOpening = false;

    }





}
