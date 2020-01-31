using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public bool isBlock;
    public bool isSpawn;
    public float exitSpeed;


    public void Start()
    {
        exitSpeed = Random.Range(0.005f, 0.008f);
    }

}
