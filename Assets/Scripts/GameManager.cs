using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject player = null;
    public GameObject levelParent = null;
    public GameObject camera = null;
    public GameObject gunTarget = null;
    public SpriteRenderer gunTargetSprite = null;
    private PlayerAnimationController playerAnim = null;
    private CharacterMovement playerMovement = null;

    private float timeUntilFirstSpawn = 15.0f;

    public AudioClip shotgunClip = null;
    private AudioSource shotgunSource = null;
    public AudioClip shotgunCockClip = null;
    private AudioSource shotgunCockSource = null;
    public AudioClip shotgunReloadClip = null;
    private AudioSource shotgunReloadSource = null;

    public AudioClip bloodSplashClip = null;
    private AudioSource bloodSplashSource = null;

    private Vector3 gunTargetRegularScale = new Vector3(0.75f, 0.75f, 1.0f);
    private Vector3 gunTargetDoubleScale = new Vector3(1.25f, 1.25f, 1.0f);
    private Color gunNotFocusedColor = new Color(1.0f, 0.0f, 0.0f);
    private Color gunFocusedColor = new Color(0.0f, 1.0f, 0.0f);
    CameraShake cameraShake = null;

    float lastTimeFired = 9999.0f;

    const float kTimeBeforeShots = 1.0f;
    const float kTimeUntilReloadSFX = 0.4f;
    bool shotReady = true;
    bool playedCockedSfx = false;
    bool playedReload = false;

    float lastTimeSpawned = 0;

    public static int NumEnemiesAlive = 0;

    // Use this for initialization
    void Start ()
    {
        Cursor.visible = false;
        playerAnim = player.GetComponent<PlayerAnimationController>();
        playerMovement = player.GetComponent<CharacterMovement>();

        shotgunSource = gameObject.AddComponent<AudioSource>();
        shotgunSource.clip = shotgunClip;
        shotgunCockSource = gameObject.AddComponent<AudioSource>();
        shotgunCockSource.clip = shotgunCockClip;
        shotgunReloadSource = gameObject.AddComponent<AudioSource>();
        shotgunReloadSource.clip = shotgunReloadClip;

        bloodSplashSource = gameObject.AddComponent<AudioSource>();
        bloodSplashSource.clip = bloodSplashClip;

        cameraShake = Camera.main.GetComponent<CameraShake>();

        Random.InitState(987657894);

        SwitchLevel("Prefabs/level1");
    }

    public void SwitchLevel(string levelName)
    {
        NumEnemiesAlive = 0;

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
        if (timeUntilFirstSpawn > 0.0f)
        {
            timeUntilFirstSpawn -= TimeManager.GetInstance().GetTimeDelta();
        }

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

        if (!shotReady)
        {
            if (!playedReload && lastTimeFired >= kTimeUntilReloadSFX)
            {
                shotgunReloadSource.Stop();
                shotgunReloadSource.pitch = CharacterMovement.GetRandomPitch(0.2f) * TimeManager.GetInstance().GetCurrentTimescale();
                shotgunReloadSource.Play();
                playedReload = true;
            }

            if (lastTimeFired >= kTimeBeforeShots)
            {
                shotReady = true;
            }

            gunTargetSprite.color = gunNotFocusedColor;
            gunTarget.transform.localScale = gunTargetDoubleScale;
        }
        else
        {
            gunTargetSprite.color = gunFocusedColor;

            if (Input.GetMouseButton(0))
            {
                gunTarget.transform.localScale = gunTargetRegularScale;
            }
            else
            {
                gunTarget.transform.localScale = gunTargetDoubleScale;
            }
        }

        lastTimeFired += TimeManager.GetInstance().GetTimeDelta();

        UpdateSpawning();
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

        // if (shotReady)
        //{
            direction.Normalize();
            tempRot = Mathf.Atan2(tempVec.y, tempVec.x * player.transform.localScale.x) * Mathf.Rad2Deg;
            gunBone.Rotation = Mathf.Clamp(tempRot, LowerRotationBound, UpperRotationBound) - gunBone.parent.LocalToWorldRotation(gunBone.parent.rotation);
        //}
 
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

        if (shotReady && !playedCockedSfx && Input.GetMouseButton(0))
        {
            playedCockedSfx = true;
            shotgunCockSource.Stop();
            shotgunCockSource.pitch = CharacterMovement.GetRandomPitch(0.2f) * TimeManager.GetInstance().GetCurrentTimescale();
            shotgunCockSource.Play();
        }

        if (Input.GetMouseButtonUp(0) && shotReady)
        {
            lastTimeFired = 0.0f;
            shotReady = false;
            playedCockedSfx = false;
            playedReload = false;

            // shotgun sfx 
            {
                shotgunSource.Stop();
                shotgunSource.pitch = CharacterMovement.GetRandomPitch(0.2f) * TimeManager.GetInstance().GetCurrentTimescale();
                shotgunSource.Play();
            }

            var playerCollider = player.GetComponent<CapsuleCollider2D>();
            if (playerCollider)
            {
                playerCollider.enabled = false;
            }
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(gunBone.WorldX + player.transform.position.x,
                                                            gunBone.WorldY + (player.transform.position.y)),
                                                            direction);

            bool personHit = false;
            if (hit.collider != null)
            {
                CharacterHealth characterHealth = hit.collider.gameObject.GetComponent<CharacterHealth>();

                if (characterHealth)
                {
                    personHit = true;

                    characterHealth.ChangeHealth(-100.0f);

                    var bloodBurst = characterHealth.bloodBurst;

                    if (bloodBurst)
                    {
                        bloodBurst.Stop();

                        // particle velocity
                        {
                            //ParticleSystem.Particle[] p = new ParticleSystem.Particle[bloodBurst.particleCount + 1];
                            //int l = bloodBurst.GetParticles(p);

                            //int i = 0;
                            //while (i < l)
                            //{
                            //    p[i].velocity = new Vector3(direction.x * 20.0f, direction.y * 20.0f/*p[i].remainingLifetime / p[i].startLifetime * 10F*/, 0);
                            //    i++;
                            //}

                            //bloodBurst.SetParticles(p, l);
                        }

                        bloodBurst.transform.position = new Vector3(hit.point.x, hit.point.y, bloodBurst.transform.position.z);
                        bloodBurst.Play();
                    }

                    bloodSplashSource.Stop();
                    bloodSplashSource.pitch = CharacterMovement.GetRandomPitch(0.2f) * TimeManager.GetInstance().GetCurrentTimescale();
                    bloodSplashSource.PlayDelayed(0.05f);
                }
            }
            if (playerCollider)
            {
                playerCollider.enabled = true;
            }

            // camera shake
            cameraShake.originalPos = Camera.main.transform.localPosition;
            if (personHit)
            {
                cameraShake.shakeAmount = 0.5f;
                cameraShake.shakeDuration = 0.12f;
            }
            else
            {
                cameraShake.shakeAmount = 0.2f;
                cameraShake.shakeDuration = 0.1f;
            }
        }
    }

    void UpdateSpawning()
    {
        if (timeUntilFirstSpawn > 0.0f)
        {
            return;
        }

        if (NumEnemiesAlive < 3)
        {
            lastTimeSpawned += TimeManager.GetInstance().GetTimeDelta();

            if (lastTimeSpawned > 3.0f)
            {
                string prefab = "Prefabs/inspector";
                
                float randVal = Random.value;

                if (randVal > 0.67f)
                {
                    prefab = "Prefabs/barb";
                }
                else if (randVal > 0.34f)
                {
                    prefab = "Prefabs/rosie";
                }

                GameObject enemyObj = GameObject.Instantiate(Resources.Load(prefab)) as GameObject;

                if (enemyObj)
                {
                    enemyObj.transform.position = new Vector3(player.transform.position.x + (playerAnim.aimDirection * (Random.value * 20.0f)),
                                                                player.transform.position.y  + 12.0f,
                                                                player.transform.position.z);
                }
                

                lastTimeSpawned = 0.0f;

                NumEnemiesAlive++;
            }
        }
    }
}
