using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public GameObject FloorPrefab;

    //if we want to save rooms
    //public List<List<GameObject>> WorldFloors = new List<List<GameObject>>();

    private List<GameObject> RoomFloor = new List<GameObject>();
    private List<GameObject> PropList = new List<GameObject>();

    public GameObject playerPrefab;
    public GameObject dangerPrefab;

    private GameObject player;
    

    public float RoomSize;

    public float elevationFactor = 0.2f;

    private bool destroyRoom = false;
    private bool spawnRoom = false;

    private Camera m_camera;
    private GameObject SpawnTile;


    void Start()
    {
        //to distribute the level without overlapping because of the size
        m_camera = FindObjectOfType<Camera>();
        RoomSize *= FloorPrefab.transform.localScale.x;
        SpawnRoom();
        GenerateRoom();
    }

    void GenerateRoom()
    {
        Renderer rend;
        for(float i = 0; i < RoomSize; i += FloorPrefab.transform.localScale.x)
        {
            for (float j = 0; j < RoomSize; j += FloorPrefab.transform.localScale.x)
            {
                GameObject a = Instantiate(FloorPrefab.gameObject, new Vector3(i, 15, j), Quaternion.identity);

                rend = a.GetComponent<Renderer>();
                rend.material.color = new Color(Random.Range(0.2f, 0.5f), 0, Random.Range(0.1f, 0.3f));
                RoomFloor.Add(a);
            }
        }
        //how many blockade blocks do we want   
        int blockTiles = 4;
        for(int i = 0; i < blockTiles; i++)
        {
            GetBlockTile();
        }
        int dmgTiles = 2;
        for(int i = 0; i < dmgTiles; i++)
        {
            GetDmgTile();
        }
        SpawnTile = RoomFloor[(RoomFloor.Count / 2)];
        Renderer spawnTileMat = SpawnTile.GetComponent<Renderer>();
        spawnTileMat.material.color = Color.blue;
        spawnRoom = true;

        m_camera.transform.position = new Vector3(SpawnTile.transform.position.x, 30, SpawnTile.transform.position.z - 8);

        player = Instantiate(playerPrefab, SpawnTile.transform.position + Vector3.up, Quaternion.identity);
    }


    void GetBlockTile()
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

    void GetDmgTile()
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
                    30, 
                    block.transform.position.z), block.GetComponent<Block>().exitSpeed);

                rend = block.GetComponent<Renderer>();
                rend.material.color = new Color(
                    Mathf.Lerp(rend.material.color.r, 0.05f, 0.005f),
                    Mathf.Lerp(rend.material.color.g, 0.4f, 0.007f),
                    Mathf.Lerp(rend.material.color.b, 0.01f, 0.002f)
                    );
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
    }


    IEnumerator CancelSpawnMovement()
    {
        yield return new WaitForSeconds(3f);
        spawnRoom = false;
    }
    IEnumerator SpawnRoom()
    {
        
        yield return new WaitForSeconds(3f);
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
