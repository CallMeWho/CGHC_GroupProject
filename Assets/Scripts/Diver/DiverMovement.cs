using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;

public class DiverMovement : MonoBehaviour
{
    private DiverConditions m_diverConditions;
    private Rigidbody2D m_rb;
    private BoxCollider2D m_boxCollider;
    [SerializeField] public GameObject SpawnPoint;

    private List<Vector2> m_rayPointsList = new List<Vector2>();

    [SerializeField] public float RotationSpeed;

    private void Start()
    {
        m_diverConditions = GetComponent<DiverConditions>();
        m_rb = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_diverConditions.currentState = DiverState.Walking;

        m_rb.bodyType = RigidbodyType2D.Dynamic;
        //ShowRays();

        // Get the position of the player spawn point
        Vector2 spawnPoint = SpawnPoint.transform.position;

        // Set the initial position of the player
        transform.position = spawnPoint;
    }

    private void Update()
    {
        //ShowRays();

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
        if (m_diverConditions.CurrentMoveSpeed <= 0)
        {
            m_diverConditions.CurrentMoveSpeed = 10;
        }

        m_rb.mass = 1.0f;
        m_rb.freezeRotation = true;

        float horzInput = Input.GetAxis("Horizontal");
        float moveSpeed = m_diverConditions.CurrentMoveSpeed;

        Vector2 displacement = new Vector2(horzInput, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(displacement);
    }

    private void HandleDiving()
    {
        

        if (m_diverConditions.CurrentMoveSpeed <= 0)
        {
            m_diverConditions.CurrentMoveSpeed = 10;
        }

        m_rb.gravityScale = 0f;

        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        Vector2 moveDirection = new Vector2(horzInput, vertInput);
        float inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);
        moveDirection.Normalize();
        float moveSpeed = m_diverConditions.CurrentMoveSpeed;

        Vector2 displacement = moveDirection * moveSpeed * inputMagnitude * Time.deltaTime;
        //m_rb.MovePosition(transform.position + (Vector3)displacement);
        
        transform.Translate(displacement, Space.World);

        if (moveDirection != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);

            if (Mathf.Abs(horzInput) > 0)
            {
                // Flip the sprite horizontally
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Sign(horzInput) * Mathf.Abs(newScale.x);
                transform.localScale = newScale;
            }
        }

        

        //botleft
        //Vector2 raycastOrigin = transform.position + transform.TransformVector(m_boxCollider.offset) - 
        //   transform.TransformVector(m_boxCollider.size) * 0.5f;

        //topleft
        Vector2 raycastOriginTL = transform.position + transform.TransformVector(m_boxCollider.offset) + 
            transform.TransformVector(new Vector2(-m_boxCollider.size.x, m_boxCollider.size.y)) * 0.5f;

        //topright
        Vector2 raycastOriginTR = transform.position + transform.TransformVector(m_boxCollider.offset) + 
            transform.TransformVector(new Vector2(m_boxCollider.size.x, m_boxCollider.size.y)) * 0.5f;

        RaycastHit2D hit = Physics2D.Raycast(
            raycastOriginTL, transform.up, 10f, LayerMask.GetMask("Map"));
        RaycastHit2D hit2 = Physics2D.Raycast(
            raycastOriginTR, transform.up, 10f, LayerMask.GetMask("Map"));
        Debug.DrawRay(raycastOriginTL, transform.up, UnityEngine.Color.green);
        Debug.DrawRay(raycastOriginTR, transform.up, UnityEngine.Color.green);

        if (hit)
        {
            Debug.Log("hit wall");
            m_rb.velocity = Vector2.zero;
        }
    }

    private void ShowRays()
    {
        m_rayPointsList.Clear();

        Vector2 rayPoint_TopRight = new Vector2(m_boxCollider.bounds.max.x, m_boxCollider.bounds.max.y);
        Vector2 rayPoint_BotRight = new Vector2(m_boxCollider.bounds.max.x, m_boxCollider.bounds.min.y);

        m_rayPointsList.Add(rayPoint_TopRight);
        m_rayPointsList.Add(rayPoint_BotRight);

        foreach (Vector2 rayPoint in m_rayPointsList)
        {
            Vector2 rotatedDirection = transform.rotation * Vector2.right;

            Debug.DrawRay(rayPoint, rotatedDirection, UnityEngine.Color.green);
        }
    }

    private void Raycasting(Vector2 displacement, float inputMagnitude)
    {
        
    }
}