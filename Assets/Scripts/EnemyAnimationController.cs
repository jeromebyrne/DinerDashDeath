using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules;

public class EnemyAnimationController : MonoBehaviour {

    CharacterMovement enemyMovement = null;
    private SkeletonAnimation skeletonAnimation = null;
    private Vector3 startingScale = new Vector2(1.0f, 1.0f);
    private EnemyAI enemyAI = null;
    private CharacterHealth characterHealth = null;
    public bool hasPlayedDeathAnim = false;
    string deathAnimPlayed;
    public bool isInRagdollMode = false;
    private float timeSinceDeath = 0.0f;

    // Use this for initialization
    void Start () {
        startingScale = gameObject.transform.localScale;
        enemyMovement = gameObject.GetComponent<CharacterMovement>();
        skeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
        characterHealth = gameObject.GetComponent<CharacterHealth>();
    }
	
	// Update is called once per frame
	void Update () {

        float animTimeScale = TimeManager.GetInstance().GetCurrentTimescale();

        if (!hasPlayedDeathAnim)
        {
            gameObject.transform.localScale = new Vector3(startingScale.x * enemyMovement.GetIntendedDirectionX(),
                                                        startingScale.y,
                                                        startingScale.z);
        }

        float percentSpeed = animTimeScale == 1.0f ? Mathf.Abs(enemyMovement.GetCurrentVelocityX() / enemyMovement.maxVelocity.x) : 1.0f;

        if (percentSpeed == 0.0f)
        {
            percentSpeed = 1.0f;
        }
        else if (percentSpeed < 0.40f)
        {
            percentSpeed = 0.4f;
        }

        skeletonAnimation.timeScale = animTimeScale * percentSpeed;

        TrackEntry trackEntry = skeletonAnimation.AnimationState.GetCurrent(0);

        if (trackEntry != null)
        {
            if (!characterHealth.IsDead())
            {
                if (enemyMovement.IsGrounded())
                {
                    if (enemyAI.IsAttacking())
                    {
                        if (trackEntry.Animation.Name != "attack" ||
                            trackEntry.IsComplete)
                        {
                            PlayAnimation("attack", false);

                            CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();

                            if (cameraShake)
                            {
                                /*
                                cameraShake.originalPos = Camera.main.transform.localPosition;
                                cameraShake.shakeAmount = 0.9f;
                                cameraShake.shakeDuration = 0.1f;
                                */
                            }
                        }
                    }
                    else
                    {
                        if (System.Math.Abs(enemyMovement.GetCurrentVelocityX()) > 0.0f)
                        {
                            if (trackEntry.Animation.Name != "run")
                            {
                                PlayAnimation("run", true);
                            }
                        }
                        else
                        {
                            if (trackEntry.Animation.Name != "idle")
                            {
                                PlayAnimation("idle", true);
                            }
                        }
                    }
                }
                else if (enemyMovement.GetLastTimeGrounded() > 0.4f)
                {
                    if (trackEntry.Animation.Name != "fall")
                    {
                        PlayAnimation("fall", true);
                    }
                }
            }
            else
            {
                if (hasPlayedDeathAnim == false)
                {
                    if (enemyMovement.IsGrounded())
                    {
                        if (trackEntry.Animation.Name != "death")
                        {
                            PlayAnimation("death", false);
                            deathAnimPlayed = "death";
                            hasPlayedDeathAnim = true;
                        }
                    }
                    else
                    {
                        if (trackEntry.Animation.Name != "death2")
                        {
                            PlayAnimation("death2", false);
                            deathAnimPlayed = "death2";
                            hasPlayedDeathAnim = true;
                        }
                    }
                }
            }
        }

        if (hasPlayedDeathAnim)
        {
            timeSinceDeath += TimeManager.GetInstance().GetTimeDelta();

            float timeUntilScale = 0.8f;

            if (deathAnimPlayed != "death2")
            {
                if (timeSinceDeath > timeUntilScale)
                {
                    float timeDelta = TimeManager.GetInstance().GetTimeDelta();
                    float dir = gameObject.transform.localScale.x >= 0.0f ? 1.0f : -1.0f;
                    float scale = Mathf.Abs(gameObject.transform.localScale.y) - timeDelta * 3.0f;

                    if (scale <= 0.0f)
                    {
                        Destroy(gameObject);

                        GameManager.NumEnemiesAlive -= 1;
                    }
                    else
                    {
                        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x,
                                                                    -scale,
                                                                    gameObject.transform.localScale.z);
                    }
                }
            }
            else if (timeSinceDeath > 0.3f)
            {
                Destroy(gameObject);

                GameManager.NumEnemiesAlive -= 1;
            }
        }
    }

    void PlayAnimation(string animationName, bool loop)
    {
        if (!skeletonAnimation)
        {
            return;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
    }
}

