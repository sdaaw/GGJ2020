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

    public Texture corruptedTexture;

    public Vector3 finalDestination;

    public void Start()
    {
        if(isBlock)
        {
            //change texture pls
            Renderer r;
            r = gameObject.GetComponent<Renderer>();
        }
        exitSpeed = Random.Range(0.03f, 0.05f);
        spawnSpeed = Random.Range(0.03f, 0.05f);
    }

}
