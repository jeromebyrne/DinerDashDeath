using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public Player player = null;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void LateUpdate()
    {
        if (player)
        {
            var animCtrl = player.GetComponent<PlayerAnimationController>();

            if (animCtrl != null)
            {
                var gunBone = animCtrl.skeletonAnimation.skeleton.FindBone("gun");

                if (gunBone != null)
                {
                    UpdateGunAiming(gunBone);
                }
            }
        }
    }

    void UpdateGunAiming(Spine.Bone gunBone)
    {
        // could be public
        const float LowerRotationBound = -60.0f;
        const float UpperRotationBound = 60.0f;

        // temp variables
        float tempRot;
        Vector3 tempVec;

        // gun bone rotation
        tempVec = Camera.main.WorldToScreenPoint(new Vector3(gunBone.WorldX + transform.position.x, gunBone.WorldY + transform.position.y, 0));
        tempVec = Input.mousePosition - tempVec;
        tempRot = Mathf.Atan2(tempVec.y, tempVec.x * transform.localScale.x) * Mathf.Rad2Deg;
        gunBone.Rotation = Mathf.Clamp(tempRot, LowerRotationBound, UpperRotationBound) - gunBone.parent.LocalToWorldRotation(gunBone.parent.rotation);
    }
}
