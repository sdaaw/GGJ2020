using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public bool isEnabled;

    public bool AllowMovement;
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

    private void Start()
    {
        pc = FindObjectOfType<PlayerController>();
        m_stats = GetComponent<Stats>();
        m_cam = FindObjectOfType<Camera>();
        m_barStartColor = healthBar.colors.normalColor;
    }

    public void Update()
    {
        if(isEnabled)
        {
            DoLogic();
            UpdateHealthBar();
        }    
    }

    public void FixedUpdate()
    {
        if(chase && Vector3.Distance(transform.position, lastSeenSpot) >= 1)
        {
            //ranged enemy ai?
            //Move((transform.position - lastSeenSpot).normalized);

            Move((lastSeenSpot - transform.position).normalized);
        }
        else if(chase && Vector3.Distance(transform.position, lastSeenSpot) <= 1)
        {
            chase = false;
        }
        else
        {
            //do idle or move untill wall or something
        }
    }

    public void DoLogic()
    {
        if(pc != null)
            CheckForPlayer();
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
