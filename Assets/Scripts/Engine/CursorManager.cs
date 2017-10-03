using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour
{
    enum CURSOR_STATE
    {
        NONE,
        CAMERA_MOVEMENT,
        ACTION_BUILD,
        HARVEST,
        ATTACK,
        RALLY_POINT
    }

    private CURSOR_STATE state = CURSOR_STATE.NONE;
    private CURSOR_STATE lastState = CURSOR_STATE.NONE;

    [Header("Cursor textures")]
    public Texture2D defaultTexture;
    public Texture2D moveCameraTexture;
    public Texture2D buildTexture;
    public Texture2D chopTexture;
    public Texture2D mineTexture;
    public Texture2D gatherTexture;
    public Texture2D farmTexture;
    public Texture2D attackTexture;
    public Texture2D ralyPointTexture;

    void Start()
    {
        Cursor.SetCursor(defaultTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnEnable()
    {
        EventManager.StartListening("SetDefaultCursor", SetToDefaultCursor);
        EventManager.StartListening("ChangeToMoveCameraCursor", SetToMoveCameraCursor);
        EventManager.StartListening("SetBuildCursor", SetToBuildCursor);
        EventManager.StartListening("CancelMoveCameraCursor", CancelMoveCameraCursor);
        EventManager.StartListening("SetChopCursor", SetToChopCursor);
        EventManager.StartListening("SetMineCursor", SetToMineCursor);
        EventManager.StartListening("SetGatherCursor", SetToGatherCursor);
        EventManager.StartListening("SetFarmCursor", SetToFarmCursor);
        EventManager.StartListening("SetAttackCursor", SetToAttackCursor);
        EventManager.StartListening("SetRallyPointCursor", SetToRallyPointCursor);
    }

    private void OnDisable()
    {
        EventManager.StopListening("SetDefaultCursor", SetToDefaultCursor);
        EventManager.StopListening("ChangeToMoveCameraCursor", SetToMoveCameraCursor);
        EventManager.StopListening("SetBuildCursor", SetToBuildCursor);
        EventManager.StopListening("CancelMoveCameraCursor", CancelMoveCameraCursor);
        EventManager.StopListening("SetChopCursor", SetToChopCursor);
        EventManager.StopListening("SetMineCursor", SetToMineCursor);
        EventManager.StopListening("SetGatherCursor", SetToGatherCursor);
        EventManager.StopListening("SetFarmCursor", SetToFarmCursor);
        EventManager.StopListening("SetAttackCursor", SetToAttackCursor);
        EventManager.StopListening("SetRallyPointCursor", SetToRallyPointCursor);
    }

    void LateUpdate()
    {
        // Todo check why we need this
        // Used for holding over building that needs construction
        // Todo change this
        // "Blinks" when going from building state to holding over building not yet constructed
        if (PlayerManager.instance._controllerSelecting.showSelectBox)
            SetToDefaultCursor();
    }

    public void SetToDefaultCursor()
    {
        // Never show while moving camera around
        if (state == CURSOR_STATE.CAMERA_MOVEMENT || state == CURSOR_STATE.NONE)
            return;

        ChangeToDefaultCursor();
    }

    void ChangeToDefaultCursor()
    {
        Cursor.SetCursor(defaultTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.NONE;
        lastState = state;
    }

    public void SetToMoveCameraCursor()
    {
        Cursor.SetCursor(moveCameraTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.CAMERA_MOVEMENT;
    }

    public void CancelMoveCameraCursor()
    {
        // Change to whatever was the last state before starting to move camera
        if (lastState == CURSOR_STATE.NONE)
        {
            ChangeToDefaultCursor();
        }

        else
            ChangeToBuildCursor();
    }

    public void SetToBuildCursor()
    {
        // Never show while moving camera around
        if (state == CURSOR_STATE.CAMERA_MOVEMENT || state == CURSOR_STATE.ACTION_BUILD)
            return;

        ChangeToBuildCursor();
    }

    public void ChangeToBuildCursor()
    {
        Cursor.SetCursor(buildTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.ACTION_BUILD;
        lastState = state;
    }

    public void SetToChopCursor()
    {
        Cursor.SetCursor(chopTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.HARVEST;
    }

    public void SetToMineCursor()
    {
        Cursor.SetCursor(mineTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.HARVEST;
    }

    public void SetToGatherCursor()
    {
        Cursor.SetCursor(gatherTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.HARVEST;
    }

    public void SetToFarmCursor()
    {
        Cursor.SetCursor(farmTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.HARVEST;
    }

    public void SetToAttackCursor()
    {
        Cursor.SetCursor(attackTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.ATTACK;
    }

    public void SetToRallyPointCursor()
    {
        Cursor.SetCursor(attackTexture, Vector2.zero, CursorMode.Auto);
        state = CURSOR_STATE.RALLY_POINT;
    }
}
