using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    public string levelToSwitchTo;

    private GameManager gameManager = null;

	// Use this for initialization
	void Start () {
        GameObject managersObj = GameObject.Find("Managers");

        gameManager = managersObj.GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Switch level
            gameManager.SwitchLevel(levelToSwitchTo);
        }
    }
}
