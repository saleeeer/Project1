using UnityEngine;

public class PlanetSelectionFeedback : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject arrowPrefab;

    [Header("Settings")]
    public float distanceFromPlanet = 1.5f;

    [Header("Animation")]
    public float moveAmount = 0.15f;
    public float moveSpeed = 3f;

    GameObject[] arrows;
    Vector3[] originalPositions;

    PlanetData planet;

    void Awake()
    {
        planet = GetComponent<PlanetData>();

        CreateArrows();

        SetArrowsActive(false);
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        bool isSelected =
            GameManager.Instance.selectedOriginPlanet == planet;

        SetArrowsActive(isSelected);

        if (isSelected)
        {
            AnimateArrows();
        }
    }

    void CreateArrows()
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning(
                "PlanetSelectionFeedback: No hay arrowPrefab asignado en " +
                gameObject.name
            );

            return;
        }

        arrows = new GameObject[4];
        originalPositions = new Vector3[4];

        Vector3[] positions =
        {
            Vector3.up,
            Vector3.right,
            Vector3.down,
            Vector3.left
        };

        // La flecha original apunta hacia ABAJO.
        // Todas apuntan hacia el centro del planeta.
        float[] rotations =
        {
            0f,
            270f,
            180f,
            90f
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject arrow =
                Instantiate(
                    arrowPrefab,
                    transform
                );

            arrow.transform.localPosition =
                positions[i] * distanceFromPlanet;

            arrow.transform.localRotation =
                Quaternion.Euler(
                    0,
                    0,
                    rotations[i]
                );

            arrows[i] = arrow;
            originalPositions[i] = arrow.transform.localPosition;
        }
    }

    void AnimateArrows()
    {
        if (arrows == null)
            return;

        float offset =
            Mathf.Sin(Time.time * moveSpeed) * moveAmount;

        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] == null)
                continue;

            Vector3 newPosition =
                originalPositions[i];

            // Arriba: hacia arriba / hacia el planeta
            if (i == 0)
            {
                newPosition.y += offset;
            }

            // Derecha: hacia derecha / hacia el planeta
            else if (i == 1)
            {
                newPosition.x += offset;
            }

            // Abajo: dirección inversa
            else if (i == 2)
            {
                newPosition.y -= offset;
            }

            // Izquierda: dirección inversa
            else if (i == 3)
            {
                newPosition.x -= offset;
            }

            arrows[i].transform.localPosition =
                newPosition;
        }
    }

    void SetArrowsActive(bool active)
    {
        if (arrows == null)
            return;

        foreach (GameObject arrow in arrows)
        {
            if (arrow != null)
                arrow.SetActive(active);
        }
    }
}