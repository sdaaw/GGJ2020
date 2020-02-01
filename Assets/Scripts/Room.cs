using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public GameObject FloorPrefab;


    public List<List<GameObject>> WorldFloors = new List<List<GameObject>>();

    private List<GameObject> RoomFloor = new List<GameObject>();

    public GameObject playerPrefab;

    private GameObject player;


    public float RoomSize;

    public float elevationFactor = 0.15f;

    private int roomDifficulty;

    private bool destroyRoom = false;
    private bool spawnRoom = false;

    private Camera camera;
    private GameObject SpawnTile;


    void Start()
    {
        //to distribute the level without overlapping because of the size
        camera = FindObjectOfType<Camera>();
        RoomSize *= FloorPrefab.transform.localScale.x;
        SpawnRoom();
        GenerateRoom();
    }

    void GenerateRoom()
    {
        for(float i = 0; i < RoomSize; i += FloorPrefab.transform.localScale.x)
        {
            for (float j = 0; j < RoomSize; j += FloorPrefab.transform.localScale.x)
            {
                GameObject a = Instantiate(FloorPrefab.gameObject, new Vector3(i, 15, j), Quaternion.identity);
                RoomFloor.Add(a);
            }
        }
        //how many blockade blocks do we want   
        int blockTiles = 4;
        for(int i = 0; i < blockTiles; i++)
        {
            GetBlockTile();
        }
        SpawnTile = RoomFloor[(RoomFloor.Count / 2)];
        Renderer spawnTileMat = SpawnTile.GetComponent<Renderer>();
        spawnTileMat.material.color = Color.blue;
        spawnRoom = true;

        camera.transform.position = new Vector3(SpawnTile.transform.position.x, 14, SpawnTile.transform.position.z - 5);

        player = Instantiate(playerPrefab, SpawnTile.transform.position + Vector3.up, Quaternion.identity);
    }


    void GetBlockTile()
    {
        Block SpawnTile = RoomFloor[RoomFloor.Count / 2].GetComponent<Block>();
        SpawnTile.isSpawn = true;
        Block BlockTile;
        Renderer rend;
        
        BlockTile = RoomFloor[Random.Range(0, RoomFloor.Count)].GetComponent<Block>();
        BlockTile.transform.localScale = new Vector3(
            BlockTile.gameObject.transform.localScale.x, 
            BlockTile.gameObject.transform.localScale.y * 10, 
            BlockTile.gameObject.transform.localScale.z);

        rend = BlockTile.gameObject.GetComponent<Renderer>();
        rend.material.color = Color.gray;

    }


    void Update()
    {
        if(destroyRoom)
        {
            spawnRoom = false;
            foreach (GameObject block in RoomFloor)
            {
                block.transform.position = Vector3.Lerp(block.transform.position, new Vector3(
                    block.transform.position.x,
                    30, 
                    block.transform.position.z), block.GetComponent<Block>().exitSpeed);
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
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            if(!destroyRoom)
            {
                Destroy(player);
                StartCoroutine(SpawnRoom());
                destroyRoom = true;
            }
        }
    }


    IEnumerator CancelSpawnMovement()
    {
        yield return new WaitForSeconds(2f);
        spawnRoom = false;
    }
    IEnumerator SpawnRoom()
    {
        
        yield return new WaitForSeconds(3f);
        WorldFloors.Add(RoomFloor);
        RoomFloor.Clear();
        destroyRoom = false;
        GenerateRoom();

    }
}
