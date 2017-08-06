using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class PlayerAnimationController : MonoBehaviour {

    public Player player = null;
    private CharacterMovement playerMovement = null;
    public SkeletonAnimation skeletonAnimation = null;

	// Use this for initialization
	void Start ()
    {
        playerMovement = player.gameObject.GetComponent<CharacterMovement>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        TrackEntry trackEntry = skeletonAnimation.AnimationState.GetCurrent(0);

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

    void PlayAnimation(string animationName, bool loop)
    {
        if (!skeletonAnimation)
        {
            return;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
    }
}
