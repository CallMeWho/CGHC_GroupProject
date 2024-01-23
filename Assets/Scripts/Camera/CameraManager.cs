using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager CameraManagerInstance;

    public Camera Camera;
    public CinemachineVirtualCamera VirtualCamera;
    public PolygonCollider2D CaveCameraBound;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    [SerializeField] public TerrainInfo TerrainInfo;

    public bool ChangeBound;
    private CinemachineVirtualCamera Cinemachine;

    private void Awake()
    {
        // camera manager will never be destroyed in the game
        if (CameraManagerInstance == null)
        {
            CameraManagerInstance = this;
            DontDestroyOnLoad(CameraManagerInstance);
        }
        else
        {
            Destroy(CameraManagerInstance);
        }
    }

    private void Start()
    {
        ChangeBound = true;
    }

    private void Update()
    {
        SetBoundSize();
    }

    private void SetBoundSize()
    {
        if (GameInfo.CurrentSceneName == "Company")
        {
            ChangeBound = true;
        }

        if (GameInfo.CurrentSceneName == "Cave" && ChangeBound)
        {
            CaveCameraBound.pathCount = 0;  // remove paths & their points

            // create a new array to store 4 new points
            Vector2[] newPoints = new Vector2[4];

            newPoints[0] = TerrainInfo.BoundaryMinPoint;
            newPoints[1] = new Vector2(TerrainInfo.BoundaryMinPoint.x, TerrainInfo.BoundaryMaxPoint.y); // upper left point
            newPoints[2] = TerrainInfo.BoundaryMaxPoint;
            newPoints[3] = new Vector2(TerrainInfo.BoundaryMaxPoint.x, TerrainInfo.BoundaryMinPoint.y); // lower right point
            
            CaveCameraBound.SetPath(0, newPoints);  // add point array to bound
            ChangeBound = false;
        }
    }

    private void SetFollowPlayer()
    {
        Cinemachine = VirtualCamera.GetComponent<CinemachineVirtualCamera>();
    }
}