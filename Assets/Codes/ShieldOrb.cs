using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldOrb : MonoBehaviour
{
    public Transform owner;
    public float speed;
    private float angle;
    public float damage;

    private void Update()
    {
        if(owner != null)
        {
            angle += speed * Time.deltaTime;

            float xPosition = Mathf.Sin(angle) * 2;
            float zPosition = Mathf.Cos(angle) * 2;

            transform.position = new Vector3(xPosition + owner.transform.position.x, 1, zPosition + owner.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //its not our player (cant shoot itself)
        if (other.transform != owner)
        {
            if (other.gameObject.layer != 8)
            {
                if (other.GetComponent<Stats>())
                {
                    other.GetComponent<Stats>().TakeDmg(damage);
                    //play particle
                    Destroy(gameObject);
                }
            }
        }
    }
}
