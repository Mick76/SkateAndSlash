using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMouth : MonoBehaviour
{
  public float m_maxAngle = 45;
  public bool m_lookAtPlayer = false;
  public bool m_chewAnim = true;

  [Header("Bite settings")]
  public bool m_opening = false;
  public Transform upMouth;
  public Transform jaw;
  public float m_upMouthApertureAngle = -30.0f;
  public float m_jawApertureAngle = 36.0f;
  public float apertureSpeed = 20.0f;
  public float m_animTime = 0.2f;
  float timeCounter = 0.0f;

  float startJawAngle;
  float startUpMouthAngle;

// Start is called before the first frame update
void Start()
  {
    startJawAngle = jaw.localEulerAngles.x;
    startUpMouthAngle = upMouth.localEulerAngles.x;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.B))
    {
      OpenMouth();
    }
    if (Input.GetKeyDown(KeyCode.V))
    {
      CloseMouth();
    }

    if (m_lookAtPlayer)
    {
      Vector3 lookVector = ((Player.instance.transform.position + (Vector3.up * 2)) - transform.position).normalized;
      if (Vector3.Angle(lookVector, transform.parent.forward) < m_maxAngle)
      {
        transform.LookAt(Player.instance.transform.position + (Vector3.up * 2));
      }
    }

    if (m_opening)
    {
      timeCounter += Time.deltaTime;

      Vector3 euler = jaw.localEulerAngles;
      euler.x = Mathf.LerpAngle(startJawAngle, m_jawApertureAngle, timeCounter/m_animTime);
      jaw.localEulerAngles = euler;

      euler = upMouth.localEulerAngles;
      euler.x = Mathf.LerpAngle(startUpMouthAngle, m_upMouthApertureAngle, timeCounter / m_animTime);
      upMouth.localEulerAngles = euler;

      if (timeCounter >= m_animTime)
      {
        timeCounter = 0.0f;
        CloseMouth();
      }
    }
    else
    {
      timeCounter += Time.deltaTime;

      Vector3 euler = jaw.localEulerAngles;
      euler.x = Mathf.LerpAngle(m_jawApertureAngle, 0.0f, timeCounter / m_animTime);
      jaw.localEulerAngles = euler;

      euler = upMouth.localEulerAngles;
      euler.x = Mathf.LerpAngle(m_upMouthApertureAngle, 0.0f, timeCounter / m_animTime);
      upMouth.localEulerAngles = euler;

      if (timeCounter >= m_animTime && m_chewAnim)
      {
        timeCounter = 0.0f;
        OpenMouth();
      }
    }

  }

  public void OpenMouth()
  {
    timeCounter = 0.0f;
    m_opening = true;
  }

  public void CloseMouth()
  {
    timeCounter = 0.0f;
    m_opening = false;
  }
}
