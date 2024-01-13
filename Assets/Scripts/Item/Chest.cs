using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Chest : InteractableObject
{
    public Sprite Open;
    public Sprite Closed;

    private SpriteRenderer sr;
    private bool isOpen;

    public override void Interact()
    {
        if (isOpen)
        {
            sr.sprite = Closed;
        }
        else
        {
            sr.sprite = Open;
        }

        isOpen = !isOpen;
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = Closed;
    }
}
