using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isEnabled;

    public bool AllowMovement;
    private bool stop;
    public PlayerController pc;
    public float detectDistance;
    public float speed;
    public bool chase;
    public LayerMask detectLayer;

    private Stats m_stats;
    private Camera m_cam;

    private Vector3 lastSeenSpot;

    public Slider healthBar;
    private Color m_barStartColor;

    public bool isMelee;
    public bool isRanged;
    public float escapeDistance;

    private Animator m_anim;

    public Weapon curWeapon;

    public bool HasGun
    {
        get
        {
            return GetComponent<Gun>() != null;
        }
    }

    private void Start()
    {
        pc = FindObjectOfType<PlayerController>();
        m_stats = GetComponent<Stats>();
        m_cam = FindObjectOfType<Camera>();
        m_barStartColor = healthBar.colors.normalColor;
        m_anim = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        if(isEnabled)
        {
            DoLogic();
            UpdateHealthBar();
            Shoot();
        }    
    }

    public void FixedUpdate()
    {
        if (isMelee)
        {
            if (chase && Vector3.Distance(transform.position, lastSeenSpot) >= 2 && !stop)
            {
                Move((lastSeenSpot - transform.position).normalized);

                if (m_anim != null)
                    m_anim.SetFloat("Speed", Mathf.Abs((lastSeenSpot - transform.position).normalized.magnitude));

                if (pc != null)
                    transform.LookAt(pc.transform.position);
            }
            else if (chase && Vector3.Distance(transform.position, lastSeenSpot) <= 2)
            {
                chase = false;
            }
            else
            {
                //do idle or move untill wall or something
                m_anim.SetFloat("Speed", 0);
            }


            if(Vector3.Distance(transform.position, pc.transform.position) <= 2)
            {
                if (curWeapon != null)
                {
                    if (curWeapon.GetType() == typeof(Sword))
                    {
                        Sword sword = (Sword)curWeapon;
                        if (sword.swingTimer <= 0)
                        {
                            stop = true;
                            sword.Swing();

                            if (m_anim != null)
                            {
                                int rnd = Random.Range(0, 2);
                                if (rnd == 0)
                                {
                                    m_anim.SetTrigger("Melee1");
                                }
                                else if (rnd == 1)
                                {
                                    m_anim.SetTrigger("Melee2");
                                }
                            }
                        }
                        else
                            stop = false;
                    }
                }
            }


        }
        else if (isRanged)
        {
            if(pc != null)
                transform.LookAt(pc.transform.position);

            if (chase && Vector3.Distance(transform.position, lastSeenSpot) <= escapeDistance)
            {
                Move((transform.position - lastSeenSpot).normalized);
                if (m_anim != null)
                    m_anim.SetFloat("Speed", Mathf.Abs((lastSeenSpot - transform.position).normalized.magnitude));
            }
            else if (chase && Vector3.Distance(transform.position, lastSeenSpot) >= escapeDistance)
            {
                chase = false;
            }
            else
            {
                //do idle or move untill wall or something
                if (m_anim != null)
                    m_anim.SetFloat("Speed", 0);
            }
        }

        if(transform.position.y < -20)
        {
            Destroy(gameObject);
        }
            
    }

    public void DoLogic()
    {
        if(pc != null)
            CheckForPlayer();
    }

    public void Shoot()
    {
        if(isRanged && HasGun)
        {
            if (GetComponent<Gun>().canShoot)
            {
                GetComponent<Gun>().Shoot(transform);

                if (GetComponentInChildren<Animator>())
                    GetComponentInChildren<Animator>().SetTrigger("Shoot");
            }      
        }
    }

    public void CheckForPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, (pc.transform.position - transform.position).normalized, out hit, 200, detectLayer);

        Debug.DrawLine(transform.position, hit.point, Color.red);

        if(hit.collider != null && hit.collider.gameObject != null 
            && hit.collider.gameObject == pc.gameObject && Vector3.Distance(transform.position, pc.transform.position) <= detectDistance)
        {
            chase = true;
            lastSeenSpot = pc.transform.position;
        }
    }

    protected void Move(Vector3 moveVector)
    {
        if (AllowMovement)
        {
            transform.Translate(moveVector * Time.fixedDeltaTime * speed, Space.World);
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.value = m_stats.health / m_stats.maxHealth;
        healthBar.transform.LookAt(healthBar.transform.position + m_cam.transform.rotation * Vector3.back,
                                       m_cam.transform.rotation * Vector3.down);
        /*float dist = Vector3.Distance(Camera.main.transform.position, healthBar.transform.position) * 0.025f;
        healthBar.transform.localScale = Vector3.one * dist;*/
    }

    public void FlashHealthBar()
    {
        Image im = healthBar.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        StartCoroutine(FlashHealthBar(im, 0.1f));
    }

    private IEnumerator FlashHealthBar(Image im, float dur)
    {
        if (!healthBar.transform.parent.gameObject.activeSelf)
            healthBar.transform.parent.gameObject.SetActive(true);
        im.color = Color.white;
        yield return new WaitForSeconds(dur);
        im.color = m_barStartColor;
    }
}
