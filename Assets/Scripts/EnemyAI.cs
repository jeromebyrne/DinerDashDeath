using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    private GameObject playerTarget = null;
    private CharacterMovement characterMovement = null;
    private CharacterHealth characterHealth = null;
    private const float kMoveSpeed = 1.5f;
    private const float kDistanceToStrikeSquared = 3.0f * 3.0f;
    private const float kDistanceToStopPursuingSquared = 100.0f * 100.0f;
    private bool isAttacking = false;

    public bool IsAttacking()
    {
        return isAttacking;
    }

    // Use this for initialization
    void Start ()
    {
        playerTarget = GameObject.Find("player");
        characterMovement = gameObject.GetComponent<CharacterMovement>();
        characterHealth = gameObject.GetComponent<CharacterHealth>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (playerTarget && !characterHealth.IsDead() && characterMovement.IsGrounded())
        {
            Vector3 offset = gameObject.transform.position - playerTarget.transform.position;
            float distanceFromPlayerSquared = offset.sqrMagnitude;

            if (distanceFromPlayerSquared > kDistanceToStrikeSquared  && distanceFromPlayerSquared < kDistanceToStopPursuingSquared)
            {
                if (playerTarget.transform.position.x > gameObject.transform.position.x)
                {
                    characterMovement.IncrementVelocityX(kMoveSpeed);
                }
                else
                {
                    characterMovement.IncrementVelocityX(-kMoveSpeed);
                }

                isAttacking = false;
            }
            else if (distanceFromPlayerSquared <= kDistanceToStrikeSquared)
            {
                isAttacking = true;
            }
        }
	}
}
