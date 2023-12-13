using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Editor
    [Header("Settings")]
    [SerializeField] private float InitialGravity = -20f;

    [Header("Collision")]
    [SerializeField] private LayerMask m_CollideWithLayer;
    [SerializeField] private int m_VerticalRayAmount = 4;
    #endregion

    #region Internal
    //player collider
    private BoxCollider2D m_BoxCollider2D;  

    // raycast
    private Vector2 m_BoundTopLeft;
    private Vector2 m_BoundTopRight;
    private Vector2 m_BoundBottomLeft;
    private Vector2 m_BoundBottomRight;
    private float m_BoundWidth;
    private float m_BoundHeight;

    // gravity
    private float m_CurrentGravity;
    private Vector2 m_GravityForce;
    private Vector2 m_PlayerCurrentPosition;

    // collision
    private float m_Skin = 0.05f;
    #endregion

    private void Start()
    {
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        ApplyGravity();
        StartMovement();

        SetRayOrigins();
        GroundCollision();

        SetRayOrigins();
        CalculateMovement();

        //raycast
        Debug.DrawRay(m_BoundBottomLeft, Vector2.left, Color.green);
        Debug.DrawRay(m_BoundBottomRight, Vector2.right, Color.green);
        Debug.DrawRay(m_BoundTopLeft, Vector2.left, Color.green);
        Debug.DrawRay(m_BoundTopRight, Vector2.right, Color.green);

        //downfall 
        transform.Translate(m_PlayerCurrentPosition, Space.Self);    //space.self is what??


    }

    #region RayCast
    private void SetRayOrigins()
    {
        Bounds playerBounds = m_BoxCollider2D.bounds;

        m_BoundBottomLeft = new Vector2(playerBounds.min.x, playerBounds.min.y);
        m_BoundBottomRight = new Vector2(playerBounds.max.x, playerBounds.min.y);
        m_BoundTopLeft = new Vector2(playerBounds.min.x, playerBounds.max.y);
        m_BoundTopRight = new Vector2(playerBounds.max.x, playerBounds.max.y);

        m_BoundHeight = Vector2.Distance(m_BoundBottomLeft, m_BoundTopLeft);
        m_BoundWidth = Vector2.Distance(m_BoundBottomLeft, m_BoundBottomRight);
    }
    #endregion

    #region DownfallMovement
    private void ApplyGravity()
    {
        m_CurrentGravity = InitialGravity;
        m_GravityForce.y += m_CurrentGravity * Time.deltaTime;
    }

    private void StartMovement()
    {
        m_PlayerCurrentPosition = m_GravityForce * Time.deltaTime;
    }

    private void CalculateMovement()    //not understand this function
    {
        if (Time.deltaTime > 0)
        {
            m_GravityForce = m_PlayerCurrentPosition / Time.deltaTime;
        }
    }
    #endregion

    #region Collision
    private void GroundCollision()
    {
        float rayLength = m_BoundHeight / 2f + m_Skin;

        if (m_PlayerCurrentPosition.y <0)   //player downfall
        {
            rayLength += Mathf.Abs(m_PlayerCurrentPosition.y);  //abs is what??
        }

        Vector2 leftOrigin = (m_BoundBottomLeft + m_BoundTopLeft) / 2f;
        Vector2 rightOrigin = (m_BoundBottomRight + m_BoundTopRight) / 2f;
        leftOrigin += (Vector2)(transform.up * m_Skin) + (Vector2)(transform.right * m_PlayerCurrentPosition.x);
        rightOrigin += (Vector2)(transform.up * m_Skin) + (Vector2)(transform.right * m_PlayerCurrentPosition.x);

        //raycast
        for (int i = 0; i < m_VerticalRayAmount; i++)
        {
            Vector2 rayOrigin = Vector2.Lerp(leftOrigin, rightOrigin, (float) i / (float) m_VerticalRayAmount - 1); //lerp is what??
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -transform.up, rayLength, m_CollideWithLayer);
            Debug.DrawRay(rayOrigin, -transform.up * rayLength, Color.green);

            if (hit)
            {
                m_PlayerCurrentPosition.y = -hit.distance + m_BoundHeight / 2f + m_Skin;

                if (Mathf.Abs(m_PlayerCurrentPosition.y) < 0.00001f)
                {
                    m_PlayerCurrentPosition.y = 0f;
                }
            }
        }
    }
    #endregion
}

