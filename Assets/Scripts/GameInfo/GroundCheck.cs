using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private void Start()
    {
        if (GameInfo.CurrentSceneName != "Company")
        {
            GameInfo.IsOnGround = false;
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameInfo.IsOnGround = false;
        GameInfo.GroundFriction = 0;
    }

    private void EvaluateCollision(Collision2D collision)   // to check if player collides with (collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            GameInfo.IsOnGround |= normal.y >= 0.9f;
        }
    }

    private void RetrieveFriction(Collision2D collision)
    {
        PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;

        GameInfo.GroundFriction = 0;

        if (material != null)
        {
            GameInfo.GroundFriction = material.friction;
        }
        else { return; }
    }
}
