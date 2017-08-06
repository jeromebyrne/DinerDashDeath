using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    const string kHorizontalAxis = "Horizontal";
    const string kVerticalAxis = "Vertical";

    const float kPlayerMoveX = 1.0f;

    public CharacterMovement playerMovement = null;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {

        float deltaMove = Input.GetAxis(kHorizontalAxis) * kPlayerMoveX;

        if (deltaMove != 0.0f)
        {
            playerMovement.IncrementVelocityX(deltaMove);
        }

        if (Input.GetAxis(kVerticalAxis) > 0)
        {
            playerMovement.Jump();
        }
    }
}
