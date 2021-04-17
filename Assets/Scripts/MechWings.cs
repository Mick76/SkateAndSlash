using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechWings : MonoBehaviour
{

  public float m_angle = 35.0f;
  public float m_speed = 20.0f;
  private float m_elapsedTime = 0;

  void Update()
  {
    m_elapsedTime += Time.deltaTime;
    Vector3 euler = transform.localEulerAngles;
    euler.x = Mathf.Sin(m_elapsedTime * m_speed) *m_angle;
    transform.localEulerAngles = euler;
  }
}
