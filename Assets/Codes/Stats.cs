using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public void TakeDmg(float dmg)
    {
        //particle effect/flash

        if (GetComponent<EnemyMelee>())
            GetComponent<EnemyMelee>().FlashHealthBar();

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
