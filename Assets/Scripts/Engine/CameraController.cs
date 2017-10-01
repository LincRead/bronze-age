using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public static CameraController instance;

    [HideInInspector]
    public Transform _transform;

    private Grid _grid;

    public float scrollSpeed = 1f;

    private float timeButtonDownBeforeScroll = 0.1f;
    private float timeSinceMoveButtonPressedDown = 0.0f;

    private Vector2 oldMousePosition = Vector2.zero;

    private float rightBound;
    private float leftBound;
    private float topBound;
    private float bottomBound;

    [HideInInspector]
    public bool currentlyMovingCamera = false;

    void Start()
    {
        instance = this;

        _transform = GetComponent<Transform>();
        _grid = Grid.instance;

        SetupBounds();
    }

    void SetupBounds()
    {
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        float extraSpace = 2f;

        leftBound = (float)(horzExtent - (_grid.GetGridWorldSizeX() / 2)) - extraSpace;
        rightBound = (float)((_grid.GetGridWorldSizeX() / 2) - horzExtent) + extraSpace;
        bottomBound = (float)(vertExtent) - extraSpace;
        topBound = (float)((_grid.GetGridWorldSizeY()) - vertExtent) + extraSpace;
    }

    void Update()
    {
        // Check if camera is moved by mouse or keys, and reset controller if not
        if(!MovingCameraUsingMouseButton() && !MovingCameraUsingKeys())
        {
            Reset();
            MoveCameraWithMousePosition();
        }
    }

    bool MovingCameraUsingMouseButton()
    {
        // Center mouse button pressed down
        if (Input.GetMouseButtonDown(2))
        {
            oldMousePosition = Input.mousePosition;

            return false;
        }

        // Center mouse button released
        else if (Input.GetMouseButtonUp(2))
        {
            if (currentlyMovingCamera)
            {
                EventManager.TriggerEvent("CancelMoveCameraCursor");
            }

            return false;
        }

        // Holding in center mouse button
        else if (Input.GetMouseButton(2))
        {
            Vector2 newMousePosition = Input.mousePosition;

            // After holding down the center mouse button for a certain amount of time, ...
            // ... and we have moved the cursor while holding button down
            if (timeSinceMoveButtonPressedDown > timeButtonDownBeforeScroll 
                && newMousePosition != oldMousePosition)
            {
                Vector3 newCameraPos = _transform.position;
                newCameraPos += transform.TransformDirection((Vector3)((oldMousePosition - newMousePosition) * Time.deltaTime));
                newCameraPos.x = Mathf.Clamp(newCameraPos.x, leftBound, rightBound);
                newCameraPos.y = Mathf.Clamp(newCameraPos.y, bottomBound, topBound);
                transform.position = newCameraPos;

                if(!currentlyMovingCamera)
                    EventManager.TriggerEvent("ChangeToMoveCameraCursor");

                currentlyMovingCamera = true;
            }

            oldMousePosition = newMousePosition;

            timeSinceMoveButtonPressedDown += Time.deltaTime;

            return true;
        }

        return false;
    }

    bool MovingCameraUsingKeys()
    {
        Vector3 newCameraPos = _transform.position;
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
            currentlyMovingCamera = true;
            return true;
        }

        return false;
    }

    void MoveCameraWithMousePosition()
    {
        Vector3 newCameraPos = _transform.position;
        bool moveCamera = false;

        if (Input.mousePosition.x < 1)
        {
            newCameraPos.x -= scrollSpeed * Time.deltaTime;
            moveCamera = true;
        }

        if(Input.mousePosition.x >= Screen.width - 1 && Input.mousePosition.x <= Screen.width + 1)
        {
            newCameraPos.x += scrollSpeed * Time.deltaTime;
            moveCamera = true;
        }

        if (Input.mousePosition.y < 1)
        {
            newCameraPos.y -= scrollSpeed * Time.deltaTime;
            moveCamera = true;
        }

        if (Input.mousePosition.y >= Screen.height - 1 && Input.mousePosition.y <= Screen.height + 1)
        {
            newCameraPos.y += scrollSpeed * Time.deltaTime;
            moveCamera = true;
        }

        if (moveCamera)
        {
            newCameraPos.x = Mathf.Clamp(newCameraPos.x, leftBound, rightBound);
            newCameraPos.y = Mathf.Clamp(newCameraPos.y, bottomBound, topBound);
            transform.position = newCameraPos;
            currentlyMovingCamera = true;
        }
    }

    void Reset()
    {
        oldMousePosition = Vector2.zero;
        currentlyMovingCamera = false;
        timeSinceMoveButtonPressedDown = 0.0f;
    }
}
