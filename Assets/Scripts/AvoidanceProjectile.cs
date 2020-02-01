using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceProjectile : MonoBehaviour
{


    public float speed;
    public float size;
    public bool isAimedAtPlayer;

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        if(isAimedAtPlayer)
        {
            transform.LookAt(player.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
