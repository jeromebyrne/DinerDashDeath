using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject player = null;
    public GameObject levelParent = null;
    public GameObject camera = null;
    public GameObject gunTarget = null;
    public SpriteRenderer gunTargetSprite = null;
    private PlayerAnimationController playerAnim = null;
    private CharacterMovement playerMovement = null;

    private Vector3 gunTargetRegularScale = new Vector3(0.5f, 0.5f, 1.0f);
    private Vector3 gunTargetDoubleScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Color gunNotFocusedColor = new Color(1.0f, 0.7f, 0.0f);
    private Color gunFocusedColor = new Color(1.0f, 0.0f, 0.0f);

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
        const float LowerRotationBound = -90.0f;
        const float UpperRotationBound = 90.0f;

        // temp variables
        float tempRot;
        Vector3 tempVec;

        // gun bone rotation
        tempVec = Camera.main.WorldToScreenPoint(new Vector3(gunBone.WorldX + player.transform.position.x, gunBone.WorldY + (player.transform.position.y), 0));
        tempVec = Input.mousePosition - tempVec;
        Vector3 direction = tempVec;
        direction.Normalize();
        tempRot = Mathf.Atan2(tempVec.y, tempVec.x * player.transform.localScale.x) * Mathf.Rad2Deg;
        gunBone.Rotation = Mathf.Clamp(tempRot, LowerRotationBound, UpperRotationBound) - gunBone.parent.LocalToWorldRotation(gunBone.parent.rotation);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = gunTarget.transform.position.z;
        gunTarget.transform.position = mousePos;

        if (mousePos.x >= player.transform.position.x)
        {
            playerAnim.aimDirection = 1.0f;
        }
        else
        {
            playerAnim.aimDirection = -1.0f;
        }

        if (Input.GetMouseButton(0))
        {
            gunTarget.transform.localScale = gunTargetRegularScale;
            gunTargetSprite.color = gunFocusedColor;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var playerCollider = player.GetComponent<CapsuleCollider2D>();
            if (playerCollider)
            {
                playerCollider.enabled = false;
            }
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(gunBone.WorldX + player.transform.position.x,
                                                            gunBone.WorldY + (player.transform.position.y)),
                                                            direction);

            if (hit.collider != null)
            {
                CharacterHealth characterHealth = hit.collider.gameObject.GetComponent<CharacterHealth>();

                if (characterHealth)
                {
                    characterHealth.ChangeHealth(-100.0f);

                    var bloodBurst = characterHealth.bloodBurst;

                    if (bloodBurst)
                    {
                        bloodBurst.Stop();
                        bloodBurst.transform.position = new Vector3(hit.point.x, hit.point.y, bloodBurst.transform.position.z);
                        /*bloodBurst.transform.position = new Vector3(hit.collider.gameObject.transform.position.x, 
                                                                    hit.collider.gameObject.transform.position.y, 
                                                                    bloodBurst.transform.position.z);*/
                        bloodBurst.Play();
                    }
                }
            }
            if (playerCollider)
            {
                playerCollider.enabled = true;
            }
        }
        else
        {
            gunTarget.transform.localScale = gunTargetDoubleScale;
            gunTargetSprite.color = gunNotFocusedColor;
        }
        
    }
}
