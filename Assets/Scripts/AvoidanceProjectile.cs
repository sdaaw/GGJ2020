using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceProjectile : MonoBehaviour
{


    public float speed;
    public float size;
    public bool isAimedAtPlayer;
    public bool move;
    private float timer = 0;
    public bool doNotMove = false;

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
        if(move || isAimedAtPlayer)
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

        timer += 1 * Time.deltaTime;
        if(timer > 20)
        {
            Destroy(gameObject);
        }
    }
}
