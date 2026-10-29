
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    private enum TutorialStep
    {
        Intro,
        SpawnShip,
        MoveToNeutral,
        CaptureNeutral,
        OrderAttack,
        Combat,
        Finished
    }

    [Header("UI")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI instructionText;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;

    [Header("Tutorial Planets")]
    public PlanetData playerHome;
    public PlanetData neutralPlanet;
    public PlanetData enemyPlanet;

    [Header("Tutorial Setup")]
    public int startingUnits = 20;
    public int startingCredits = 1000;
    public ShipType enemyTutorialShipType = ShipType.Fighter;
    public float enemyDemoAttackRange = 10f;

    [Header("Scene")]
    public string gameplaySceneName = "Level";

    GameManager gm;

    int playerEmpireIndex;
    int enemyEmpireIndex;

    int shipsBeforeSpawn;

    ShipMovement tutorialEnemyShip;
    bool enemySpawned = false;

    TutorialStep currentStep;

    void Start()
    {
        if (actionButton != null)
            actionButton.onClick.AddListener(OnActionButtonPressed);

        StartCoroutine(InitializeTutorial());
    }

    IEnumerator InitializeTutorial()
    {
        yield return null;

        gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("TutorialManager: falta GameManager.");
            yield break;
        }

        if (gm.empires.Count < 2)
        {
            Debug.LogError(
                "TutorialManager necesita dos imperios configurados."
            );
            yield break;
        }

        if (playerHome == null ||
            neutralPlanet == null ||
            enemyPlanet == null)
        {
            Debug.LogError(
                "Asigná Player Home, Neutral Planet y Enemy Planet."
            );
            yield break;
        }

        playerEmpireIndex = gm.playerEmpireIndex;

        if (playerEmpireIndex < 0 ||
            playerEmpireIndex >= gm.empires.Count)
        {
            Debug.LogError("El imperio del jugador no es válido.");
            yield break;
        }

        enemyEmpireIndex = playerEmpireIndex == 0 ? 1 : 0;

        SetupScenario();

        ShowStep(
            TutorialStep.Intro,
            "¡Bienvenido a Battle in the Stars!\n\n" +
            "En este tutorial aprenderas a construir naves, " +
            "mover tu flota, conquistar planetas y combatir " +
            "contra el imperio rival.\n\n" +
            "Completa cada objetivo para avanzar.",
            true,
            "Comenzar"
        );
    }

    void SetupScenario()
    {
        playerHome.SetOwner(playerEmpireIndex);
        neutralPlanet.SetOwner(-1);
        enemyPlanet.SetOwner(enemyEmpireIndex);

        playerHome.units = Mathf.Clamp(
            startingUnits,
            0,
            playerHome.maxUnits
        );

        neutralPlanet.units = 0;
        enemyPlanet.units = 0;

        int creditsNeeded =
            startingCredits - gm.GetCredits(playerEmpireIndex);

        if (creditsNeeded > 0)
        {
            gm.AddCredits(playerEmpireIndex, creditsNeeded);
        }

        enemyPlanet.minUnitsToSend = int.MaxValue;

        enemyPlanet.SetTutorialVisibility(false, false);

        gm.selectedOriginPlanet = null;
        gm.selectedPlanet = null;
    }

    void Update()
    {
        if (gm == null)
            return;

        switch (currentStep)
        {
            case TutorialStep.SpawnShip:

                if (CountPlayerShips() > shipsBeforeSpawn)
                {
                    BeginMoveStep();
                }

                break;

            case TutorialStep.MoveToNeutral:

                if (HasPlayerOrderTo(neutralPlanet))
                {
                    ShowStep(
                        TutorialStep.CaptureNeutral,
                        "¡Orden de movimiento recibida!\n\n" +
                        "Tus naves viajaran al planeta neutral. " +
                        "Cuando lleguen, comenzaran a capturarlo. " +
                        "Espera a que el planeta pase a ser tuyo.",
                        false
                    );
                }

                break;

            case TutorialStep.CaptureNeutral:

                if (neutralPlanet.ownerEmpireIndex ==
                    playerEmpireIndex)
                {
                    BeginEnemyStep();
                }

                break;

            case TutorialStep.OrderAttack:

                if (HasPlayerOrderTo(enemyPlanet))
                {
                    ShowStep(
                        TutorialStep.Combat,
                        "¡La flota esta en camino!\n\n" +
                        "Cuando las naves enemigas estén dentro " +
                        "del rango de ataque, comenzarán a dispararse " +
                        "automaticamente.\n\n" +
                        "Destru la nave defensora del otro imperio. " +
                        "Si necesitás refuerzos, podés construir más " +
                        "naves y volver a dar la orden de ataque.",
                        false
                    );
                }

                break;

            case TutorialStep.Combat:

                if (enemySpawned && tutorialEnemyShip == null)
                {
                    ShowStep(
                        TutorialStep.Finished,
                        "¡Tutorial completado!\n\n" +
                        "Ya sabés construir naves, moverlas, " +
                        "capturar planetas y combatir contra otro imperio.\n\n" +
                        "Ahora estas listo para conquistar la galaxia.",
                        true,
                        "Ir a la partida"
                    );
                }

                break;
        }
    }

    // ================= PASOS =================

    void BeginSpawnStep()
    {
        shipsBeforeSpawn = CountPlayerShips();

        ShowStep(
            TutorialStep.SpawnShip,
            "OBJETIVO 1: CONSTRUIR UNA NAVE\n\n" +
            "1. Hace click izquierdo sobre tu planeta base.\n" +
            "2. Usa el boton de spawn de naves del HUD.\n\n" +
            "El planeta necesita unidades disponibles y " +
            "creditos para construir una nave.",
            false
        );
    }

    void BeginMoveStep()
    {
        ShowStep(
            TutorialStep.MoveToNeutral,
            "OBJETIVO 2: MOVER TU FLOTA\n\n" +
            "Hace click derecho sobre el planeta neutral " +
            "del mapa para enviar tus naves hacia el.\n\n" +
            "El movimiento y la captura se realizarán " +
            "automaticamente.",
            false
        );
    }

    void BeginEnemyStep()
    {
        if (!SpawnTutorialEnemy())
        {
            ShowStep(
                TutorialStep.Finished,
                "No se pudo crear la nave enemiga de práctica.\n\n" +
                "Revisa que los prefabs de ambos imperios estén " +
                "asignados en el GameManager.",
                true,
                "Ir a la partida"
            );

            return;
        }

        enemyPlanet.SetTutorialVisibility(true, true);

        ShowStep(
            TutorialStep.OrderAttack,
            "OBJETIVO 3: ATACAR AL OTRO IMPERIO\n\n" +
            "Hace click derecho sobre el planeta enemigo " +
            "para ordenar el ataque.\n\n" +
            "Hay una nave defensora esperando. " +
            "Las naves comenzarán a combatir automáticamente " +
            "cuando entren en rango.",
            false
        );
    }

    // ================= ENEMIGO DE PRÁCTICA =================

    bool SpawnTutorialEnemy()
    {
        GameObject prefab =
            gm.GetShipPrefab(
                enemyTutorialShipType,
                enemyEmpireIndex
            );

        if (prefab == null)
        {
            Debug.LogError(
                "TutorialManager: no se encontró el prefab enemigo."
            );

            return false;
        }

        GameObject enemyObject = Instantiate(
            prefab,
            enemyPlanet.transform.position,
            Quaternion.identity
        );

        tutorialEnemyShip =
            enemyObject.GetComponent<ShipMovement>();

        if (tutorialEnemyShip == null)
        {
            Destroy(enemyObject);

            Debug.LogError(
                "El prefab enemigo necesita ShipMovement."
            );

            return false;
        }

        tutorialEnemyShip.empireIndex = enemyEmpireIndex;
        tutorialEnemyShip.isPlayerControlled = false;
        tutorialEnemyShip.currentPlanet = enemyPlanet;
        tutorialEnemyShip.targetPlanet = null;
        tutorialEnemyShip.orbitSpeed *= 0.7f;

        ShipCombat enemyCombat =
            enemyObject.GetComponent<ShipCombat>();

        if (enemyCombat != null)
        {
            enemyCombat.attackRange = Mathf.Max(
                enemyCombat.attackRange,
                enemyDemoAttackRange
            );
        }

        enemySpawned = true;

        return true;
    }

    // ================= COMPROBACIONES =================

    int CountPlayerShips()
    {
        int count = 0;

        foreach (ShipMovement ship in gm.allShips)
        {
            if (ship == null)
                continue;

            if (ship.empireIndex == playerEmpireIndex &&
                ship.isPlayerControlled)
            {
                count++;
            }
        }

        return count;
    }

    bool HasPlayerOrderTo(PlanetData planet)
    {
        if (planet == null)
            return false;

        foreach (ShipMovement ship in gm.allShips)
        {
            if (ship == null)
                continue;

            if (ship.empireIndex != playerEmpireIndex ||
                !ship.isPlayerControlled)
            {
                continue;
            }

            if (ship.targetPlanet == planet ||
                ship.currentPlanet == planet)
            {
                return true;
            }
        }

        return false;
    }

    // ================= UI =================

    void ShowStep(
        TutorialStep step,
        string message,
        bool showButton,
        string buttonLabel = ""
    )
    {
        currentStep = step;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        if (instructionText != null)
            instructionText.text = message;

        if (actionButton != null)
            actionButton.gameObject.SetActive(showButton);

        if (actionButtonText != null && showButton)
            actionButtonText.text = buttonLabel;
    }

    public void OnActionButtonPressed()
    {
        if (currentStep == TutorialStep.Intro)
        {
            BeginSpawnStep();
        }
        else if (currentStep == TutorialStep.Finished)
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    void OnDestroy()
    {
        if (actionButton != null)
            actionButton.onClick.RemoveListener(OnActionButtonPressed);
    }
}
