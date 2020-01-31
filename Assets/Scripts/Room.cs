using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public GameObject FloorPrefab;
    public Vector3 exitPosition;


    private List<GameObject> RoomFloor = new List<GameObject>();


    public float RoomSize;

    public float elevationFactor = 0.15f;

    private int roomDifficulty;

    private bool switchingRoom = false;


    void Start()
    {
        //to distribute the level without overlapping because of the size
        RoomSize *= FloorPrefab.transform.localScale.x;
        //SpawnRoom();
        GenerateRoom();
    }

    void GenerateRoom()
    {
        for(float i = 0; i < RoomSize; i += FloorPrefab.transform.localScale.x)
        {
            //GameObject a = Instantiate(FloorPrefab.gameObject, new Vector3(i, Random.Range(-elevationFactor, elevationFactor), 0), Quaternion.identity);
            //RoomFloor.Add(a);
            for (float j = 0; j < RoomSize; j += FloorPrefab.transform.localScale.x)
            {
                GameObject b = Instantiate(FloorPrefab.gameObject, new Vector3(i, Random.Range(-elevationFactor, elevationFactor), j), Quaternion.identity);
                RoomFloor.Add(b);
            }
        }
        //how many blocks do we want   
        int blockTiles = 4;
        for(int i = 0; i < blockTiles; i++)
        {
            GetBlockTile();
        }
        GameObject SpawnTile = RoomFloor[(RoomFloor.Count / 2)];
        Renderer spawnTileMat = SpawnTile.GetComponent<Renderer>();
        spawnTileMat.material.color = Color.blue;
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
            BlockTile.gameObject.transform.localScale.y * 2, 
            BlockTile.gameObject.transform.localScale.z);

        rend = BlockTile.gameObject.GetComponent<Renderer>();
        rend.material.color = Color.gray;

    }

    void GenerateNewRoom()
    {
        switchingRoom = true;
        //StartCoroutine(SpawnRoom());
    }

    void Update()
    {
        if(switchingRoom)
        {
            
            foreach (GameObject block in RoomFloor)
            {
                block.transform.position = Vector3.Lerp(block.transform.position, new Vector3(
                    block.transform.position.x,
                    15, 
                    block.transform.position.z), block.GetComponent<Block>().exitSpeed);
            }
        }

        if(Input.GetKey(KeyCode.Space))
        {
            GenerateNewRoom();
        }
    }

    IEnumerator SpawnRoom()
    {
        yield return new WaitForSeconds(1f);
        GenerateRoom();
    }
}
