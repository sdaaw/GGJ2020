using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField]
    private GameObject m_bulletPrefab;

    public float bulletVelocity;
    public bool canShoot;

    public float shootDelay;
    private float shootTimer;

    public int upgradeLvl;

    public float shoot2Delay;
    private float shoot2Timer;
    public float aoeShotDelay;
    public bool canShoot2;

    public float GetShoot2Timer
    {
        get
        {
            return shoot2Timer;
        }
           
    }

    private void Start()
    {
        //player can shoot instantly, enemies cant
        if(GetComponent<PlayerController>())
        {
            shootTimer = shootDelay;
            shoot2Timer = shoot2Delay;
        }
    }

    private void Update()
    {
        if (shootTimer < shootDelay)
            shootTimer += Time.deltaTime;

        if (shootTimer >= shootDelay)
        {
            shootTimer = shootDelay;
            canShoot = true;
        }

        if (shoot2Timer < shoot2Delay)
            shoot2Timer += Time.deltaTime;

        if (shoot2Timer >= shoot2Delay)
        {
            shoot2Timer = shoot2Delay;
            canShoot2 = true;
        }
    }

    public void Shoot(Transform gunHolder)
    {
        if(canShoot)
        {
            canShoot = false;
            shootTimer = 0;

            if (GetComponent<PlayerController>())
            {
                if (!GetComponent<PlayerController>().isWizard)
                    SoundManager.PlayASource("PlayerShoot");
                else
                    SoundManager.PlayASource("Spell");
            }

            if (upgradeLvl == 0)
            {
                GameObject bullet = Instantiate(m_bulletPrefab, gunHolder.position + gunHolder.forward, Quaternion.identity);
                bullet.GetComponent<Bullet>().Activate(bulletVelocity, gunHolder.forward, gunHolder, damage);
            }
            //triple bullet (multishot)
            else if(upgradeLvl == 1)
            {
                GameObject bullet = Instantiate(m_bulletPrefab, gunHolder.position + gunHolder.forward, Quaternion.identity);
                bullet.GetComponent<Bullet>().Activate(bulletVelocity, gunHolder.forward, gunHolder, damage);

                GameObject bullet1 = Instantiate(m_bulletPrefab, gunHolder.position + gunHolder.forward, Quaternion.identity);
                Transform shootDir = gunHolder;
                shootDir.transform.Rotate(new Vector3(0, 30, 0));
                bullet1.GetComponent<Bullet>().Activate(bulletVelocity, shootDir.forward, gunHolder, damage);

                GameObject bullet2 = Instantiate(m_bulletPrefab, gunHolder.position + gunHolder.forward, Quaternion.identity);
                Transform shootDir1 = gunHolder;
                shootDir1.transform.Rotate(new Vector3(0, 300, 0));
                bullet2.GetComponent<Bullet>().Activate(bulletVelocity, shootDir1.forward, gunHolder, damage);
            }  
        }
    }

    public void Shoot2(Transform gunHolder)
    {
        if (canShoot2)
        {
            canShoot2 = false;
            shoot2Timer = 0;
            StartCoroutine(DelayAoeShot(gunHolder));
        }
    }

    private IEnumerator DelayAoeShot(Transform gunHolder)
    {
        yield return new WaitForSeconds(aoeShotDelay);

        int bulletSpray = 15;

        for (int i = 0; i < bulletSpray; i++)
        {
            //yield return new WaitForSeconds(0.1f);
            SoundManager.PlayASource("Spell2");

            GameObject bullet = Instantiate(m_bulletPrefab, gunHolder.position + gunHolder.forward, Quaternion.identity);
            bullet.transform.Rotate(new Vector3(0, 25 * i, 0));
            bullet.GetComponent<Bullet>().Activate(bulletVelocity, gunHolder.forward, gunHolder, damage);
        }
    }
}
