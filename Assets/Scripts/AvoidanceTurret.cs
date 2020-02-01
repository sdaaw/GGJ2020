using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceTurret : MonoBehaviour
{

    public bool isFiring = false;
    AvoidanceLogic AL;
    Renderer r;

    // Start is called before the first frame update
    void Start()
    {
        AL = FindObjectOfType<AvoidanceLogic>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isFiring)
        {
            GameObject a = Instantiate(AL.projectileList[Random.Range(0, AL.projectileList.Count)], transform.position, Quaternion.identity);
            //a.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            r = a.GetComponent<Renderer>();
            r.material.color = new Color(Random.Range(0.2f, 0.5f), 0, Random.Range(0.1f, 0.3f));
            a.GetComponent<AvoidanceProjectile>().isAimedAtPlayer = true;
        }

    }
}
