using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool AllowMovement;
    public float speed;

    private Rigidbody m_rigidbody;
    private Transform m_transform;

    public ParticleSystem dashParticle;

    private Quaternion m_oldRotation;
    private float m_horAxis;
    private float m_verAxis;
    private Vector3 m_move;
    private Vector3 m_mousePos;

    [SerializeField]
    private Camera m_playerCamera;

    [SerializeField]
    private LayerMask m_layerMask;

    public Animator animator;

    [SerializeField]
    private LayerMask m_dashMask;

    public float dashDistance;
    public float dashCooldown;
    private bool m_allowDash;

    public List<AudioClip> clips = new List<AudioClip>();
    public AudioSource bgMusic;

    public ShieldOrb soPrefab;

    private Animator m_anim;

    public Weapon CurWeapon;

    public Sprite[] playerHps;
    public Image playerHp;
    private Canvas m_playerUI;

    public Image playerDash;
    private float dashTimerForUI;

    private Stats m_stats;


    public bool HasGun
    {
        get
        {
            return GetComponent<Gun>() != null;
        }
    }

    public void SetMusic(bool wierdMusic)
    {
        if (bgMusic == null)
            return;

        if (!wierdMusic && bgMusic.clip != clips[0])
            bgMusic.clip = clips[0];
        else if (wierdMusic)
            bgMusic.clip = clips[1];

        if (!bgMusic.isPlaying)
            bgMusic.Play();
    }

    private void Awake()
    {
        m_transform = transform;
        m_rigidbody = GetComponent<Rigidbody>();
        m_allowDash = true;
        m_playerCamera = FindObjectOfType<Camera>();
        m_anim = GetComponentInChildren<Animator>();
        m_stats = GetComponent<Stats>();

        Canvas[] cvs = FindObjectsOfType<Canvas>();
        for(int i = 0; i < cvs.Length; i++)
        {
            if (cvs[i].name == "PlayerUI")
            {
                m_playerUI = cvs[i];
                break;
            }   
        }

        playerHp = m_playerUI.transform.GetChild(0).GetComponent<Image>();
        playerDash = m_playerUI.transform.GetChild(1).GetComponent<Image>();
    }

    private void Start()
    {
        AddShieldOrbs(3);
    }

    void FixedUpdate()
    {
        if (AllowMovement)
        {
            DoMovement();
            Rotate((MouseDir() - m_transform.position).normalized);
        }
    }

    private void Update()
    {
        if (!AllowMovement)
            return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(m_allowDash)
            {
                StartCoroutine(Dash());
                dashTimerForUI = 0;
            }     
        }

        if (dashTimerForUI < dashCooldown)
            dashTimerForUI += Time.deltaTime;

        UpdateDashImage();

        if (Input.GetKeyDown(KeyCode.Mouse0) && CurWeapon != null)
        {
            //check all weapons here
            if(CurWeapon.GetType() == typeof(Gun))
            {
                ((Gun)CurWeapon).Shoot(m_transform);
                if (m_anim != null)
                    m_anim.SetTrigger("Shoot");
            }
            else if(CurWeapon.GetType() == typeof(Sword))
            {
                Sword sword = (Sword)CurWeapon;
                if (m_anim != null && sword.canMelee)
                {
                    sword.Swing();
                    m_anim.SetTrigger("Swing1");
                }     
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && CurWeapon != null)
        {
            //check all weapons here
            if (CurWeapon.GetType() == typeof(Gun))
            {
                ((Gun)CurWeapon).Shoot(m_transform);
                if (m_anim != null)
                    m_anim.SetTrigger("Shoot2");
            }
            else if (CurWeapon.GetType() == typeof(Sword))
            {
                Sword sword = (Sword)CurWeapon;
                if (m_anim != null && sword.canMelee)
                {
                    sword.Swing();
                    m_anim.SetTrigger("Swing2");
                }
            }
        }

        //Debug.Log(m_move);
        if (m_anim != null)
            m_anim.SetFloat("Speed", Mathf.Abs(m_move.magnitude) );
    }

    void DoMovement()
    {
        m_oldRotation = m_playerCamera.transform.rotation;

        Vector3 temp = m_oldRotation.eulerAngles;
        temp.x = 0;
        m_playerCamera.transform.rotation = Quaternion.Euler(temp);

        m_horAxis = Input.GetAxis("Horizontal");
        m_verAxis = Input.GetAxis("Vertical");

        m_move.x = m_horAxis;
        m_move.y = 0;
        m_move.z = m_verAxis;

        m_move = m_playerCamera.transform.TransformDirection(m_move);

        m_playerCamera.transform.rotation = m_oldRotation;

        m_move.y = 0;

        Move(m_move);
    }


    protected void Move(Vector3 moveVector)
    {
        if (AllowMovement)
        {
            m_transform.Translate(moveVector * Time.fixedDeltaTime * speed, Space.World);
        }
    }

    protected void Rotate(Vector3 rotateVector)
    {
        if (AllowMovement)
        {
            m_transform.rotation = Quaternion.LookRotation(new Vector3(rotateVector.x,0,rotateVector.z));
        }
    }

    private IEnumerator Dash()
    {
        m_allowDash = false;

        Vector3 dir = m_transform.forward;

        if (m_move.magnitude > 0)
        {
            dir = m_move;
        }

        RaycastHit[] hit = Physics.RaycastAll(m_transform.position, dir, dashDistance + 1, m_dashMask);

        if (hit.Length > 1 && hit[1].collider != null)
        {
            //???
            m_transform.position = hit[1].point - dir;
        }
        else
        {
            m_transform.Translate(dir * dashDistance, Space.World);
        }
        dashParticle.Play();
        yield return new WaitForSeconds(dashCooldown);

        m_allowDash = true;
    }

    private Vector3 MouseDir()
    {
        m_mousePos.x = Input.mousePosition.x;
        m_mousePos.y = Input.mousePosition.y;
        m_mousePos.z = Camera.main.WorldToScreenPoint(m_transform.position).z;

        return Camera.main.ScreenToWorldPoint(m_mousePos);
    }

    public void AddShieldOrbs(int amount)
    {
        StartCoroutine(WaitSpawnShieldOrbs(amount));
    }

    private IEnumerator WaitSpawnShieldOrbs(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            ShieldOrb so = Instantiate(soPrefab, m_transform);
            so.owner = m_transform;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpdateHealthImage()
    {
        int hp = (int)m_stats.health;
        playerHp.sprite = playerHps[hp - 1];
    }

    public void UpdateDashImage()
    {
        playerDash.fillAmount = dashTimerForUI / dashCooldown;
    }
}