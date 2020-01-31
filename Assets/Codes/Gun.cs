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
            GameObject bullet = Instantiate(m_bulletPrefab, gunHolder.position + gunHolder.forward, Quaternion.identity);
            bullet.GetComponent<Bullet>().Activate(bulletVelocity, gunHolder.forward, 0);
        }
    }
}
