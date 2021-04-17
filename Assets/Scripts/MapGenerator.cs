using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapGenerator : MonoBehaviour
{
  public bool m_gameStarted = false;
  public Player m_player;
  public EnemyEgg m_boss;
  public GameObject[] m_floorPrefab;
  public GameObject m_levelEnd;
  public int m_chunkCount = 30;
  public int currLevel = 0;
  [HideInInspector]
  public float m_lastPlacedPosX = 20;
  public GameObject m_winText;
  public bool m_hasWinned = false;

  [Header("sound settings")]
  public AudioSource m_music;

  [Header("UI settings")]
  public GameObject m_mainMenu;
  public GameObject m_inGameUI;
  public Image m_deathBackGround;
  public TextMeshProUGUI m_restartText;
  public TextMeshPro m_recordText;
  public Slider m_slider;

  List<ChunkDesc> m_floorsStack = new List<ChunkDesc>();

  public static MapGenerator instance = null;
  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else
    {
      Destroy(this);
    }

  }

  // Start is called before the first frame update
  void Start()
  {
    m_winText.SetActive(false);

    m_mainMenu.SetActive(true);
    m_inGameUI.SetActive(false);
    m_boss.gameObject.SetActive(false);
    m_player.gameObject.SetActive(false);
    m_player.m_eggPlayer.gameObject.SetActive(false);
    m_music.gameObject.SetActive(false);
  }

  // Update is called once per frame
  void Update()
  {
    if (!m_gameStarted)
    {
      if (Input.GetKeyDown(KeyCode.Space))
      {
        m_gameStarted = true;
        m_mainMenu.SetActive(false);
        m_inGameUI.SetActive(true);
        m_boss.gameObject.SetActive(true);
        m_player.gameObject.SetActive(true);
        m_player.m_eggPlayer.gameObject.SetActive(true);
        m_music.gameObject.SetActive(true);
        GenerateMap();
      }
    }

    if (!m_player.m_alive)
    {
      Color currColor = m_deathBackGround.color;
      currColor.a = Mathf.Lerp(currColor.a, 1, Time.deltaTime * 2);
      m_deathBackGround.color = currColor;

      Color currRestartColor = m_restartText.color;
      currRestartColor.a = Mathf.Lerp(currRestartColor.a, 1, Time.deltaTime);
      m_restartText.color = currRestartColor;

      m_music.volume = Mathf.Lerp(m_music.volume, 0, Time.deltaTime * 2);
    }
  }

  void GenerateMap()
  {
    for (int i = 1; i < m_slider.value; i++)
    {
      ChunkDesc newChunk = Instantiate(m_floorPrefab[Random.Range(0, m_floorPrefab.Length)], new Vector3(m_lastPlacedPosX, -1, 20.0f), Quaternion.identity).GetComponent<ChunkDesc>();
      m_lastPlacedPosX += newChunk.m_lenght;
      m_floorsStack.Add(newChunk);
    }
    ChunkDesc levelEnd = Instantiate(m_levelEnd, new Vector3(m_lastPlacedPosX, -1, 20.0f), Quaternion.identity).GetComponent<ChunkDesc>();
    m_lastPlacedPosX += levelEnd.m_lenght;

  }

  public void WinGame()
  {
    m_hasWinned = true;
    Vector3 newPos = m_winText.transform.position;
    newPos.x = m_lastPlacedPosX + 10;
    m_winText.transform.position = newPos;
    m_winText.SetActive(true);
    m_recordText.text = "Enemies Slashed: " + m_player.m_enemiesKilled.ToString();
  }
}
