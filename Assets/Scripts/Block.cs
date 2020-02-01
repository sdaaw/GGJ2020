using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public bool isBlock;
    public bool isSpawn;
    public float exitSpeed;
    public float spawnSpeed;


    public void Start()
    {
        exitSpeed = Random.Range(0.005f, 0.008f);
        spawnSpeed = Random.Range(0.01f, 0.03f);
    }

}
