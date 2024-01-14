using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buy : MonoBehaviour
{
    [SerializeField] public int Credit;
    [SerializeField] public int Quota;

    public void IncreaseCredit(int itemValue)
    {
        Credit += itemValue;
    }
}
