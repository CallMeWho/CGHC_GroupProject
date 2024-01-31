using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player interacts
public class InteractProcess : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField] public GameObject InteractIcon;

    // Offset of the interact icon on each axis
    [SerializeField, Range(-2, 2)] private float iconOffsetX = 1.2f;
    [SerializeField, Range(-2, 2)] private float iconOffsetY = 1f;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private BoxCollider2D playerCollider;    
    private Vector2 colliderSize;    
    private Vector3 iconScale;

    private void Awake()
    {
        playerCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        colliderSize = playerCollider.size;
        iconScale = InteractIcon.transform.localScale;
    }

    private void Update()
    {
        UpdateKeyPressStatus();
        UpdateInteractionStatus();

        CheckInteract();
        UpdateIconPosition();
    }

    #region Public Callable Functions
    public void ShowInteractIcon()
    {
        GameInfo.IsTouchingObject = true;
        InteractIcon.SetActive(true);
    }

    public void HideInteractIcon()
    {
        GameInfo.IsTouchingObject = false;
        InteractIcon.SetActive(false);
    }
    #endregion

    #region Private Functions
    private void UpdateKeyPressStatus()
    {
        GameInfo.IsPressingKey = input.RetrieveInteractInput(); 
    }

    private void UpdateInteractionStatus()
    {
        GameInfo.HasInteracted = GameInfo.IsTouchingObject && GameInfo.IsPressingKey;
    }

    // Check the collided game object, and interact it
    private void CheckInteract()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, colliderSize, 0, Vector2.zero);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();

                if (interactableObject == null)
                    continue;

                bool isInteracted = interactableObject.Interact();

                if (!isInteracted)
                    continue;

                ItemInteraction itemInteraction = hit.transform.GetComponent<ItemInteraction>();

                if (itemInteraction != null)
                {
                    int itemValue = itemInteraction.Value;
                    GameInfo.CurrentCredit += itemValue;
                }

                return;
            }
        }
    }

    // Set the player interact icon position
    private void UpdateIconPosition()
    {
        InteractIcon.transform.position = transform.position + new Vector3(iconOffsetX, iconOffsetY, -2);
        InteractIcon.transform.rotation = Quaternion.identity;

        bool isFlipped = transform.localScale.x < 0;
        Vector3 scale = new Vector3(iconScale.x, iconScale.y, iconScale.z);
        InteractIcon.transform.localScale = Vector3.Scale(scale, isFlipped ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1));
    }
    #endregion
}
