using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField] public GameObject InteractIcon;

    private bool isInteracting;
    private BoxCollider2D playerCol;
    private Vector2 boxSize;

    private void Awake()
    {
        playerCol = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        boxSize = playerCol.size;
    }

    private void Update()
    {
        isInteracting = input.RetrieveInteractInput();

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
}
