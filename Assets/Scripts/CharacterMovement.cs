using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    public float moveForce = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        float h = Input.GetAxis("Horizontal") * moveForce;

        var rb = GetComponent<Rigidbody2D>();

        rb.AddForce(new Vector2(h, 0));
	}
}
