using Core.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatmodeManager : MonoBehaviour
{
    [Header("Level 1 Positions")]
    [SerializeField] private Transform level1Start;
    [SerializeField] private Transform level1Predefined;
    [SerializeField] private Transform level1PCGEnd1;
    [SerializeField] private Transform level1PCGEnd2;

    [Header("Level 2 Positions")]
    [SerializeField] private Transform level2Start;
    [SerializeField] private Transform level2SpecialWeapon;
    [SerializeField] private Transform level2PCGEnd1;
    [SerializeField] private Transform level2PCGEnd2;
    [SerializeField] private Transform level2PCGEnd3;

    [Header("Pause & Camera Control")]
    public PauseController pauseController;
    public GameObject freeCamera;

    private Transform player;
    private bool isFreeCamActive = false;
    private bool showFPS = false;

    // Novas flags
    private bool level1PCGReady = false;
    private bool level2PCGReady = false;

    private readonly object cheatPauseKey = new object();
    private readonly object cheatInputKey = new object();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        HandleCheatKeys();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
        level1PCGReady = false;
        level2PCGReady = false;

        if (pauseController == null)
        {
            pauseController = FindAnyObjectByType<PauseController>();
            if (pauseController == null)
                Debug.LogWarning("CheatModeManager: PauseController not found in scene!");
        }
    }

    private void FindPlayer()
    {
        GameObject foundPlayer = GameObject.FindWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            player = null;
            Debug.LogWarning("CheatModeManager: Player not found in scene!");
        }

        if (freeCamera == null)
        {
            FreeCameraController cam = FindAnyObjectByType<FreeCameraController>();
            if (cam != null)
            {
                freeCamera = cam.gameObject;
            }
            else
            {
                Debug.LogWarning("CheatModeManager: FreeCameraController not found in scene!");
            }
        }
    }

    private void HandleCheatKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Transform startPoint = FindSceneStartPoint();
            if (startPoint != null)
            {
                Teleport(startPoint);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (IsInLevel(1)) Teleport(level1Predefined);
            else if (IsInLevel(2)) Teleport(level2SpecialWeapon);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (IsInLevel(1) && level1PCGReady) Teleport(level1PCGEnd1);
            else if (IsInLevel(2) && level2PCGReady) Teleport(level2PCGEnd1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (IsInLevel(1) && level1PCGReady) Teleport(level1PCGEnd2);
            else if (IsInLevel(2) && level2PCGReady) Teleport(level2PCGEnd2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (IsInLevel(2) && level2PCGReady) Teleport(level2PCGEnd3);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleFreeCamera();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            bool paused = !pauseController.IsPaused;
            pauseController.Pause(cheatPauseKey, paused);
            pauseController.SetInputActive(cheatInputKey, true);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            showFPS = !showFPS;
        }
    }

    private void Teleport(Transform target)
    {
        if (player != null && target != null)
        {
            Debug.Log($"Teleportando para: {target.name} ({target.position})");
            player.position = target.position;
        }
        else
        {
            Debug.LogWarning("CheatModeManager: Player ou destino estão nulos.");
        }
    }

  
    private bool IsInLevel(int level)
    {
       return SceneManager.GetActiveScene().name.Contains("Level" + level.ToString("D2"));
    }


private void ToggleFreeCamera()
    {
        isFreeCamActive = !isFreeCamActive;
        freeCamera?.SetActive(isFreeCamActive);

        Cursor.lockState = isFreeCamActive ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isFreeCamActive;
    }

    private void OnGUI()
    {
        if (showFPS)
        {
            float fps = 1f / Time.unscaledDeltaTime;
            GUI.Label(new Rect(10, 10, 100, 25), "FPS: " + Mathf.RoundToInt(fps));
        }
    }

    public void SetPCGEndPoint(int index, Transform endTransform)
    {
        if (endTransform == null)
        {
            Debug.LogWarning("SetPCGEndPoint: endTransform é nulo.");
            return;
        }

        if (endTransform.gameObject.scene != SceneManager.GetActiveScene())
        {
            Debug.LogWarning($"SetPCGEndPoint ignorado: objeto {endTransform.name} pertence à cena {endTransform.gameObject.scene.name}, mas a cena ativa é {SceneManager.GetActiveScene().name}");
            return;
        }

        Debug.Log($"SetPCGEndPoint: index {index}, transform {endTransform?.name}");

        if (IsInLevel(1))
        {
            if (index == 1) level1PCGEnd1 = endTransform;
            else if (index == 2) level1PCGEnd2 = endTransform;

            level1PCGReady = true;
            
        }
        else if (IsInLevel(2))
        {
            if (index == 1) level2PCGEnd1 = endTransform;
            else if (index == 2) level2PCGEnd2 = endTransform;
            else if (index == 3) level2PCGEnd3 = endTransform;

            level2PCGReady = true;
          
        }
    }

    private Transform FindSceneStartPoint()
    {
        GameObject startObj = GameObject.Find("StartPoint");
        return startObj != null ? startObj.transform : null;
    }


}
