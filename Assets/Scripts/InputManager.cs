using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    const string kHorizontalAxis = "Horizontal";
    const string kVerticalAxis = "Vertical";
    const float kPlayerMoveX = 2.0f;

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
            playerMovement.acceleratingX = true;
        }
        else
        {
            playerMovement.acceleratingX = false;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W))
        {
            playerMovement.Jump();
        }
        
        if (Input.GetKey(KeyCode.Q))
        {
            TimeManager.GetInstance().SetCurrentTimescale(0.1f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            TimeManager.GetInstance().SetCurrentTimescale(2.0f);
        }
        else
        {
            TimeManager.GetInstance().SetCurrentTimescale(1.0f);
        }
    }
}
