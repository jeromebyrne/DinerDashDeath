using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject player = null;
    public GameObject levelParent = null;
    public GameObject camera = null;
    public GameObject gunTarget = null;
    private PlayerAnimationController playerAnim = null;
    private CharacterMovement playerMovement = null;

	// Use this for initialization
	void Start ()
    {
        Cursor.visible = false;
        playerAnim = player.GetComponent<PlayerAnimationController>();
        playerMovement = player.GetComponent<CharacterMovement>();
        SwitchLevel("Prefabs/level1");
    }

    public void SwitchLevel(string levelName)
    {
        foreach (Transform child in levelParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject level = GameObject.Instantiate(Resources.Load(levelName)) as GameObject;

        if (level)
        {
            level.transform.SetParent(levelParent.transform);

            LevelInfo levelInfo = level.GetComponent<LevelInfo>();
            if (levelInfo)
            {
                player.transform.position = new Vector3(levelInfo.playerSpawn.transform.position.x,
                                                        levelInfo.playerSpawn.transform.position.y,
                                                        player.transform.position.z);

                camera.transform.position = new Vector3(player.transform.position.x,
                                                        camera.transform.position.y,
                                                        camera.transform.position.z);
            }
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void Update()
    {
        if (player)
        {
            var animCtrl = player.GetComponent<PlayerAnimationController>();

            if (animCtrl != null)
            {
                var gunBone = animCtrl.skeletonAnimation.skeleton.FindBone("gun_control");

                if (gunBone != null)
                {
                    UpdateGunAimingAndShooting(gunBone);
                }
            }
        }
    }

    void UpdateGunAimingAndShooting(Spine.Bone gunBone)
    {
        if (Input.GetMouseButton(0))
        {
            // could be public
            const float LowerRotationBound = -90.0f;
            const float UpperRotationBound = 90.0f;

            // temp variables
            float tempRot;
            Vector3 tempVec;

            // gun bone rotation
            tempVec = Camera.main.WorldToScreenPoint(new Vector3(gunBone.WorldX + player.transform.position.x, gunBone.WorldY + (player.transform.position.y), 0));
            tempVec = Input.mousePosition - tempVec;
            tempRot = Mathf.Atan2(tempVec.y, tempVec.x * player.transform.localScale.x) * Mathf.Rad2Deg;
            gunBone.Rotation = Mathf.Clamp(tempRot, LowerRotationBound, UpperRotationBound) - gunBone.parent.LocalToWorldRotation(gunBone.parent.rotation);

 
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = gunTarget.transform.position.z;
            gunTarget.transform.position = mousePos;
            gunTarget.SetActive(true);

            if (mousePos.x >= player.transform.position.x)
            {
                playerAnim.aimDirection = 1.0f;
            }
            else
            {
                playerAnim.aimDirection = -1.0f;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            
            // Fire weapon
        }
        else
        {
            gunTarget.SetActive(false);
            playerAnim.aimDirection = playerMovement.GetIntendedDirectionX();
        }
        
    }
}
