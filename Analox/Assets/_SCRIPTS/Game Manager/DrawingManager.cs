using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public Camera drawingCam;
    public GameObject brushRed;
    public GameObject strokeContainer;

    private LineRenderer _lineRenderer;
    private Vector3 _lastPos;

    
    

    //ad a new point to the current strokes linerender at the given world pos
    void AddPoint(Vector3 pointPos)
    {
        _lineRenderer.positionCount++; //add 1 to linerrender
        int positionIndex = _lineRenderer.positionCount - 1; //get the index of the new point 
        _lineRenderer.SetPosition(positionIndex, pointPos); //place new point at the given pos
    }

    //converts current mouse screen pos to world pos and adds it as a stroke point
    void PointToMousePos()
    {


        //screen space pos using mouse xy and cameras z distance
        Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(drawingCam.transform.position.z));

        //converts screen pos to world space
        Vector3 mousePosition = drawingCam.ScreenToWorldPoint(ScreenPosition);


        // lock z pos to match stroke container so all strokes are on the same plane
        mousePosition.z = strokeContainer.transform.position.z;
        if (_lastPos != mousePosition)
        {
            AddPoint(mousePosition);
            _lastPos = mousePosition;

        }


    }

    

    void Update()
    {
            
           Drawing();

        Debug.Log(MenusManager.instance._canDraw);
        

        if(drawingCam == null)
        {
            drawingCam = GameObject.FindGameObjectWithTag("drawCam").GetComponent<Camera>();
            return;
        }

    }

    //drawing logic
    void Drawing()
    {

        if (MenusManager.instance._canDraw)
        {

            //on first frame pressed mouse, create new stroke (change to new input system)
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {

                CreateBrushRed();

                //if(brushCount == 2)
                //{
                //    CreateBrushWhite();
                //}


            }
            else if (Input.GetKey(KeyCode.Mouse0)) //while held down, keep additng points to current stroke
            {
                PointToMousePos();
            }
            else //when release button stop drawing by resettign _lineRenderer
            {
                _lineRenderer = null;
            }
        }
    }

    void CreateBrushRed()
    {

        float zDepth = drawingCam.WorldToScreenPoint(strokeContainer.transform.position).z;
        GameObject brushInstance = Instantiate(brushRed);
        brushInstance.transform.SetParent(strokeContainer.transform);
        _lineRenderer = brushInstance.GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.sortingLayerName = "Map";
        _lineRenderer.sortingOrder = 1;

        //you need 2 points to start a line renderer
        Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(drawingCam.transform.position.z));

        Vector3 mousePos = drawingCam.ScreenToWorldPoint(screenPos);

        mousePos.z = strokeContainer.transform.position.z;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, mousePos);
        _lineRenderer.SetPosition(1, mousePos);
        _lastPos = mousePos;

        //_lineRenderer.startWidth = 0.05f;
        //_lineRenderer.endWidth = 0.05f;
    }

    // change this to an eraser and detect if theres a line renderer within the trigger zone and if there is destory it
    public void ClearCanvas() //destroy all linerenderer objecst
    {
        LineRenderer[] allstrokes = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None);

        foreach (LineRenderer stroke in allstrokes)
        {
            Destroy(stroke.gameObject);
        }

    }

}
