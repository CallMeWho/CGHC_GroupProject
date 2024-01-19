using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuotaProcess : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private void Update()
    {
        if (GameInfo.CurrentCredit >= GameInfo.Quota)
        {
            GameInfo.HasMetQuota = true;
        }
        else
        {
            GameInfo.HasMetQuota = false;
        }
    }
}
