using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float dmg;
    public bool isActive;
    private float speed;
    private Vector3 dir;


    public void Activate(float speed, Vector3 dir, float dmg)
    {
        this.speed = speed;
        this.dir = dir;

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
        Destroy(this);
    }

}
