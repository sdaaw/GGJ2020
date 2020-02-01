using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    public float swingTimer;
    public float swingTimerMax;
    public bool canMelee;

    public float swingDuration;
    public float maxSwingDuration;
    public bool canDealDamage;

    private void Update()
    {
        if(swingTimer > 0)
            swingTimer -= Time.deltaTime;
        if(swingTimer < 0)
        {
            swingTimer = 0;
            canMelee = true;
        }

        if (swingDuration > 0)
        {
            canDealDamage = true;
            swingDuration -= Time.deltaTime;
        }
            
        if(swingDuration < 0)
        {
            swingDuration = 0;
            canDealDamage = false;
        }
    }

    public void Swing()
    {
        canMelee = false;
        swingTimer = swingTimerMax;
        swingDuration = maxSwingDuration;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root != owner && other.gameObject.layer != gameObject.layer)
        {
            if (other.GetComponent<Stats>() && canDealDamage)
            {
                other.GetComponent<Stats>().TakeDmg(damage);
            }
        }
    }
}
