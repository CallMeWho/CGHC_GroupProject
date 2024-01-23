using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemInteraction : InteractableObject
{
    public enum InteractType { Item, Shop }

    [SerializeField] private InteractType interactType;
    [SerializeField] private Sprite NormalSprite;
    [SerializeField] private Sprite InteractedSprite;
    [SerializeField] private GameObject TouchBorder;
    [SerializeField] public int Value;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private SpriteRenderer sr;
    private bool canInteract = true;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // set item to before interact sprite
        sr.sprite = NormalSprite;

        // hide item touch border
        if (TouchBorder != null)
        {
            TouchBorder.SetActive(false);
        }
    }

    // after player interacts with item, it will do...
    public override bool Interact()
    {
        bool interactDone = false;

        if (canInteract && GameInfo.HasInteracted)
        {
            if (interactType.ToString() == "Item")
            {
                PlaySound();
                StartCoroutine(InteractCoroutine());    //internal cooldown, or if player keep pressing will get more money for only 1 item
                StartCoroutine(FadeSprite());
            }

            else if (interactType.ToString() == "Shop")
            {
                if (GameInfo.CanBuySkill && GameInfo.HasInteracted)
                {
                    PlaySound();
                    StartCoroutine(InteractCoroutine());
                    GameScenesManager.GameScenesManagerInstance.LoadGameScene("Shop");
                }
            }

            interactDone = true;
        }

        return interactDone;
    }

    public void ShowBorder()
    {
        TouchBorder.SetActive(true);
    }

    public void HideBorder()
    {
        TouchBorder.SetActive(false);
    }

    // set player interact key with a cooldown
    // (Why?) if player keep pressing key, credit will increase several times as long as item not disappear yet
    private IEnumerator InteractCoroutine()
    {
        canInteract = false;
        yield return new WaitForSeconds(1f);
        canInteract = true;
    }

    // item fades out after interact
    private IEnumerator FadeSprite()
    {
        sr.sprite = InteractedSprite;

        float duration = 1.0f; // time to fully disappear (editable)
        float elapsedTime = 0.0f;

        Color originalColor = sr.material.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            sr.material.color = Color.Lerp(originalColor, transparentColor, t);
            yield return null;
        }

        // after item disappeared, set it to no active
        gameObject.SetActive(false);
    }

    private void PlaySound()
    {
        // when interact with shop
        if (name == "ShopCollider")
        {
            AudioManager.instance.PlaySound("ShopBuySound", AudioManager.instance.sfxSounds, AudioManager.instance.sfxSource, true);
        }

        // when interact with item
        if (name == "Chest(Clone)" || name == "Crystal 1(Clone)" || name == "Crystal 2(Clone)" || name == "Crystal 3(Clone)" ||
            name == "Gold(Clone)" || name == "Iron(Clone)")
        {
            AudioManager.instance.PlaySound("MiningSound", AudioManager.instance.sfxSounds, AudioManager.instance.sfxSource, true);
        }
    }
}
