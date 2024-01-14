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
            collision.GetComponent<Interact>().ShowInteractIcon();

            if (this.GetComponent<Chest>())
            {
                this.GetComponent<Chest>().ShowBorder();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Interact>().HideInteractIcon();

            if (this.GetComponent<Chest>())
            {
                this.GetComponent<Chest>().HideBorder();
            }
        }
    }
}
