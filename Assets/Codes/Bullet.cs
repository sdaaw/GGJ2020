using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float dmg;
    public bool isActive;
    private float speed;
    private Vector3 dir;
    private Transform owner;


    public void Activate(float speed, Vector3 dir, Transform owner, float dmg)
    {
        this.speed = speed;
        this.dir = dir;
        this.owner = owner;

        if (dmg > 0)
            this.dmg = dmg;

        isActive = true;
    }

    private void FixedUpdate()
    {
        if(isActive)
        {
            transform.Translate(dir * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //its not our player (cant shoot itself)
        if(other.transform != owner)
        {
            //its not a bullet
            if(other.gameObject.layer != 8)
            {
                if (other.GetComponent<Stats>())
                    other.GetComponent<Stats>().TakeDmg(dmg);

                Destroy(gameObject);
            }
        }      
    }
}
