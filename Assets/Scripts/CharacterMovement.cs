﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    const float kJumpDelta = 15.0f;

    private Vector2 currentVelocity = new Vector2();
    private float intendedDirectionX = 1.0f;
    private bool isGrounded = false;
    public Vector2 maxVelocity = new Vector2(10.0f, 10.0f);
    public Vector2 resistance = new Vector2(1.0f, 1.0f);
    private Vector3 startingScale = new Vector2(1.0f, 1.0f);

    float lastJumped = 0.0f;
    float timeGrounded = 0.0f;
    float lastGrounded = 0.0f;

    // TEMP vars
    public Text velXText = null;
    public Text velYText = null;

    public float GetCurrentVelocityX()
    {
        return currentVelocity.x;
    }

    // Use this for initialization
    void Start()
    {
        startingScale = gameObject.transform.localScale;
    }

    public void IncrementVelocityX(float deltaX)
    {
        intendedDirectionX = deltaX >= 0.0f ? 1.0f : -1.0f;

        gameObject.transform.localScale = new Vector3(startingScale.x * intendedDirectionX,
                                                        startingScale.y,
                                                        startingScale.z);

        float newVelX = currentVelocity.x + (deltaX * TimeManager.GetInstance().GetTimeDelta());

        float currentTimeScale = TimeManager.GetInstance().GetCurrentTimescale();

        if (newVelX > (maxVelocity.x * currentTimeScale))
        {
            newVelX = maxVelocity.x * currentTimeScale;
        }
        else if (newVelX < (-maxVelocity.x * currentTimeScale))
        {
            newVelX = -maxVelocity.x * currentTimeScale;
        }

        currentVelocity = new Vector2(newVelX, currentVelocity.y);
    }

    public void IncrementVelocityY(float deltaY)
    {
        float newVelY = currentVelocity.y + (deltaY * TimeManager.GetInstance().GetTimeDelta());

        float currentTimeScale = TimeManager.GetInstance().GetCurrentTimescale();

        if (newVelY > (maxVelocity.y * currentTimeScale))
        {
            newVelY = maxVelocity.y * currentTimeScale;
        }
        else if (newVelY < (-maxVelocity.y * currentTimeScale))
        {
            newVelY = -maxVelocity.y * currentTimeScale;
        }

        currentVelocity = new Vector2(currentVelocity.x, newVelY);
    }

    // Update is called once per frame
    void Update()
    {
        float timeDelta = TimeManager.GetInstance().GetTimeDelta();

        lastJumped += timeDelta;

        if (isGrounded)
        {
            timeGrounded += timeDelta;
            lastGrounded = 0.0f;
        }
        else
        {
            timeGrounded = 0.0f;
            lastGrounded += timeDelta;
        }

        gameObject.transform.position = new Vector3(gameObject.transform.position.x + currentVelocity.x, 
                                                    gameObject.transform.position.y + currentVelocity.y, 
                                                    gameObject.transform.position.z);

        ApplyResistance();

        velXText.text = currentVelocity.x.ToString();
        velYText.text = currentVelocity.y.ToString();
    }

    void ApplyResistance()
    {
        Vector2 newVelocity = currentVelocity;

        float timeDelta = TimeManager.GetInstance().GetTimeDelta();

        if (currentVelocity.x > 0)
        {
            newVelocity.x = currentVelocity.x - (resistance.x * timeDelta);

            if (newVelocity.x < 0.0f)
            {
                newVelocity.x = 0.0f;
            }
        }
        else
        {
            newVelocity.x = currentVelocity.x + (resistance.x * timeDelta);

            if (newVelocity.x > 0.0f)
            {
                newVelocity.x = 0.0f;
            }
        }

        if (isGrounded)
        {
            if (lastJumped > 0.1f)
            {
                newVelocity.y = 0.0f;
            }
        }
        else
        {
            float currentTimeScale = TimeManager.GetInstance().GetCurrentTimescale();

            newVelocity.y = currentVelocity.y - (resistance.y * timeDelta * currentTimeScale);
        }

        currentVelocity = newVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        checkCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        checkCollision(collision);
    }

    public void Jump()
    {
        if (isGrounded || (lastGrounded < 0.1f && currentVelocity.y <= 0.0f))
        {
            lastJumped = 0.0f;

            IncrementVelocityY(kJumpDelta);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject != gameObject &&
            collision.otherCollider.gameObject != gameObject)
        {
            return;
        }

        if (collision.otherCollider.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

    void checkCollision(Collision2D collision)
    {
        if (collision.collider.gameObject != gameObject &&
            collision.otherCollider.gameObject != gameObject)
        {
            return;
        }

        if (collision.otherCollider.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

}
