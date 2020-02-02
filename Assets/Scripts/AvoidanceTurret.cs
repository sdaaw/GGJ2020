using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceTurret : MonoBehaviour
{

    public bool isFiring = false;
    AvoidanceLogic AL;
    Renderer r;
    private float timer = 0;
    public ParticleSystem deathParticle;
    public ParticleSystem ogParticle;
    public ParticleSystem departingParticle;

    public List<GameObject> projectiles = new List<GameObject>();

    public Vector3 destination;
    public float moveSpeed = 0.05f;
    private float dir = 0;

    public bool circleAttack = false;
    public bool releaseCircleAttack = false;

    public bool rainAttack = false;
    private float rainTimer = 0;
    private float circleTimer = 0;

    public float radius = 5f;

    // Start is called before the first frame update
    void Start()
    {
        AL = FindObjectOfType<AvoidanceLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFiring)
        {
            if(timer > 0.2f)
            {
                GameObject a = Instantiate(AL.projectileList[Random.Range(0, AL.projectileList.Count)], transform.position, Quaternion.LookRotation(-Vector3.forward));
                a.GetComponent<AvoidanceProjectile>().move = true;
                timer = 0;
            }
            timer += 1 * Time.deltaTime;
        }

        if(destination != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, destination, moveSpeed);
        }


        if(circleAttack)
        {
            if(circleTimer > 0.05f)
            {
                GameObject a = Instantiate(AL.projectileList[Random.Range(0, AL.projectileList.Count)], transform.position, Quaternion.identity);
                a.transform.position = new Vector3(transform.position.x + (Mathf.Sin(Time.time / 2f) * radius), 2f, transform.position.z + (Mathf.Cos(Time.time / 2f) * radius));
                a.GetComponent<AvoidanceProjectile>().doNotMove = true;
                projectiles.Add(a);
                circleTimer = 0;
            }
            circleTimer += 1 * Time.deltaTime;
        }

        if(rainAttack)
        {
            if(rainTimer > 0.1f)
            {
                GameObject a = Instantiate(AL.projectileList[Random.Range(0, AL.projectileList.Count)], new Vector3(transform.position.x + Random.Range(-30, 30), 2, 50), Quaternion.LookRotation(-Vector3.forward));
                a.GetComponent<AvoidanceProjectile>().move = true;
                rainTimer = 0;
            }
            rainTimer += 1 * Time.deltaTime;
        }

    }
}
