using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
  [Header("Settings")]
  public float smoothSpeed = 10.0f;
  public float rotationSmoothSpeed = 5.0f;
  public Vector3 offset;
  public Player m_player;
  public Camera cameraRef;
  Rigidbody m_playerRB;

  // Start is called before the first frame update
  private void Start()
  {
    m_playerRB = m_player.GetComponent<Rigidbody>();
  }

  private void FixedUpdate()
  {
    Vector3 desiredPosition = Vector3.zero;
    Vector3 flatForward = transform.forward;
    flatForward.y = 0;
    flatForward.Normalize();
    desiredPosition = (transform.right * offset.x) + (transform.up * offset.y) + (flatForward * offset.z);

    Vector3 playerPos = m_player.transform.position;
    if (!m_player.m_alive)
    {
      playerPos = m_player.m_eggPlayer.transform.position;
    }

    if (playerPos.x > MapGenerator.instance.m_lastPlacedPosX-10)
    {
      playerPos.x = MapGenerator.instance.m_lastPlacedPosX - 20;
      playerPos.y = 4;
      playerPos.z = 0;
    }

    desiredPosition += playerPos;

    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

      transform.position = smoothedPosition;

    float angleY = Mathf.LerpAngle(transform.eulerAngles.y, transform.eulerAngles.y, rotationSmoothSpeed * Time.deltaTime);
    transform.eulerAngles = new Vector3(transform.eulerAngles.x, angleY, transform.eulerAngles.z);

  }
}