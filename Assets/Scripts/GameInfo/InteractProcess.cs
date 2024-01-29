using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player interacts
public class InteractProcess : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField] public GameObject InteractIcon;
    [SerializeField, Range(-2, 2)] private float iconOffsetX = 1.2f;
    [SerializeField, Range(-2, 2)] private float iconOffsetY = 1f;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private BoxCollider2D playerCol;    // player collider
    private Vector2 boxSize;    // collider size
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
        CheckIfPressingKey();
        CheckIfHasInteracted();

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
    private void CheckIfPressingKey()
    {
        if (Input.GetKey(KeyCode.J))
        {
            GameInfo.IsPressingKey = true;
        }
        else
        {
            GameInfo.IsPressingKey = false;
        }
    }

    private void CheckIfHasInteracted()
    {
        if (GameInfo.IsTouchingObject && GameInfo.IsPressingKey)
        {
            GameInfo.HasInteracted = true;
        }
        else
        {
            GameInfo.HasInteracted = false;
        }
    }

    // check collided game object, and interact it
    private void CheckInteract()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxSize, 0, Vector2.zero);   // check player collider collision

        if (hits.Length > 0)    // got collided game object (or collider, not sure)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.GetComponent<InteractableObject>())
                {
                    bool isInteracted = hit.transform.GetComponent<InteractableObject>().Interact();

                    if (isInteracted && hit.transform.GetComponent<ItemInteraction>())
                    {
                        int itemValue = hit.transform.GetComponent<ItemInteraction>().Value;
                        GameInfo.CurrentCredit += itemValue;
                    }

                    return; // will choose the nearest one only, if dont want then remove return
                }
            }
        }
    }

    // set player interact icon position
    private void UpdateIconPosition()
    {
        InteractIcon.transform.position = transform.position + new Vector3(iconOffsetX, iconOffsetY, -2);
        InteractIcon.transform.rotation = Quaternion.identity;

        bool isFlipped = transform.localScale.x < 0;
        float x = iconScale.x;
        float y = iconScale.y;
        float z = iconScale.z;
        InteractIcon.transform.localScale = isFlipped ? new Vector3(x, y, z) : new Vector3(-x, y, z);
    }
    #endregion
}
