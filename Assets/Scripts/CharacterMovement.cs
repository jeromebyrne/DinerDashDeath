using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class CharacterMovement : MonoBehaviour
{
    const float kJumpAmount = 1.0f;

    private Vector2 currentVelocity = new Vector2();
    private float intendedDirectionX = 1.0f;
    private bool isGrounded = false;
    public Vector2 maxVelocity = new Vector2(10.0f, 10.0f);
    public Vector2 resistance = new Vector2(1.0f, 1.0f);
    public bool acceleratingX = false;
    public AudioClip footstepClip = null;
    private AudioSource footstepSource = null;

    [SpineEvent]
    public string footstepEventName = "Footstep";

    float lastJumped = 0.0f;
    float timeGrounded = 0.0f;
    float lastGrounded = 0.0f;

    // TEMP vars
    public Text velXText = null;
    public Text velYText = null;

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public float GetTimeGrounded()
    {
        return timeGrounded;
    }

    public float GetLastTimeGrounded()
    {
        return lastGrounded;
    }

    public float GetIntendedDirectionX()
    {
        return intendedDirectionX;
    }

    public float GetCurrentVelocityX()
    {
        return currentVelocity.x;
    }

    // Use this for initialization
    void Start()
    {
        footstepSource = gameObject.AddComponent<AudioSource>();

        footstepSource.clip = footstepClip;

        SkeletonAnimation skelAnim = gameObject.GetComponent<SkeletonAnimation>();

        if (skelAnim != null)
        {
            skelAnim.AnimationState.Event += HandleEvent;
        }
    }

    public void IncrementVelocityX(float deltaX)
    {
        intendedDirectionX = deltaX >= 0.0f ? 1.0f : -1.0f;

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
    void FixedUpdate()
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

        if (!acceleratingX)
        {
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

            currentVelocity = new Vector2(currentVelocity.x, kJumpAmount * TimeManager.GetInstance().GetCurrentTimescale());
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

    void HandleEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == footstepEventName)
        {
            footstepSource.Stop();
            footstepSource.pitch = GetRandomPitch(0.2f);
            footstepSource.Play();
        }
    }

    static float GetRandomPitch(float maxOffset)
    {
        return 1f + Random.Range(-maxOffset, maxOffset);
    }
}
