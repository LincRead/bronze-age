using UnityEngine;
using System.Collections;

public class ClickActionIndicator : MonoBehaviour
{
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;

    [Header("Click action sprites")]
    public Sprite attackSprite;
    public Sprite moveSprite;
    public Sprite setRallyPointSprite;

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
        EventManager.StartListening("ActivateRallyPointIndicator", ActivatRallyPointSetIndicator);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ActivateAttackIndicator", ActivateAttackIndicator);
        EventManager.StopListening("ActivateMoveUnitsIndicator", ActivateMoveUnitsIndicator);
        EventManager.StopListening("ActivateRallyPointIndicator", ActivatRallyPointSetIndicator);
    }

    public void ActivateAttackIndicator()
    {
        _transform.position = PlayerManager.mousePosition;
        ActivateBounceEffect(attackSprite);
    }

    public void ActivateMoveUnitsIndicator()
    {
        _transform.position = PlayerManager.mousePosition;
        ActivateBounceEffect(moveSprite);
    }

    public void ActivatRallyPointSetIndicator()
    {
        _transform.position = PlayerManager.mousePosition + new Vector2(0.08f, 0.10f);
        ActivateBounceEffect(setRallyPointSprite);
    }

    public void ActivateBounceEffect(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.enabled = true;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.0f);
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
