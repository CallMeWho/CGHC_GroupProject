using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour // only at company
{
    // player
    private bool isOnGround;

    // ground
    private float groundFriction;

    // scene
    private Spawn spawn;
    private string sceneName;

    private void Awake()
    {
        spawn = GetComponent<Spawn>();
    }

    private void Update()
    {
        sceneName = spawn.GetSceneName();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (sceneName != "Company")
        {
            return;
        }

        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (sceneName != "Company")
        {
            return;
        }

        isOnGround = false;
        groundFriction = 0;
    }

    private void EvaluateCollision(Collision2D collision)   // to check if player collides with (collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            isOnGround |= normal.y >= 0.9f;
        }
    }

    private void RetrieveFriction(Collision2D collision)
    {
        PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;

        groundFriction = 0;

        if (material != null)
        {
            groundFriction = material.friction;
        }
        else { return; }
    }

    public bool GetOnGround()
    {
        return isOnGround;
    }

    public float GetFriction()
    {
        return groundFriction;
    }
}
