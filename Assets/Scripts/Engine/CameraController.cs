using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public static CameraController Manager;

    public float scrollSpeed = 1f;

    float timeButtonDownBeforeScroll = 0.1f;
    float timeSinceButtonDown = 0.0f;

    Vector2 oldMousePosition = Vector2.zero;

    private float rightBound;
    private float leftBound;
    private float topBound;
    private float bottomBound;

    bool movingCamera = false;

    [HideInInspector]
    public Transform _myTransform;

    Grid _grid;
    ClickIndicator _clickIndicator;

    // Use this for initialization
    void Start()
    {
        Manager = this;

        _myTransform = GetComponent<Transform>();
        _grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        _clickIndicator = WorldManager.Manager._clickIndicator;

        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        float extraSpace = 2f;
        leftBound = (float)(horzExtent - (_grid.GetGridWorldSizeX() / 2)) - extraSpace;
        rightBound = (float)((_grid.GetGridWorldSizeX() / 2) - horzExtent) + extraSpace;
        bottomBound = (float)(vertExtent) - extraSpace;
        topBound = (float)((_grid.GetGridWorldSizeY()) - vertExtent) + extraSpace;
        //Vector3.up* gridWorldSize.y / 2
    }

    // Update is called once per frame
    void Update()
    {
        if(!MovingCameraUsingMouse())
            if (!MovingCameraUsingKeys())
                Reset();
    }

    bool MovingCameraUsingMouse()
    {
        if (Input.GetMouseButtonDown(2))
        {
            oldMousePosition = Input.mousePosition;

            return false;
        }

        else if (Input.GetMouseButtonUp(2))
        {
            if (movingCamera)
                _clickIndicator.DeactivateMoveCamera();

            return false;
        }

        else if (Input.GetMouseButton(2) /*&& !gameController.IsHoveringUI()*/)
        {
            Vector2 newMousePosition = Input.mousePosition;

            if (timeSinceButtonDown > timeButtonDownBeforeScroll && newMousePosition != oldMousePosition)
            {
                Vector3 newCameraPos = _myTransform.position;
                newCameraPos += transform.TransformDirection((Vector3)((oldMousePosition - newMousePosition) * Time.deltaTime));
                newCameraPos.x = Mathf.Clamp(newCameraPos.x, leftBound, rightBound);
                newCameraPos.y = Mathf.Clamp(newCameraPos.y, bottomBound, topBound);
                transform.position = newCameraPos;

                movingCamera = true;

                _clickIndicator.ActivateMoveCamera();

                oldMousePosition = newMousePosition;
            }

            else
            {
                oldMousePosition = Input.mousePosition;
            }

            timeSinceButtonDown += Time.deltaTime;

            return true;
        }

        return false;
    }

    bool MovingCameraUsingKeys()
    {
        Vector3 newCameraPos = _myTransform.position;
        bool movingByKey = false;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            newCameraPos.x -= scrollSpeed * Time.deltaTime;
            movingByKey = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            newCameraPos.x += scrollSpeed * Time.deltaTime;
            movingByKey = true;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            newCameraPos.y += scrollSpeed * Time.deltaTime;
            movingByKey = true;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            newCameraPos.y -= scrollSpeed * Time.deltaTime;
            movingByKey = true;
        }

        if(movingByKey)
        {
            newCameraPos.x = Mathf.Clamp(newCameraPos.x, leftBound, rightBound);
            newCameraPos.y = Mathf.Clamp(newCameraPos.y, bottomBound, topBound);
            transform.position = newCameraPos;
            movingCamera = true;
            return true;
        }

        return false;
    }

    void Reset()
    {
        oldMousePosition = Vector2.zero;
        movingCamera = false;
        timeSinceButtonDown = 0.0f;
    }

    public bool IsMoving()
    {
        return movingCamera;
    }
}
