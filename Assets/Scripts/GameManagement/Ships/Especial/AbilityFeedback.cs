
using UnityEngine;
using System.Collections;

public class AbilityFeedback : MonoBehaviour
{
    [Header("One Shot Effects")]
    public GameObject oneShotPrefab;

    [Header("Persistent Effect")]
    public GameObject persistentPrefab;

    [Header("Anchors")]
    public Transform effectAnchor;

    [Header("Sprite Feedback")]
    public SpriteRenderer spriteRenderer;

    GameObject persistentInstance;
    Color originalSpriteColor;
    Coroutine flashCoroutine;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalSpriteColor = spriteRenderer.color;

        if (persistentPrefab != null)
        {
            persistentInstance = Instantiate(
                persistentPrefab,
                GetAnchorPosition(),
                Quaternion.identity,
                transform
            );

            persistentInstance.SetActive(false);
        }
    }

    Vector3 GetAnchorPosition()
    {
        if (effectAnchor != null)
            return effectAnchor.position;

        return transform.position;
    }

    public void SetPersistent(bool active)
    {
        Debug.Log("[FEEDBACK] SetPersistent llamado: " + active);

        if (persistentInstance == null)
        {
            Debug.LogWarning("[FEEDBACK] persistentInstance es NULL");
            return;
        }

        persistentInstance.SetActive(active);

        Debug.Log(
            "[FEEDBACK] Estado final: " +
            persistentInstance.activeSelf +
            " | Objeto: " + persistentInstance.name
        );
    }

    public void PlayOneShot()
    {
        if (oneShotPrefab == null)
            return;

        GameObject effect = Instantiate(
            oneShotPrefab,
            GetAnchorPosition(),
            Quaternion.identity
        );

        Destroy(effect, 3f);
    }

    public void PlayOneShot(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
            return;

        GameObject effect = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        Destroy(effect, 3f);
    }

    public void SetSpriteFeedback(bool active, Color color, float alpha)
    {
        if (spriteRenderer == null)
            return;

        if (active)
        {
            spriteRenderer.color = new Color(
                color.r,
                color.g,
                color.b,
                Mathf.Clamp01(alpha)
            );
        }
        else
        {
            StopSpriteFlash();
            RestoreSpriteColor();
        }
    }

    public void PlaySpriteFlash(Color color, float alpha, float duration)
    {
        if (spriteRenderer == null)
            return;

        StopSpriteFlash();

        flashCoroutine = StartCoroutine(
            SpriteFlashRoutine(color, alpha, duration)
        );
    }

    IEnumerator SpriteFlashRoutine(Color color, float alpha, float duration)
    {
        spriteRenderer.color = new Color(
            color.r,
            color.g,
            color.b,
            Mathf.Clamp01(alpha)
        );

        yield return new WaitForSeconds(duration);

        RestoreSpriteColor();
        flashCoroutine = null;
    }

    void StopSpriteFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
    }

    public void RestoreSpriteColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalSpriteColor;
    }
}
