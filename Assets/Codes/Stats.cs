using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float health;

    public void TakeDmg(float dmg)
    {
        health -= dmg;
        if (health <= 0)
            Dead();
    }

    public void Dead()
    {
        //play dead animation
        //gg

        Destroy(gameObject);
    }
}
