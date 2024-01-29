using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    private GameInfo gameInfo;
    private TextMeshProUGUI shopText;

    private void Start()
    {
        gameInfo = FindAnyObjectByType<GameScenesManager>().GameInfo;
        shopText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        shopText.text = $"REQUIRED CREDIT \n{gameInfo.ShopCost}";
    }
}
