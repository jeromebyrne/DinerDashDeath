using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class PlayerAnimationController : MonoBehaviour {

    private CharacterMovement playerMovement = null;
    public SkeletonAnimation skeletonAnimation = null;
    private Vector3 startingScale = new Vector2(1.0f, 1.0f);
    public float aimDirection = 1.0f;

    // Use this for initialization
    void Start ()
    {
        startingScale = gameObject.transform.localScale;
        playerMovement = gameObject.GetComponent<CharacterMovement>();
        skeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        float animTimeScale = TimeManager.GetInstance().GetCurrentTimescale();

        gameObject.transform.localScale = new Vector3(startingScale.x * aimDirection,
                                                        startingScale.y,
                                                        startingScale.z);

        float percentSpeed = animTimeScale == 1.0f ? Mathf.Abs(playerMovement.GetCurrentVelocityX() / playerMovement.maxVelocity.x) : 1.0f;

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
            if (playerMovement.IsGrounded())
            {
                if (System.Math.Abs(playerMovement.GetCurrentVelocityX()) > 0.0f)
                {
                    if (trackEntry.Animation.Name != "run")
                    {
                        PlayAnimation("run", true);
                    }
                }
                else
                {
                    if (trackEntry.Animation.Name != "idle1")
                    {
                        PlayAnimation("idle1", true);
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
