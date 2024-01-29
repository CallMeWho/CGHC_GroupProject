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

    public abstract bool Interact();    // after player interacts, will do...

    private void OnTriggerEnter2D(Collider2D collision)  // when player touching object, will do...
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<InteractProcess>().ShowInteractIcon();   // show player interact icon
            
            if (this.GetComponent<ItemInteraction>())
            {
                this.GetComponent<ItemInteraction>().ShowBorder();  // show object border
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
