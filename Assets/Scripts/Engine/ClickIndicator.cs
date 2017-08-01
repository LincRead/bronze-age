using UnityEngine;
using System.Collections;

public class ClickIndicator : MonoBehaviour
{
    enum INDICATOR_STATE
    {
        NONE,
        CAMERA_MOVEMENT,
        UNIT_ACTION
    }

    INDICATOR_STATE state = INDICATOR_STATE.NONE;

    public float showActionForSecs = 2f;

    public Texture2D defaultCursorTexture;
    public Sprite attackSprite;
    public Sprite moveSprite;
    public Texture2D moveCameraTexture;
    public Texture2D buildTexture;

    Transform _transform;

    public GameObject indicatorBouncePrefab;
    IndicatorBounce _indicatorBounce;

    // Use this for initialization
    void Start()
    {
        Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);

        _transform = GetComponent<Transform>();

        SpriteRenderer _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;

        _indicatorBounce = indicatorBouncePrefab.GetComponent<IndicatorBounce>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (WorldManager.Manager._objectSelection.isSelecting)
            ChangeToDefaultCursor();

         _transform.position = WorldManager.Manager.GetMousePosition();
    }

    public void ShowBuildingPlacementindicator(Vector2 pos)
    {
        if (state == INDICATOR_STATE.CAMERA_MOVEMENT)
            return;

        Cursor.SetCursor(buildTexture, Vector2.zero, CursorMode.Auto);
        state = INDICATOR_STATE.UNIT_ACTION;
    }

    public void ChangeToDefaultCursor()
    {
        if (state == INDICATOR_STATE.CAMERA_MOVEMENT)
            return;

        Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);
        state = INDICATOR_STATE.NONE;
    }

    public void ActivateAttack(Vector2 pos)
    {
        if(state == INDICATOR_STATE.NONE)
            _indicatorBounce.ActivateBounceEffect(WorldManager.Manager.GetMousePosition(), attackSprite);
    }

    public void ActivateMoveSprite(Vector2 pos)
    {
        if (state == INDICATOR_STATE.NONE)
            _indicatorBounce.ActivateBounceEffect(WorldManager.Manager.GetMousePosition(), moveSprite);
    }

    public void ActivateBounceEffect()
    {
        LeanTween.scale(gameObject, new Vector3(0.75f, 0.75f, 1.0f), 0.15f);
        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.25f).setDelay(0.15f);

        // Todo wait for finish before going back to default again
        // Todo can cancel invoke by calling this again
    }

    public void ActivateMoveCamera()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;
        Cursor.SetCursor(moveCameraTexture, Vector2.zero, CursorMode.Auto);
        state = INDICATOR_STATE.CAMERA_MOVEMENT;
    }

    public void DeactivateMoveCamera()
    {
        Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);
        state = INDICATOR_STATE.NONE;
    }
}
