using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Chest : InteractableObject
{
    [SerializeField] public Sprite Open;
    [SerializeField] public Sprite Closed;
    [SerializeField] private GameObject TouchBorder;

    private SpriteRenderer sr;
    private bool isOpen;
    private bool isTouch;

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
        TouchBorder.SetActive(false );
    }

    public void ShowBorder()
    {
        TouchBorder.SetActive(true);
    }

    public void HideBorder()
    {
        TouchBorder.SetActive(false);
    }
}
