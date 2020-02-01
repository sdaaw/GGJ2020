using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private GameObject m_bulletPrefab;

    public float bulletVelocity;
    public bool canShoot;

    public float shootDelay;
    private float shootTimer;

    public float damage;

    public int upgradeLvl;

    private void Update()
    {
        if (shootTimer < shootDelay)
            shootTimer += Time.deltaTime;

        if (shootTimer >= shootDelay)
        {
            shootTimer = shootDelay;
            canShoot = true;
        }            
    }

    public void Shoot(Transform gunHolder)
    {
        if(canShoot)
        {
            canShoot = false;
            shootTimer = 0;

            if(upgradeLvl == 0)
            {
                GameObject bullet = Instantiate(m_bulletPrefab, gunHolder.position + gunHolder.forward, Quaternion.identity);
                bullet.GetComponent<Bullet>().Activate(bulletVelocity, gunHolder.forward, gunHolder, damage);
            }
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
}
