using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class EnemyAnimationController : MonoBehaviour {

    CharacterMovement enemyMovement = null;
    public SkeletonAnimation skeletonAnimation = null;
    private Vector3 startingScale = new Vector2(1.0f, 1.0f);
    private EnemyAI enemyAI = null;

    // Use this for initialization
    void Start () {
        startingScale = gameObject.transform.localScale;
        enemyMovement = gameObject.GetComponent<CharacterMovement>();
        skeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
        enemyAI = gameObject.GetComponent<EnemyAI>();
    }
	
	// Update is called once per frame
	void Update () {

        float animTimeScale = TimeManager.GetInstance().GetCurrentTimescale();

        gameObject.transform.localScale = new Vector3(startingScale.x * enemyMovement.GetIntendedDirectionX(),
                                                        startingScale.y,
                                                        startingScale.z);

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
            if (enemyMovement.IsGrounded())
            {
                if (enemyAI.IsAttacking())
                {
                    if (trackEntry.Animation.Name != "attack")
                    {
                        PlayAnimation("attack", true);
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

