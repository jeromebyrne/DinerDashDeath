using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera m_Camera;
    public CharacterMovement playerMovement = null;
    private PlayerAnimationController playerAnim = null;
    private float cameraSpeed = 1.35f;
    private const float kPlayerOffset = 6.0f;

    private Vector3 startingPos = new Vector3();

    // Use this for initialization
    void Start()
    {
        startingPos = gameObject.transform.position;

        playerAnim = playerMovement.gameObject.GetComponent<PlayerAnimationController>();
    }

    public void SetPlayer(CharacterMovement player)
    {
        playerMovement = player;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerMovement)
        {
            var lastPos = m_Camera.transform.position;

            Vector3 newPos = new Vector3(playerMovement.transform.position.x + (playerAnim.aimDirection * kPlayerOffset),
                                        startingPos.y,
                                        m_Camera.transform.position.z);

            m_Camera.transform.position = Vector3.Lerp(lastPos, newPos, TimeManager.GetInstance().GetTimeDelta() * cameraSpeed);
        }
    }
}
