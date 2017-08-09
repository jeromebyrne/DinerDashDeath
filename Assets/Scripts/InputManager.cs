using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    const string kHorizontalAxis = "Horizontal";
    const string kVerticalAxis = "Vertical";
    const float kPlayerMoveX = 2.0f;

    bool slomoOn = false;
    bool fastFloOn = false;

    public Image slomoImage = null;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.Jump();
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            slomoOn = !slomoOn;
            fastFloOn = false;
            
        }
        else if (Input.GetMouseButtonUp(2))
        {
            fastFloOn = !fastFloOn;
            slomoOn = false;
        }

        if (slomoOn)
        {
            TimeManager.GetInstance().SetCurrentTimescale(0.25f);
            slomoImage.enabled = true;
        }
        else if (fastFloOn)
        {
            TimeManager.GetInstance().SetCurrentTimescale(1.75f);
            slomoImage.enabled = false;
        }
        else
        {
            TimeManager.GetInstance().SetCurrentTimescale(1.0f);
            slomoImage.enabled = false;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
