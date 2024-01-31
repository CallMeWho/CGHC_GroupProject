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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetBoundSize();
        }
    }

    private void SetBoundSize()
    {
        if (GameInfo.CurrentSceneName == "Cave")
        {
            CaveCameraBound.pathCount = 0;

            newPoints[0] = TerrainInfo.BoundaryMinPoint;
            newPoints[1] = new Vector2(TerrainInfo.BoundaryMinPoint.x, TerrainInfo.BoundaryMaxPoint.y);
            newPoints[2] = TerrainInfo.BoundaryMaxPoint;
            newPoints[3] = new Vector2(TerrainInfo.BoundaryMaxPoint.x, TerrainInfo.BoundaryMinPoint.y);

            CaveCameraBound.SetPath(0, newPoints);
        }
    }

    private void SetCameraBoundary()
    {
        switch (GameInfo.CurrentSceneName)
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

    private void SetFollowPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Transform playerTransform = player.transform;

            if (playerTransform != null)
            {
                cinemachine.Follow = playerTransform;
                cinemachine.LookAt = playerTransform;
            }
        }
    }
}
