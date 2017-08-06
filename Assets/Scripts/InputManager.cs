using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    const string kHorizontalAxis = "Horizontal";
    const string kVerticalAxis = "Vertical";
    const float kPlayerMoveX = 1.0f;
    private bool pressingJump = false;

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

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerMovement.Jump();
            /*
            if (!pressingJump)
            {
                playerMovement.Jump();
            }

            pressingJump = true;
            */
        }
        else
        {
            //pressingJump = false;
        }
    }
}
