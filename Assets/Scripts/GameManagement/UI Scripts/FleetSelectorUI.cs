
using UnityEngine;

public class FleetSelectorUI : MonoBehaviour
{
    public static FleetSelectorUI Instance;

    [Header("UI")]
    public GameObject fleetSelectorPanel;

    [Header("Fleet Settings")]
    [SerializeField]
    private int selectedFleetSize = 1;

    public int SelectedFleetSize => selectedFleetSize;

    // Permite detectar cuándo el jugador selecciona
    // una opción del menú de flotas.
    public int SelectionVersion { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (fleetSelectorPanel != null)
            fleetSelectorPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) ||
            Input.GetKeyDown(KeyCode.RightControl))
        {
            ToggleSelector();
        }

        // Si el juego se pausa por cualquier motivo,
        // cerrar el selector si estaba abierto.
        if (Time.timeScale == 0f &&
            fleetSelectorPanel != null &&
            fleetSelectorPanel.activeSelf)
        {
            fleetSelectorPanel.SetActive(false);
        }
    }

    public void ToggleSelector()
    {
        if (fleetSelectorPanel == null)
            return;

        // No permitir abrir el selector durante pausa,
        // eventos, victoria o derrota.
        if (Time.timeScale == 0f)
        {
            fleetSelectorPanel.SetActive(false);
            return;
        }

        fleetSelectorPanel.SetActive(
            !fleetSelectorPanel.activeSelf
        );
    }

    public void SelectOne()
    {
        SelectFleetSize(1);
    }

    public void SelectFour()
    {
        SelectFleetSize(4);
    }

    public void SelectEight()
    {
        SelectFleetSize(8);
    }

    void SelectFleetSize(int amount)
    {
        selectedFleetSize = amount;

        // Incrementa la versión cada vez que se elige
        // una opción, incluso si es la misma que antes.
        SelectionVersion++;

        Debug.Log(
            "Tamaño de flota seleccionado: " +
            selectedFleetSize
        );

        if (fleetSelectorPanel != null)
            fleetSelectorPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
