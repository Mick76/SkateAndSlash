using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class Player : MonoBehaviour
{
  public EggPlayer m_eggPlayer;
  public bool m_alive = true;

  [Header("Raycast Settings")]
  public Transform[] m_raycastPoints;
  public LayerMask m_layer;

  [Header("Move Settings")]
  public float m_repulsionForce = 100.0f;
  public float m_acceleration = 20.0f;
  public float m_maxVelocity = 20.0f;
  public float m_defaultElevation = 0.25f;
  bool m_usingBoost = false;

  [Header("Rotation Settings")]
  public Transform m_allPivot;
  public Transform m_boardPivot;
  public float angDrag;
  public float spinForce;
  public float maxAngVel;
  Rigidbody m_rb;
  bool m_rotating = false;
  bool m_rotatingRight = false;
  bool m_rotatingLeft = false;
  bool m_braking = false;

  [Header("Sound FX")]
  public AudioClip m_rollingSFX;
  public AudioClip m_onCollisionSFX;
  public AudioClip m_jumpSFX;
  public AudioClip m_squeak1;
  public AudioClip m_squeak2;
  public AudioClip m_whosh;
  public AudioClip m_die;
  AudioSource m_wheelsAudioSource;
  [HideInInspector]
  public AudioSource m_fxAudioSource;
  bool m_fading = false;

  public bool m_grounded = false;

  [Header("Trick Settings")]
  public GameObject m_edge;
  public float m_jumpPower = 80.0f;
  public float m_jumpAnglePower = 80.0f;
  public float m_shuvitSpinPower = 80.0f;
  public float m_KickflipSpinPower = 80.0f;
  public float m_dolphinflipSpinPower = 80.0f;
  public float m_maxTrickAngVel = 2;
  bool m_jump = false;
  bool m_jumping = false;
  public Vector3 m_oldForward = Vector3.zero;
  public Vector3 m_oldRight = Vector3.zero;
  public Vector3 m_oldUp = Vector3.zero;

  public bool m_doingTrick = false;
  float m_trickDuration = 1.0f;
  float trickTimeCounter = 0.0f;

  float m_groundedCooldown = 0.5f;
  float groundedTimeCounter = 0.6f;

  [Header("record Settings")]
  public int m_enemiesKilled = 0;

  //shaker
  CameraShakeInstance shake;
  float magn = 1, rough = 1, fadeIn = 0.1f, fadeOut = 2f;

  public static Player instance;

  private void Awake()
  {
    instance = this;
  }

  // Start is called before the first frame update
  void Start()
  {
    m_rb = GetComponent<Rigidbody>();

    AudioSource[] audioSources = GetComponents<AudioSource>();
    m_wheelsAudioSource = audioSources[0];
    m_fxAudioSource = audioSources[1];
    m_wheelsAudioSource.clip = m_rollingSFX;
    m_wheelsAudioSource.Play();

    shake = CameraShaker.Instance.StartShake(magn, rough, fadeIn);
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.R))
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    if (!m_alive)
    {
      return;
    }

    if (m_doingTrick || transform.position.y > 0.7f)
    {
      m_edge.SetActive(true);
    }
    else
    {
      m_edge.SetActive(false);
    }

    if (transform.position.y < -10 && !MapGenerator.instance.m_hasWinned)
    {
      Die();
    }

    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.DownArrow))
    {
      if (m_grounded)
      {
        m_jump = true;
      }
    }

    if (m_doingTrick)
    {
      trickTimeCounter += Time.deltaTime;
      if (trickTimeCounter > m_trickDuration)
      {
        trickTimeCounter = 0.0f;
        m_doingTrick = false;
      }
    }

    //if (!m_grounded)
    if (!m_doingTrick || !m_grounded)
    {
      m_rb.maxAngularVelocity = m_maxTrickAngVel;
      if (Input.GetKeyDown(KeyCode.RightArrow))
      {
        DoTrick();
        Debug.Log("shovit");
        m_doingTrick = true;
        m_rb.AddTorque(transform.up * m_shuvitSpinPower, ForceMode.Impulse);
      }
      else if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
        DoTrick();
        Debug.Log("shovitL");
        m_doingTrick = true;
        m_rb.AddTorque(transform.up * -m_shuvitSpinPower, ForceMode.Impulse);
      }
    }

    //rotation effects
    if ((m_rotating && m_rb.velocity.magnitude < 5.0f) || m_braking)
    {
      //Vector3 euler = m_allPivot.localEulerAngles;
      //euler.x = Mathf.LerpAngle(euler.x, -20, Time.deltaTime * 10.0f);
      //m_allPivot.localEulerAngles = euler;
    }
    else
    {
      Vector3 euler = m_allPivot.localEulerAngles;
      euler.x = Mathf.LerpAngle(euler.x, 0, Time.deltaTime * 10.0f);
      m_allPivot.localEulerAngles = euler;

      euler = m_boardPivot.localEulerAngles;
      if (m_rotatingLeft)
      {
        euler.z = Mathf.LerpAngle(euler.z, 16, Time.deltaTime * 10.0f);
      }
      else if (m_rotatingRight)
      {
        euler.z = Mathf.LerpAngle(euler.z, -16, Time.deltaTime * 10.0f);
      }
      else
      {
        euler.z = Mathf.LerpAngle(euler.z, 0, Time.deltaTime * 10.0f);
      }

      m_boardPivot.localEulerAngles = euler;
    }

    Vector3 eulerTransform = transform.localEulerAngles;
    eulerTransform.z = Mathf.LerpAngle(eulerTransform.z, 0, Time.deltaTime * 4.0f);
    transform.localEulerAngles = eulerTransform;

    //wheel sound fx
    m_wheelsAudioSource.volume = (m_rb.velocity.magnitude / m_maxVelocity) * 0.2f;

    int groundedWheels = 0;
    foreach (Transform point in m_raycastPoints)
    {
      RaycastHit hit;
      if (Physics.SphereCast(point.position, 0.1f, Vector3.down, out hit, 1, m_layer))
      {
        Debug.DrawLine(point.position, hit.point, Color.red);
        groundedWheels++;
      }
    }


    if (groundedWheels != 0)
    {
      if (!m_grounded)
      {
        m_grounded = true;
      }
    }
    else
    {
      if (m_grounded) //if it was grounded but now not
      {
        m_grounded = false;
        m_oldForward = transform.forward;
        m_oldRight = transform.right;
        m_oldUp = transform.up;
      }
    }

    if (!m_doingTrick)
    {
      transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.right, Vector3.up), Time.deltaTime * 100);
    }

  }

  private void FixedUpdate()
  {
    if (!m_alive)
    {
      return;
    }

    foreach (Transform point in m_raycastPoints)
    {
      RaycastHit hit;
      if (Physics.SphereCast(point.position, 0.1f, Vector3.down, out hit, 10, m_layer))
      {
        Debug.DrawLine(point.position, hit.point, Color.green);
        float force = m_defaultElevation / hit.distance * m_repulsionForce;
        force = Mathf.Clamp(force, 0, 10.0f);
        m_rb.AddForceAtPosition(force * Vector3.up, point.position,ForceMode.Acceleration);
      }
    }

    //accelerate
    m_rb.AddForce(Vector3.right * m_acceleration, ForceMode.Acceleration);

    if (Input.GetKey(KeyCode.W))
    {
      if (m_grounded)
      {
        m_rb.AddForce(transform.forward * m_acceleration, ForceMode.Acceleration);

      }
      else
      {
        Vector3 flatOldForward = m_oldForward;
        flatOldForward.y = 0;
        m_rb.AddForce(flatOldForward * m_acceleration, ForceMode.Acceleration);
      }
    }

    //rotate
    if (!m_doingTrick)
    {
      m_rb.maxAngularVelocity = maxAngVel;
    }

    m_rb.angularDrag = angDrag;
    if (Input.GetKey(KeyCode.A))
    {
      if (!m_rotatingLeft)
      {
        m_fxAudioSource.PlayOneShot(m_squeak1, 1.0f);
      }
      m_rotating = true;
      m_rotatingLeft = true;
      m_rotatingRight = false;
      m_rb.AddForce(transform.right * -m_acceleration, ForceMode.Acceleration);
    }
    else if (Input.GetKey(KeyCode.D))
    {
      if (!m_rotatingRight)
      {
        m_fxAudioSource.PlayOneShot(m_squeak2, 1.0f);
      }
      m_rotating = true;
      m_rotatingRight = true;
      m_rotatingLeft = false;
      m_rb.AddForce(transform.right * m_acceleration, ForceMode.Acceleration);
    }
    else
    {
      m_rotatingRight = false;
      m_rotatingLeft = false;
      m_rotating = false;
    }

    if (m_grounded)
    {
      //brake
      if (Input.GetKey(KeyCode.S))
      {
        m_rb.AddForce(-transform.forward * m_acceleration, ForceMode.Acceleration);
        m_braking = true;
      }
      else
      {
        m_braking = false;
      }

      

      //jump
      if (m_jump && m_grounded && !m_jumping)
      {
        m_jump = false;
        m_oldForward = transform.forward;
        m_oldRight = transform.right;
        m_oldUp = transform.up;
        StartCoroutine(Ollie());
      }

    }

    if (!m_grounded)
    {
      m_rb.maxAngularVelocity = m_maxTrickAngVel;
    }

    if (m_rb.velocity.magnitude > m_maxVelocity && !m_usingBoost)
    {
      m_rb.velocity = m_rb.velocity.normalized * m_maxVelocity;
    }

    Vector3 velXZ = m_rb.velocity;
    velXZ.y = 0;

    Vector3 velY = m_rb.velocity;
    velY.x = 0;
    velY.z = 0;

    Vector3 flatForward = transform.forward;
    if (!m_grounded)
    {
      flatForward = m_oldForward;
    }
    flatForward.y = 0;
    Vector3 newVel = (flatForward.normalized * velXZ.magnitude) + velY;

  }

  IEnumerator Ollie()
  {
    CameraShaker.Instance.ShakeOnce(2, 5, 0, 1);

    m_jumping = true;
    m_fxAudioSource.PlayOneShot(m_jumpSFX, 1.0f);
    m_rb.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
    m_rb.AddTorque(transform.right * -m_jumpAnglePower, ForceMode.Impulse);
    yield return new WaitForSeconds(0.15f);
    m_rb.AddTorque(transform.right * m_jumpAnglePower * 1.0f, ForceMode.Impulse);
    m_jumping = false;
  }

  IEnumerator Land()
  {
    float startTime = Time.time;
    while (Time.time - startTime < 0.4f)
    {
      if (!m_grounded)
      {
        break;
      }
      Vector3 flatForward = m_oldForward;
      flatForward.y = 0.0f;

      transform.forward = Vector3.Lerp(transform.forward, flatForward.normalized, Time.deltaTime * 5);

      yield return new WaitForFixedUpdate();
    }
  }

  void DoTrick()
  {
    trickTimeCounter = 0.0f;
    m_oldForward = transform.forward;
    m_oldRight = transform.right;
    m_oldUp = transform.up;
    m_fxAudioSource.PlayOneShot(m_whosh, 1.0f);
  }

  public void Die()
  {
    if (m_alive)
    {
      m_fxAudioSource.PlayOneShot(m_die, 1.0f);
      m_alive = false;
      m_eggPlayer.Die();
      m_eggPlayer.enabled = false;
      StartCoroutine(GameOver());
    }
    
  }

  IEnumerator GameOver()
  {
    yield return new WaitForSeconds(1.0f);
  }

}
