using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggPlayer : MonoBehaviour
{
  public Player m_skate;
  public Rigidbody m_rb;
  public Rigidbody m_rightHand;
  public Rigidbody m_leftHand;

  // Start is called before the first frame update
  void Start()
  {
    m_rb = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update()
  {
    transform.forward = m_skate.transform.forward;
    transform.right = m_skate.m_boardPivot.transform.right;
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!m_skate.m_alive)
    {
      m_skate.m_fxAudioSource.PlayOneShot(m_skate.m_die, 1.0f);
    }
  }

  public void Die()
  {
    Destroy(GetComponent<SpringJoint>());
    Destroy(GetComponent<CenterOfMass>());
    m_rb.AddForce(Vector3.right * 20, ForceMode.Impulse);
    m_rb.AddForce(Vector3.up * 20, ForceMode.Impulse);
    m_rb.freezeRotation = false;
    m_rb.drag = 0.5f;
    m_rb.AddTorque(Vector3.right * 50, ForceMode.Impulse);
  }
}
