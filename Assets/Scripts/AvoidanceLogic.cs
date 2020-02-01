using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceLogic : MonoBehaviour
{

    public List<GameObject> projectileList = new List<GameObject>();
    public Vector3[] SpawnPositions;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Intro());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Intro()
    {
        Renderer r;
        yield return new WaitForSeconds(4f);
        for(int i = 0; i < 100; i++)
        {
            GameObject a = Instantiate(projectileList[Random.Range(0, projectileList.Count)], SpawnPositions[Random.Range(0, SpawnPositions.Length)], Quaternion.identity);
            //a.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            r = a.GetComponent<Renderer>();
            r.material.color = new Color(Random.Range(0.2f, 0.5f), 0, Random.Range(0.1f, 0.3f));
            a.GetComponent<AvoidanceProjectile>().isAimedAtPlayer = true;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
