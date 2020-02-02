using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    private Room rm;
    public float health;
    public float maxHealth;

    public bool isDead;

    public bool hasDeadAnim;

    private void Start()
    {
        rm = FindObjectOfType<Room>();
    }

    public void TakeDmg(float dmg)
    {
        //particle effect/flash
        if (isDead)
            return;

        health -= dmg;
        if (health <= 0 && !isDead)
            Dead();

        if (GetComponent<Enemy>())
            GetComponent<Enemy>().FlashHealthBar();
        else if (GetComponent<PlayerController>())
            GetComponent<PlayerController>().UpdateHealthImage();
    }

    public void Dead()
    {
        isDead = true;

        if (rm != null)
            rm.EnemyList.Remove(gameObject);

        if (!GetComponent<PlayerController>())
            StartCoroutine(WaitDestroy());
        else
        {
            GetComponent<PlayerController>().Dead();
        }
    }

    IEnumerator WaitDestroy()
    {
        if(GetComponentInChildren<Animator>())
        {
            if(hasDeadAnim)
                GetComponentInChildren<Animator>().SetTrigger("Death");
            else
            {
                //TODO: spawn dead explosion
            }
        }
           

        if (GetComponent<Enemy>())
            GetComponent<Enemy>().AllowMovement = false;

        if (GetComponent<Gun>())
            GetComponent<Gun>().enabled = false;

        if (GetComponentInChildren<Sword>())
            GetComponentInChildren<Sword>().enabled = false;

        if(hasDeadAnim)
            yield return new WaitForSeconds(4);
        else
            yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}
