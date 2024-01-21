using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemInteraction : InteractableObject
{
    public enum InteractType { Item, Shop }

    [SerializeField] private InteractType interactType;
    [SerializeField] public Sprite Open;
    [SerializeField] public Sprite Closed;
    [SerializeField] private GameObject TouchBorder;
    [SerializeField] public int Value;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private SpriteRenderer sr;
    private bool isOpen;
    private bool isTouch;
    private bool canInteract = true;

    public override bool Interact()
    {
        bool interactDone = false;

        if (canInteract && GameInfo.HasInteracted)
        {
            if (interactType.ToString() == "Item")
            {
                StartCoroutine(InteractCoroutine());    //internal cooldown, or if player keep pressing will get more money for only 1 item
                StartCoroutine(FadeSprite());
            }

            else if (interactType.ToString() == "Shop")
            {
                if (GameInfo.CanBuySkill && GameInfo.HasInteracted)
                {
                    GameScenesManager.GameScenesManagerInstance.LoadGameScene("Shop");
                }
            }

            interactDone = true;
        }

        return interactDone;
    }

    private bool temp()
    {
        if (interactType.ToString() == "Item")
        {
            if (isOpen)
            {
                sr.sprite = Closed;

            }
            else
            {
                sr.sprite = Open;
                GameInfo.CurrentCredit += Value;
                //Wait(0.1f);
                StartCoroutine(FadeSprite());
            }

            isOpen = !isOpen;   //vice versa
            return isOpen;
        }

        if (interactType.ToString() == "Shop")
        {
            if (isOpen)
            {
                sr.sprite = Closed;
            }
            else
            {
                if (GameInfo.CanBuySkill && GameInfo.HasInteracted)
                {
                    GameScenesManager.GameScenesManagerInstance.LoadGameScene("Shop");
                }
            }

            isOpen = !isOpen;   //vice versa
            return isOpen;
        }

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

    private IEnumerator InteractCoroutine()
    {
        canInteract = false;
        yield return new WaitForSeconds(1f);
        canInteract = true;
    }

    private IEnumerator FadeSprite()
    {
        sr.sprite = Open;

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
