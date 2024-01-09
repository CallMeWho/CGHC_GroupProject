using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverMovement : MonoBehaviour
{
    private DiverConditions m_diverConditions;

    private void Start()
    {
        m_diverConditions = GetComponent<DiverConditions>();
    }

    private void Update()
    {
        switch (m_diverConditions.currentState)
        {
            case DiverState.Walking:
                HandleWalking();
                break;

            case DiverState.Diving:
                HandleDiving();
                break;

            case DiverState.Dead:
                break;
        }
    }

    private void HandleWalking()
    {
        float horzInput = Input.GetAxis("Horizontal");
        float moveSpeed = m_diverConditions.CurrentMoveSpeed;

        Vector2 displacement = new Vector2(horzInput, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(displacement);
    }

    private void HandleDiving()
    {
        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        float moveSpeed = m_diverConditions.CurrentMoveSpeed;

        Vector2 displacement = new Vector2(horzInput, vertInput) * moveSpeed * Time.deltaTime;
        transform.Translate(displacement);
    }
}