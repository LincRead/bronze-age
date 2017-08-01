using UnityEngine;
using System.Collections;

public class IndicatorBounce : MonoBehaviour
{
    Transform _transform;
    SpriteRenderer _spriteRenderer;

    // Use this for initialization
    void Start()
    {
        _transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    public void ActivateBounceEffect(Vector2 pos, Sprite sprite)
    {
        _transform.position = pos;

        _spriteRenderer.sprite = sprite;
        _spriteRenderer.enabled = true;

        LeanTween.scale(gameObject, new Vector3(0.75f, 0.75f, 1.0f), 0.25f);
        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.25f).setDelay(0.25f);

        CancelInvoke();
        Invoke("Hide", 0.5f);
    }

    void Hide()
    {
        _spriteRenderer.enabled = false;
    }
}
