using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHolder : MonoBehaviour
{
    public static CanvasHolder Instance;

    [SerializeField]
    public GameObject InGameCanvas;
    public GameObject PauseCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
