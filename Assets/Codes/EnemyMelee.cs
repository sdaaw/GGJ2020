using System.Collections;
using System.Collections.Generic;
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

    private Vector3 lastSeenSpot;

    private void Start()
    {
        pc = FindObjectOfType<PlayerController>();
    }

    public void Update()
    {
        if(isEnabled)
            DoLogic();
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
}
