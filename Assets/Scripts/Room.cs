﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public GameObject FloorPrefab;

    //if we want to save rooms
    //public List<List<GameObject>> WorldFloors = new List<List<GameObject>>();

    public List<GameObject> RoomFloor = new List<GameObject>();
    public List<GameObject> PropList = new List<GameObject>();
    public List<GameObject> EnemyList = new List<GameObject>();

    public GameObject playerRangedPrefab;
    public GameObject playerMeleePrefab;

    private GameObject playerPrefab;

    public Color repairedColor;
    public Color corruptedColor;

    public float corruptedColorVariationR;
    public float corruptedColorVariationG;
    public float corruptedColorVariationB;

    public float colorVariationR;
    public float colorVariationG;
    public float colorVariationB;

    public GameObject dangerPrefab;
    public GameObject avoidanceManager;
    public GameObject enemyPrefab;
    public GameObject wallPrefab;
    public GameObject bushPrefab;

    private GameObject player;

    public float randPropScale;

    //TODO: Events
    public bool avoidanceEvent = false;
    public bool isRoomClear = false;


    public float RoomSize;

    public float elevationFactor = 1f;

    private bool destroyRoom = false;
    private bool spawnRoom = false;

    private Camera m_camera;
    private GameObject SpawnTile;


    void Start()
    {
        if (Random.Range(0, 100) > 50)
        {
            playerPrefab = playerMeleePrefab;
            print("melee");
        }
        else
        {
            playerPrefab = playerRangedPrefab;
            print("ranged");
        }
        randPropScale = Random.Range(0.2f, 0.5f);
        //to distribute the level without overlapping because of the size
        m_camera = FindObjectOfType<Camera>();
        RoomSize *= FloorPrefab.transform.localScale.x;
        avoidanceEvent = true;
        SpawnRoom();
        GenerateRoom();


    }

    void GenerateRoom()
    {
        Renderer rend;
        for(float i = 0; i < RoomSize; i += 4)
        {
            for (float j = 0; j < RoomSize; j += 4)
            {

                GameObject a = Instantiate(FloorPrefab.gameObject, new Vector3(i, 15, j), Quaternion.identity);

                if(Random.Range(0, 10) > 8) 
                {
                    Renderer rend2;
                    GameObject b = Instantiate(bushPrefab, new Vector3(
                        Random.Range(1, 45),
                        0.5f,
                        Random.Range(1, 45)), Quaternion.identity);

                    PropList.Add(b);
                    float randScale = Random.Range(0.6f, 1f);
                    b.transform.localScale = new Vector3(0, 0, 0);
                    rend2 = b.GetComponentInChildren<Renderer>();
                    rend2.material.color = new Color(Random.Range(0.2f, 0.5f), 0, Random.Range(0.1f, 0.3f));

                }

                if(Random.Range(0, 100) > 98)
                {
                    //powerup spawn

                }

                rend = a.GetComponent<Renderer>();
                rend.material.color = new Color(corruptedColor.r + Random.Range(-corruptedColorVariationR, corruptedColorVariationR), 
                    corruptedColor.g + Random.Range(-corruptedColorVariationG, corruptedColorVariationG), 
                    corruptedColor.b + Random.Range(-corruptedColorVariationB, corruptedColorVariationB));
                RoomFloor.Add(a);
            }
        }

        if (avoidanceEvent)
        {
            Instantiate(avoidanceManager, Vector3.zero, Quaternion.identity);

        } else
        {

            int blockTiles = 4; 
            for (int i = 0; i < blockTiles; i++)
            {
                SetBlockTiles();
            }
            StartCoroutine(SpawnEnemies());
        }
        SpawnTile = RoomFloor[(RoomFloor.Count / 2)];
        Renderer spawnTileMat = SpawnTile.GetComponent<Renderer>();
        spawnTileMat.material.color = Color.blue;
        spawnRoom = true;

        //m_camera.transform.position = new Vector3(SpawnTile.transform.position.x, 30, SpawnTile.transform.position.z - 8);

        
        player = Instantiate(playerPrefab, SpawnTile.transform.position + Vector3.up, Quaternion.identity);


        Camera.main.GetComponent<CameraFollow>().m_follow = player.transform;
    }

    void SetEnemies()
    {
        Enemy e;
        GameObject a = Instantiate(enemyPrefab, new Vector3(
            RoomFloor[Random.Range(0, RoomFloor.Count)].transform.position.x, 
            1,
            RoomFloor[Random.Range(0, RoomFloor.Count)].transform.position.z), Quaternion.identity);
        a.GetComponent<Enemy>().AllowMovement = true;
        e = a.GetComponent<Enemy>();
        if(Random.Range(1, 10) > 7)
        {
            e.isRanged = true;
            e.escapeDistance = Random.Range(5, 15);
            
        } else
        {
            e.isMelee = true;
        }
        EnemyList.Add(a);
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(0.1f); //a small delay so the player spawns xD
        player.GetComponent<PlayerController>().AllowMovement = false;
        yield return new WaitForSeconds(3f);
        int enemyCount = Random.Range(2, 5);
        for (int i = 0; i < enemyCount; i++)
        {
            SetEnemies();
        }
        player.GetComponent<PlayerController>().AllowMovement = true;
        yield return new WaitForSeconds(0.5f);
    }


    void SetBlockTiles()
    {
        Block BlockTile;
        Renderer rend;
        
        BlockTile = RoomFloor[Random.Range(0, RoomFloor.Count)].GetComponent<Block>();
        BlockTile.transform.localScale = new Vector3(
            BlockTile.gameObject.transform.localScale.x, 
            BlockTile.gameObject.transform.localScale.y * 5, 
            BlockTile.gameObject.transform.localScale.z);

        rend = BlockTile.gameObject.GetComponent<Renderer>();
        rend.material.color = Color.gray;

    }

    void SetDmgTiles()
    {
        Block DmgTile;
        Renderer rend;

        DmgTile = RoomFloor[Random.Range(0, RoomFloor.Count)].GetComponent<Block>();
        rend = DmgTile.gameObject.GetComponent<Renderer>();
        rend.material.color = Color.red;
        StartCoroutine(SpawnDmgProps(DmgTile.gameObject));

    }

    IEnumerator SpawnDmgProps(GameObject DmgTile)
    {
        GameObject a;
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < 10; i++)
        {
            a = Instantiate(dangerPrefab, new Vector3(DmgTile.transform.position.x + Random.Range(0, 1),
                DmgTile.transform.position.y,
                DmgTile.transform.position.z + Random.Range(0, 1)), Quaternion.identity);
            PropList.Add(a);
        }
    }


    void Update()
    {
        if(destroyRoom)
        {
            Renderer rend;
            spawnRoom = false;
            foreach (GameObject block in RoomFloor)
            {
                block.transform.position = Vector3.Lerp(block.transform.position, new Vector3(
                    block.transform.position.x,
                    40, 
                    block.transform.position.z), block.GetComponent<Block>().exitSpeed);

                /*rend = block.GetComponent<Renderer>();
                rend.material.color = new Color(
                    Mathf.Lerp(rend.material.color.r, 0.05f, 0.005f),
                    Mathf.Lerp(rend.material.color.g, 0.4f, 0.007f),
                    Mathf.Lerp(rend.material.color.b, 0.01f, 0.002f),
                    Mathf.Lerp(rend.material.color.a, 0f, 0.002f)
                    );*/
            }
        }

        if(spawnRoom)
        {
            foreach (GameObject block in RoomFloor)
            {
                block.transform.position = Vector3.Lerp(block.transform.position, new Vector3(
                    block.transform.position.x,
                    0 + Random.Range(-elevationFactor, elevationFactor),
                    block.transform.position.z), block.GetComponent<Block>().spawnSpeed);
            }
            StartCoroutine(CancelSpawnMovement());
        } else
        {
            foreach (GameObject prop in PropList)
            {

                prop.transform.localScale = Vector3.Slerp(prop.transform.localScale, new Vector3(
                    prop.GetComponent<Prop>().randomScale,
                    prop.GetComponent<Prop>().randomScale,
                    prop.GetComponent<Prop>().randomScale),
                    0.01f
                    );
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            if(EnemyList.Count == 0)
            {
                StartCoroutine(RepairWorld());
            }
            avoidanceEvent = true;
        }
    }

    public void SwitchRoom()
    {
        if (!destroyRoom)
        {
            //destroy props first
            foreach (GameObject p in PropList)
            {
                Destroy(p);
            }
            PropList.Clear();
            Destroy(player);
            StartCoroutine(SpawnRoom());
            destroyRoom = true;
        }
    }

    IEnumerator RepairWorld()
    {
        player.GetComponent<PlayerController>().AllowMovement = false;
        Renderer rend;
        foreach (GameObject block in RoomFloor)
        {
            rend = block.GetComponent<Renderer>();
            rend.material.color = new Color(
                repairedColor.r + Random.Range(-colorVariationR, colorVariationR), 
                repairedColor.g + Random.Range(-colorVariationG, colorVariationG),
                repairedColor.b + Random.Range(-colorVariationB, colorVariationB));

            yield return new WaitForSeconds(0.001f);
        }

        foreach (GameObject prop in PropList)
        {
            rend = prop.GetComponentInChildren<Renderer>();
            rend.material.color = new Color(
                repairedColor.r + Random.Range(-colorVariationR, colorVariationR),
                repairedColor.g + Random.Range(-colorVariationG, colorVariationG),
                repairedColor.b + Random.Range(-colorVariationB, colorVariationB));

            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.5f);
        SwitchRoom(); //use this to switch room
    }


    IEnumerator CancelSpawnMovement()
    {
        //this is to stop the chunks/tiles/blocks from wobbling because of lerp
        yield return new WaitForSeconds(3f);
        spawnRoom = false;
    }
    IEnumerator SpawnRoom()
    {
        
        yield return new WaitForSeconds(2f);
        //if we want to save rooms
        //WorldFloors.Add(RoomFloor);

        //DESTROY EVERYTHING HERE
        foreach(GameObject r in RoomFloor)
        {
            Destroy(r);
        }
        RoomFloor.Clear();
        destroyRoom = false;
        GenerateRoom();

    }
}
