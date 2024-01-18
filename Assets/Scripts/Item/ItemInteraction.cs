using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemInteraction : InteractableObject
{
    [SerializeField] public Sprite Open;
    [SerializeField] public Sprite Closed;
    [SerializeField] private GameObject TouchBorder;
    [SerializeField] public int Value = 100;

    private SpriteRenderer sr;
    private bool isOpen;
    private bool isTouch;

    public override bool Interact()
    {
        if (isOpen)
        {
            sr.sprite = Closed;
            
        }
        else
        {
            sr.sprite = Open;
            Wait(0.1f);
            StartCoroutine(FadeSprite());
        }

        isOpen = !isOpen;   //vice versa
        return isOpen;
    }

    public override int GetValue()
    {
        return Value;
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

    private IEnumerator Wait(float second)
    {
        yield return new WaitForSeconds(second); // Wait for 1 second before starting the fading effect
    }

    private IEnumerator FadeSprite()
    {
        float duration = 1.0f; // Time it takes for the sprite to fully disappear
        float elapsedTime = 0.0f;
        Material material = sr.material;

        Color originalColor = material.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            material.color = Color.Lerp(originalColor, transparentColor, t);
            yield return null;
        }

        // Once the sprite has fully disappeared, you can perform other actions if needed
        // For example, disabling the object or removing it from the scene
        gameObject.SetActive(false);
    }
}
