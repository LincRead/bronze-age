using UnityEngine;
using System.Collections;

public class ClickActionIndicator : MonoBehaviour
{
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;

    [Header("Click action sprites")]
    public Sprite attackSprite;
    public Sprite moveSprite;

    private float bounceTime = 0.25f;

    void Start()
    {
        _transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    private void OnEnable()
    {
        EventManager.StartListening("ActivateAttackIndicator", ActivateAttackIndicator);
        EventManager.StartListening("ActivateMoveUnitsIndicator", ActivateMoveUnitsIndicator);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ActivateAttackIndicator", ActivateAttackIndicator);
        EventManager.StopListening("ActivateMoveUnitsIndicator", ActivateMoveUnitsIndicator);
    }

    public void ActivateAttackIndicator()
    {
        ActivateBounceEffect(attackSprite);
    }

    public void ActivateMoveUnitsIndicator()
    {
        ActivateBounceEffect(moveSprite);
    }

    public void ActivateBounceEffect(Sprite sprite)
    {
        _transform.position = PlayerManager.mousePosition;

        _spriteRenderer.sprite = sprite;
        _spriteRenderer.enabled = true;

        LeanTween.scale(gameObject, new Vector3(0.75f, 0.75f, 1.0f), bounceTime);
        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1.0f), bounceTime).setDelay(bounceTime);

        CancelInvoke();
        Invoke("Hide", 0.5f);
    }

    void Hide()
    {
        _spriteRenderer.enabled = false;
    }
}
