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
    public bool canDash = true;
    private bool m_allowDash;

    public List<AudioClip> clips = new List<AudioClip>();
    public AudioSource bgMusic;

    public ShieldOrb soPrefab;

    private Animator m_anim;

    public Weapon CurWeapon;

    public bool hasShield;

    public Sprite[] playerHps;
    public Image playerHp;
    private Canvas m_playerUI;
    public ParticleSystem swordTrail;

    public Image playerDash;
    private float dashTimerForUI;

    public Sprite playerIcon;
    private Image playerIconIMG;
    private Image playerIconCD;

    private Image hasWepPickup;
    private Image hasOrbPickup;
    private Image hasShieldPickup;

    private Stats m_stats;


    public bool HasGun
    {
        get
        {
            return GetComponent<Gun>() != null;
        }
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
        playerIconIMG = m_playerUI.transform.GetChild(2).GetComponent<Image>();
        playerIconCD = m_playerUI.transform.GetChild(3).GetComponent<Image>();

        hasWepPickup = playerIconIMG.transform.GetChild(0).GetComponent<Image>();
        hasOrbPickup = playerIconIMG.transform.GetChild(1).GetComponent<Image>();
        hasShieldPickup = playerIconIMG.transform.GetChild(2).GetComponent<Image>();

        playerIconIMG.sprite = playerIcon;
    }

    private void Start()
    {
        //UpdateHealthImage();
        //AddShieldOrbs(3);
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
            if(m_allowDash && canDash)
            {
                StartCoroutine(Dash());
                dashTimerForUI = 0;
            }     
        }

        if (dashTimerForUI < dashCooldown)
            dashTimerForUI += Time.deltaTime;

        UpdateDashImage();
        UpdatePlayerIconCD();
        UpdatePickupsUI();

        //SHOOTING & SPELLS
        if (CurWeapon.GetType() == typeof(Gun))
        {
            //primary fire
            if (Input.GetKeyDown(KeyCode.Mouse0) && CurWeapon != null)
            {
                Gun gun = (Gun)CurWeapon;
                if(gun.canShoot)
                {
                    gun.Shoot(m_transform);
                    if (m_anim != null)
                        m_anim.SetTrigger("Shoot");
                } 
            }

            //aoe shooting thing
            if (Input.GetKeyDown(KeyCode.Mouse1) && CurWeapon != null)
            {
                Gun gun = (Gun)CurWeapon;
                if(gun.canShoot2)
                {
                    gun.Shoot2(m_transform);
                    if (m_anim != null)
                        m_anim.SetTrigger("Shoot2");
                }
            }
        }
        //MELEE
        else if (CurWeapon.GetType() == typeof(Sword))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && CurWeapon != null)
            {
                Sword sword = (Sword)CurWeapon;
                if (m_anim != null && sword.canMelee)
                {
                    sword.Swing();
                    m_anim.SetTrigger("Swing1");
                }
            }

            //press
            if (Input.GetKeyDown(KeyCode.Mouse1) && CurWeapon != null)
            {
                Sword sword = (Sword)CurWeapon;
                sword.canDealDamage = true;
                m_anim.SetBool("bladestorm", true);
                canDash = false;
                swordTrail.Play();

            }
            //release
            if (Input.GetKeyUp(KeyCode.Mouse1) && CurWeapon != null)
            {
                Sword sword = (Sword)CurWeapon;
                sword.canDealDamage = false;
                m_anim.SetBool("bladestorm", false);

                canDash = true;
                swordTrail.Stop();
            }
        }

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
        playerHp.sprite = playerHps[hp];
    }

    public void UpdateDashImage()
    {
        playerDash.fillAmount = dashTimerForUI / dashCooldown;
    }

    public void UpdatePlayerIconCD()
    {
        //this might be very inefficient check to be called in update.. but its jam game and im in hurry
        if(CurWeapon != null && CurWeapon.GetType() == typeof(Gun))
        {
            Gun gun = (Gun)CurWeapon;
            playerIconCD.fillAmount = 1 - (gun.GetShoot2Timer / gun.shoot2Delay);
        }
    }

    public void UpdatePickupsUI()
    {
        if (GetComponentInChildren<ShieldOrb>() != null)
            hasOrbPickup.gameObject.SetActive(true);
        else
            hasOrbPickup.gameObject.SetActive(false);

        if(CurWeapon.GetType() == typeof(Gun))
        {
            Gun gun = (Gun)CurWeapon;
            if (gun.upgradeLvl > 0)
                hasWepPickup.gameObject.SetActive(true);
            else
                hasWepPickup.gameObject.SetActive(false);
        }

        if (hasShield)
            hasShieldPickup.gameObject.SetActive(true);
        else
            hasShieldPickup.gameObject.SetActive(false);
    }

    public void Dead()
    {
        m_anim.SetTrigger("Death");
        AllowMovement = false;

        //needed to stop spin animation
        if (CurWeapon != null && CurWeapon.GetType() == typeof(Sword))
            m_anim.SetBool("bladestorm", false);

        if (GetComponent<Gun>())
            GetComponent<Gun>().enabled = false;

        if (GetComponentInChildren<Sword>())
            GetComponentInChildren<Sword>().enabled = false;
    }
}