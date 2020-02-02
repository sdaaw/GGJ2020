using System.Collections;
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
    private List<GameObject> niceTrees = new List<GameObject>();

    public GameObject playerRangedPrefab;
    public GameObject playerMeleePrefab;
    public GameObject playerVurtnePrefab;

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
    public GameObject corruptedTreePrefab;
    public GameObject treePrefab;
    public GameObject shroomEnemy;

    public List<GameObject> pickupPrefabList = new List<GameObject>();
    private List<GameObject> spawnedPickupPrefabList = new List<GameObject>();

    private GameObject player;

    public float randPropScale;

    //TODO: Events
    public bool avoidanceEvent = false;
    public bool isRoomClear = false;


    public Texture corruptedBlockTexture;

    public bool isSpawningThings;


    public float RoomSize;

    public int levelsToReachBoss;

    public int levelsCompleted = 0;

    public float elevationFactor = 1f;

    private bool destroyRoom = false;
    private bool spawnRoom = false;

    private Camera m_camera;
    private GameObject SpawnTile;


    void Start()
    {

        int plrId = Random.Range(0, 3);

        if (plrId == 0)
        {
            playerPrefab = playerMeleePrefab;
            print("melee");
        } else if(plrId == 1)
        {
            playerPrefab = playerRangedPrefab;
            print("ranged");
        } else if(plrId == 2)
        {
            playerPrefab = playerVurtnePrefab;
        }
        randPropScale = Random.Range(0.2f, 0.5f);
        //to distribute the level without overlapping because of the size
        m_camera = FindObjectOfType<Camera>();
        RoomSize *= FloorPrefab.transform.localScale.x;
        //avoidanceEvent = true;
        SpawnRoom();
        GenerateRoom();


    }

    void GenerateRoom()
    {
        isSpawningThings = true;
        Renderer rend;
        for(float i = 0; i < RoomSize; i += 4)
        {
            for (float j = 0; j < RoomSize; j += 4)
            {

                GameObject a = Instantiate(FloorPrefab.gameObject, new Vector3(i, 15, j), Quaternion.identity);

                if(Random.Range(0, 50) > 45) 
                {
                    Renderer rend2;
                    GameObject b = Instantiate(corruptedTreePrefab, new Vector3(
                        Random.Range(1, 45),
                        0.5f,
                        Random.Range(1, 45)), Quaternion.identity);

                    PropList.Add(b);
                    float randScale = Random.Range(0.6f, 1f);
                    b.transform.localScale = new Vector3(0, 0, 0);
                    rend2 = b.GetComponentInChildren<Renderer>();
                    rend2.material.color = new Color(Random.Range(0.2f, 0.5f), 0, Random.Range(0.1f, 0.3f));

                }
                if (Random.Range(0, 50) > 40)
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

                rend = a.GetComponent<Renderer>();
                rend.material.color = new Color(corruptedColor.r + Random.Range(-corruptedColorVariationR, corruptedColorVariationR), 
                    corruptedColor.g + Random.Range(-corruptedColorVariationG, corruptedColorVariationG), 
                    corruptedColor.b + Random.Range(-corruptedColorVariationB, corruptedColorVariationB));
                RoomFloor.Add(a);
            }
        }

        if (levelsCompleted == levelsToReachBoss)
        {
            Instantiate(avoidanceManager, Vector3.zero, Quaternion.identity);
        }
        else
        {
            int blockTiles = 15; 
            for (int i = 0; i < blockTiles; i++)
            {
                SetBlockTiles();
            }

            int pickups = Random.Range(0, 3);

            for (int j = 0; j < pickups; j++)
            {
                int rBlock = Random.Range(0, RoomFloor.Count);

                if (!RoomFloor[rBlock].GetComponent<Block>().isBlock)
                {
                    int pIndex = 0;
                    int r = Random.Range(0, 100);

                    if (r >= 0 && r <= 30)
                        pIndex = 0;
                    else if (r > 30 && r <= 60)
                        pIndex = 1;
                    else if (r > 60 && r <= 75)
                        pIndex = 2;
                    else if (r > 75 && r <= 85)
                        pIndex = 3;
                    else if (r > 85 && r <= 100)
                        pIndex = 4;

                    //pickups here
                    GameObject prop = Instantiate(pickupPrefabList[pIndex], new Vector3( RoomFloor[rBlock].transform.position.x
                                                                                         ,2
                                                                                         ,RoomFloor[rBlock].transform.position.z)
                        , Quaternion.identity);
                    spawnedPickupPrefabList.Add(prop);
                }
            }
               
            StartCoroutine(SpawnEnemies());
        }
        SpawnTile = RoomFloor[(RoomFloor.Count / 2)];
        Renderer spawnTileMat = SpawnTile.GetComponent<Renderer>();
        spawnTileMat.material.color = Color.blue;
        spawnRoom = true;

        //m_camera.transform.position = new Vector3(SpawnTile.transform.position.x, 30, SpawnTile.transform.position.z - 8);

        if(player == null)
            player = Instantiate(playerPrefab, SpawnTile.transform.position + Vector3.up, Quaternion.identity);
        else
        {
            player.gameObject.SetActive(true);
            player.transform.position = SpawnTile.transform.position + Vector3.up;
            player.GetComponent<PlayerController>().AllowMovement = true;
        }
           


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
            a = Instantiate(shroomEnemy, new Vector3(
            RoomFloor[Random.Range(0, RoomFloor.Count)].transform.position.x,
            1,
            RoomFloor[Random.Range(0, RoomFloor.Count)].transform.position.z), Quaternion.identity);
        }
        EnemyList.Add(a); //wow
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(0.3f); //a small delay so the player spawns xD
        player.GetComponent<PlayerController>().AllowMovement = false;
        yield return new WaitForSeconds(3f);
        int enemyCount = Random.Range(2, 5);
        enemyCount += levelsCompleted/2;

        for (int i = 0; i < enemyCount; i++)
        {
            SetEnemies();
        }
        player.GetComponent<PlayerController>().AllowMovement = true;
        yield return new WaitForSeconds(0.5f);
        isSpawningThings = false;
    }


    void SetBlockTiles()
    {
        Block BlockTile;
        Renderer rend;
        
        BlockTile = RoomFloor[Random.Range(0, RoomFloor.Count)].GetComponent<Block>();
        BlockTile.transform.localScale = new Vector3(
            BlockTile.gameObject.transform.localScale.x, 
            BlockTile.gameObject.transform.localScale.y + Random.Range(2.25f, 3f), 
            BlockTile.gameObject.transform.localScale.z);

        rend = BlockTile.gameObject.GetComponent<Renderer>();
        rend.material.color = Color.gray;

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

        if(EnemyList.Count == 0 && !isSpawningThings)
            player.GetComponent<PlayerController>().SetCleanseText(true);
        else
            player.GetComponent<PlayerController>().SetCleanseText(false);

        if (Input.GetKeyUp(KeyCode.Space) && !isSpawningThings)
        {
            isSpawningThings = true;
            if (EnemyList.Count == 0)
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
            foreach(GameObject nt in niceTrees)
            {
                Destroy(nt);
            }

            /*for(int i = spawnedPickupPrefabList.Count-1; i > 0; i++)
            {
                if (spawnedPickupPrefabList[i] != null)
                    Destroy(spawnedPickupPrefabList[i]);
            }*/
            foreach (GameObject spp in spawnedPickupPrefabList)
                Destroy(spp);
            spawnedPickupPrefabList.Clear();

            niceTrees.Clear();
            PropList.Clear();
            //Destroy(player);
            player.gameObject.SetActive(false);
            StartCoroutine(SpawnRoom());
            destroyRoom = true;
        }
    }

    IEnumerator RepairWorld()
    {
        levelsCompleted++;
        player.GetComponent<PlayerController>().UpdateRoomText("Rooms cleared " + levelsCompleted + "/" + levelsToReachBoss);
        player.GetComponent<PlayerController>().score += 1000 * levelsCompleted;
        player.GetComponent<PlayerController>().AllowMovement = false;
        Renderer rend;
        MeshRenderer mRend;
        foreach (GameObject block in RoomFloor)
        {
            mRend = block.GetComponent<MeshRenderer>();


            mRend.sharedMaterial.SetTexture("_MainTex", corruptedBlockTexture);
            /*rend.material.color = new Color(
                repairedColor.r + Random.Range(-colorVariationR, colorVariationR), 
                repairedColor.g + Random.Range(-colorVariationG, colorVariationG),
                repairedColor.b + Random.Range(-colorVariationB, colorVariationB));*/

            


            yield return new WaitForSeconds(0.001f);
        }

        foreach (GameObject prop in PropList)
        {
            if(prop.GetComponent<Prop>().isCorruptedTree)
            {
                GameObject a = Instantiate(treePrefab, prop.transform.position, Quaternion.identity); //switcharoo
                prop.SetActive(false);
                niceTrees.Add(a);
                yield return new WaitForSeconds(0.01f);
            } else
            {
                rend = prop.GetComponentInChildren<Renderer>();
                rend.material.color = new Color(
                    repairedColor.r + Random.Range(-colorVariationR, colorVariationR),
                    repairedColor.g + Random.Range(-colorVariationG, colorVariationG),
                    repairedColor.b + Random.Range(-colorVariationB, colorVariationB));

                yield return new WaitForSeconds(0.01f);
            }

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
        foreach (GameObject r in RoomFloor)
        {
            Destroy(r);
        }
        RoomFloor.Clear();
        destroyRoom = false;
        GenerateRoom();

    }
}
