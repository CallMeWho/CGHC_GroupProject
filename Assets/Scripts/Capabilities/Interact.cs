using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField] public GameObject InteractIcon;
    [SerializeField, Range(-2,2)] private float iconOffsetX = 1.2f;
    [SerializeField, Range(-2, 2)] private float iconOffsetY = 1f;

    private bool isInteracting;
    private bool isTouching;
    private BoxCollider2D playerCol;
    private Vector2 boxSize;
    private Vector3 iconScale;

    private void Awake()
    {
        playerCol = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        boxSize = playerCol.size;
        iconScale = InteractIcon.transform.localScale;
    }

    private void Update()
    {
        isInteracting = input.RetrieveInteractInput();

        UpdateIconPosition();
        CheckInteract();
    }

    public void ShowInteractIcon()
    {
        InteractIcon.SetActive(true);
    }

    public void HideInteractIcon() 
    {
        InteractIcon.SetActive(false);
    }

    private void CheckInteract()
    {
        if (isInteracting)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxSize, 0, Vector2.zero);

            if (hits.Length > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.transform.GetComponent<InteractableObject>())
                    {
                        hit.transform.GetComponent<InteractableObject>().Interact();
                        return;
                    }
                }
            }
        }
    }

    private void UpdateIconPosition()
    {
        InteractIcon.transform.position = transform.position + new Vector3(iconOffsetX, iconOffsetY, -2);
        InteractIcon.transform.rotation = Quaternion.identity;

        bool isFlipped = transform.localScale.x < 0;
        float x = iconScale.x;
        float y = iconScale.y;
        float z = iconScale.z;
        InteractIcon.transform.localScale = isFlipped? new Vector3(-x, y, z) : new Vector3(x, y, z);
    }
}
