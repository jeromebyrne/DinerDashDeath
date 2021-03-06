﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {

    // public 
    public int m_PointsAwarded;
    public AudioClip m_CollectAudio;
    private AudioSource collectSource = null;

    // private
    bool m_Collected = false;

	// Use this for initialization
	void Start () {
        collectSource = gameObject.AddComponent<AudioSource>();

        collectSource.clip = m_CollectAudio;
    }
	
	// Update is called once per frame
	void Update () {

        if (m_Collected)
        {
            Vector2 nextPosition = transform.position;
            nextPosition.y += 50.0f * Time.deltaTime;
            transform.position = nextPosition;

            float nextScale = transform.localScale.x;
            nextScale -= 1.0f * Time.deltaTime;
            if (nextScale < 0.0f)
            {
                nextScale = 0.0f;
            }
            transform.localScale = new Vector3(nextScale, nextScale, nextScale);
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_Collected)
        {
            return;
        }

        if (collision.tag != "Player")
        {
            return;
        }


        m_Collected = true;

        collectSource.Play();

        Destroy(gameObject, 0.75f);
    }
}
