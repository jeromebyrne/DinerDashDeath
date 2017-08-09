using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public float startingHealth = 100.0f;
    private float currentHealth = 0.0f;

    public ParticleSystem bloodBurst = null;

    public bool IsDead()
    {
        return currentHealth <= 0.0f;
    }

	// Use this for initialization
	void Start ()
    {
        currentHealth = startingHealth;

        var bloodBurstObj = GameObject.Instantiate(Resources.Load("Prefabs/bloodBurst")) as GameObject;
        if (bloodBurstObj)
        {
            bloodBurst = bloodBurstObj.GetComponent<ParticleSystem>();
            //bloodBurst.transform.SetParent(gameObject.transform);
        }

        if (bloodBurst)
        {
            bloodBurst.Stop();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ChangeHealth(float delta)
    {
        currentHealth += delta;
    }
}
