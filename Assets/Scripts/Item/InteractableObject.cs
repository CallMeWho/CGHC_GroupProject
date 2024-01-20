using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class InteractableObject : MonoBehaviour
{
    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public abstract bool Interact();    // if player interacts
    public abstract int GetValue();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<InteractProcess>().ShowInteractIcon();
            
            if (this.GetComponent<ItemInteraction>())
            {
                this.GetComponent<ItemInteraction>().ShowBorder();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<InteractProcess>().HideInteractIcon();

            if (this.GetComponent<ItemInteraction>())
            {
                this.GetComponent<ItemInteraction>().HideBorder();
            }
        }
    }
}
