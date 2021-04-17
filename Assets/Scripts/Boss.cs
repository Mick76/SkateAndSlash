using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyEgg
{
  EnemyMouth m_mouth;
  float m_spawnEnemyInterval = 2.0f;
  float m_timeCounter = 0.0f;

  public GameObject m_enemyPrefab;
  public Transform m_spawnPoint;

  public LayerMask m_layer;

  float groundHeight = 0;

  // Start is called before the first frame update
  void Start()
  {
    m_mouth = GetComponent<EnemyMouth>();
    m_audioSource = GetComponent<AudioSource>();
    collider = GetComponent<Collider>();
  }

  // Update is called once per frame
  void Update()
  {
    if (m_dead)
    {
      return;
    }

    m_timeCounter += Time.deltaTime;
    if (m_timeCounter > m_spawnEnemyInterval)
    {
      m_timeCounter = 0.0f;

      int enemiesToSpawn = Random.Range(1, 4);
      for (int i = 0; i < enemiesToSpawn; i++)
      {
        Invoke("SpawnEnemy", i* 0.3f);
      }
      
      m_mouth.OpenMouth();
    }
  }

  private void FixedUpdate()
  {
    if (m_dead)
    {
      return;
    }

    RaycastHit hit;
    if (Physics.SphereCast(transform.position, 0.1f, Vector3.down, out hit, 50, m_layer))
    {
      Debug.DrawLine(transform.position, hit.point, Color.green);
      groundHeight = hit.point.y;
    }

    //move
    if (transform.position.x < MapGenerator.instance.m_lastPlacedPosX-10)
    {
      Vector3 newPos = transform.position;
      newPos = Vector3.Lerp(transform.position, new Vector3(Player.instance.transform.position.x + 60,
                            10 + groundHeight,
                            Player.instance.transform.position.z), Time.deltaTime * 4.0f);
      transform.position = newPos;
    }
    else
    {
      Vector3 newPos = transform.position;
      newPos = Vector3.Lerp(transform.position, new Vector3(MapGenerator.instance.m_lastPlacedPosX + 10,
                            9 + groundHeight,
                            0), Time.deltaTime * 4.0f);
      transform.position = newPos;
    }

  }

  void SpawnEnemy()
  {
    Vector3 spawnPos = m_spawnPoint.position;
    spawnPos.z += Random.Range(-1.0f, 1.0f) * 5;
    Instantiate(m_enemyPrefab, spawnPos, Quaternion.identity);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      m_dead = true;
      m_topEgg.isKinematic = false;
      m_bottomEgg.isKinematic = false;

      m_topEgg.AddForce(transform.right * 70, ForceMode.Impulse);
      m_bottomEgg.AddForce(-transform.right * 70, ForceMode.Impulse);

      m_audioSource.PlayOneShot(m_eggScream, 1.0f);
      m_audioSource.PlayOneShot(m_eggCut, 1.0f);
      collider.enabled = false;
      MapGenerator.instance.WinGame();
      Destroy(gameObject, 10.0f);
    }
  }
}
