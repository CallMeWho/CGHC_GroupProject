using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Buy buyComponent = collision.GetComponent<Buy>();

            if (buyComponent != null)
            {
                int credit = buyComponent.Credit;
                int quota = buyComponent.Quota;

                if (credit >= quota)
                {
                    SceneManager.LoadScene("Company");
                }
            }
        }
    }
}
