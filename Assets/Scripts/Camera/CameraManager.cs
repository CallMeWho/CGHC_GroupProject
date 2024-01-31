using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera Camera;
    public CinemachineVirtualCamera VirtualCamera;
    public PolygonCollider2D CompanyCameraBound;
    public PolygonCollider2D CaveCameraBound;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    [SerializeField] public TerrainInfo TerrainInfo;

    private CinemachineConfiner2D confiner;
    private CinemachineVirtualCamera cinemachine;
    private Vector2[] newPoints = new Vector2[4];
    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        confiner = VirtualCamera.GetComponent<CinemachineConfiner2D>();
        cinemachine = VirtualCamera.GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        SetBoundSize();
    }

    private void Update()
    {
        SetCameraBoundary();
        SetFollowPlayer();
        SetAudioListener();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetBoundSize();
        }
    }

    // Set the size of the camera boundary for the Cave scene.
    private void SetBoundSize()
    {
        if (GameInfo != null && TerrainInfo != null && GameInfo.CurrentSceneName == "Cave")
        {
            CaveCameraBound.pathCount = 0;

            newPoints[0] = TerrainInfo.BoundaryMinPoint;
            newPoints[1] = new Vector2(TerrainInfo.BoundaryMinPoint.x, TerrainInfo.BoundaryMaxPoint.y);
            newPoints[2] = TerrainInfo.BoundaryMaxPoint;
            newPoints[3] = new Vector2(TerrainInfo.BoundaryMaxPoint.x, TerrainInfo.BoundaryMinPoint.y);

            CaveCameraBound.SetPath(0, newPoints);
        }
    }

    // Set the camera boundary based on the current scene.
    private void SetCameraBoundary()
    {
        if (confiner != null)
        {
            switch (GameInfo?.CurrentSceneName)
            {
                case "Company":
                    confiner.m_BoundingShape2D = CompanyCameraBound;
                    break;

                case "Cave":
                    confiner.m_BoundingShape2D = CaveCameraBound;
                    break;

                default:
                    break;
            }
        }
    }

    // Set the camera to follow and look at the player.
    private void SetFollowPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Transform playerTransform = player.transform;

            if (playerTransform != null && cinemachine != null)
            {
                cinemachine.Follow = playerTransform;
                cinemachine.LookAt = playerTransform;
            }
        }
    }

    // Set the camera audio listener component.
    private void SetAudioListener()
    {
        Camera cameraObject = Camera.GetComponent<Camera>();

        if (!cameraObject.GetComponent<AudioListener>())
        {
            cameraObject.gameObject.AddComponent<AudioListener>();
        }
    }
}
