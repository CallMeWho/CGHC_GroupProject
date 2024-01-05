using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Diving divingScript;

    private void Start()
    {
        divingScript = GetComponent<Diving>();
    }

    private void Update()
    {
        if (divingScript.IsDiving)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(horizontalInput, verticalInput) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
    }
}