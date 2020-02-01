using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public bool isBlock;
    public bool isSpawn;
    public bool isDmg;

    public float exitSpeed;
    public float spawnSpeed;


    public void Start()
    {
        exitSpeed = Random.Range(0.008f, 0.015f);
        spawnSpeed = Random.Range(0.03f, 0.06f);
    }

}
