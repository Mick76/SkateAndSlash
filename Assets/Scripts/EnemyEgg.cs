using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class EnemyEgg : MonoBehaviour
{
  public Rigidbody m_topEgg;
  public Rigidbody m_bottomEgg;
  public AudioClip m_eggBreak;
  public AudioClip m_eggCut;
  public AudioClip m_eggScream;
  public AudioSource m_audioSource;
  public Collider collider;
  public bool m_dead = false;

  // Start is called before the first frame update
  void Start()
  {
    m_audioSource = GetComponent<AudioSource>();
    collider = GetComponent<Collider>();
  }

  // Update is called once per frame
  void Update()
  {
    Vector3 playerDir = Player.instance.transform.position - transform.position;
    playerDir.Normalize();
    transform.position += playerDir * 5 * Time.deltaTime;

    Vector3 newPos = transform.position;
    newPos.y = Mathf.Lerp(newPos.y, Player.instance.transform.position.y + 0.55f, Time.deltaTime * 10);
    transform.position = newPos;

    if (transform.position.x < Player.instance.transform.position.x - 10 && !m_dead)
    {
      Destroy(gameObject);
    }


  }

  
  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      if (!Player.instance.m_doingTrick && Player.instance.transform.position.y < 0.7f)
      {
        Player.instance.Die();
        return;
      }
      m_dead = true;
      Player.instance.m_enemiesKilled++;
      CameraShaker.Instance.ShakeOnce(2, 5, 0, 1);

      m_topEgg.isKinematic = false;
      m_bottomEgg.isKinematic = false;

      m_topEgg.AddForce(transform.forward*30, ForceMode.Impulse);
      m_bottomEgg.AddForce(-transform.forward*30, ForceMode.Impulse);

      m_audioSource.PlayOneShot(m_eggCut, 1.0f);
      m_audioSource.PlayOneShot(m_eggScream, 1.0f);
      collider.enabled = false;
      Destroy(gameObject, 3.0f);
    }
  }
}
