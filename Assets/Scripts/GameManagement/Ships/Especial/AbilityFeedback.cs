using UnityEngine;

public class AbilityFeedback : MonoBehaviour
{
    [Header("One Shot Effects")]
    public GameObject oneShotPrefab;

    [Header("Persistent Effect")]
    public GameObject persistentPrefab;

    [Header("Anchors")]
    public Transform effectAnchor;

    GameObject persistentInstance;

    void Awake()
    {
        if (persistentPrefab != null)
        {
            persistentInstance =
                Instantiate(
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
        if (persistentInstance == null)
            return;

        persistentInstance.SetActive(active);
    }

    public void PlayOneShot()
    {
        if (oneShotPrefab == null)
            return;

        GameObject effect =
            Instantiate(
                oneShotPrefab,
                GetAnchorPosition(),
                Quaternion.identity
            );

        Destroy(
            effect,
            3f
        );
    }

    public void PlayOneShot(
        GameObject prefab,
        Vector3 position
    )
    {
        if (prefab == null)
            return;

        GameObject effect =
            Instantiate(
                prefab,
                position,
                Quaternion.identity
            );

        Destroy(
            effect,
            3f
        );
    }
}