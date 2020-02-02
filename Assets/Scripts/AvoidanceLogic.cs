using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AvoidanceLogic : MonoBehaviour
{
    public List<GameObject> projectileList = new List<GameObject>();
    public Vector3[] SpawnPositions;
    public GameObject avoidanceTurret;
    private AvoidanceTurret AT;
    private Room room;

    public bool sinMovement = false;
    // Start is called before the first frame update
    void Start()
    {
        room = FindObjectOfType<Room>();
        avoidanceTurret = Instantiate(avoidanceTurret, new Vector3(room.RoomFloor[room.RoomFloor.Count / 2].transform.position.x, 2, 50f), Quaternion.identity);
        AT = avoidanceTurret.GetComponent<AvoidanceTurret>();
        StartCoroutine(Fight());
    }

    // Update is called once per frame
    void Update()
    {
        if (sinMovement)
        {
            avoidanceTurret.transform.Rotate(Vector3.forward * Time.deltaTime * 50f);
            avoidanceTurret.transform.position = new Vector3(avoidanceTurret.transform.position.x + (Mathf.Sin(Time.deltaTime / 10f) * 20f), 2f, avoidanceTurret.transform.position.z);
        }
    }

    IEnumerator Fight()
    {
        yield return new WaitForSeconds(5f);
        AT.destination = new Vector3(room.RoomFloor[room.RoomFloor.Count / 2].transform.position.x, 2, room.RoomFloor[room.RoomFloor.Count / 2].transform.position.z);
        AT.moveSpeed = 0.005f;
        yield return new WaitForSeconds(3f);
        AT.destination = Vector3.zero;
        sinMovement = true;
        AT.isFiring = true;
        yield return new WaitForSeconds(8f);
        AT.isFiring = false;
        sinMovement = false;
        AT.destination = new Vector3(room.RoomFloor[room.RoomFloor.Count / 2].transform.position.x, 2, room.RoomFloor[room.RoomFloor.Count / 2].transform.position.z);
        yield return new WaitForSeconds(3f);
        AT.circleAttack = true;
        AT.rainAttack = true;
        yield return new WaitForSeconds(5f);
        AT.radius = 10f;
        yield return new WaitForSeconds(5f);
        AT.radius = 15f;
        yield return new WaitForSeconds(5f);
        AT.radius = 20f;
        yield return new WaitForSeconds(5f);
        AT.circleAttack = false;
        AT.rainAttack = false;
        yield return new WaitForSeconds(5f);
        AT.deathParticle.Play();
        AT.ogParticle.Stop();
        AT.departingParticle.Play();
        yield return new WaitForSeconds(3f);
        AT.deathParticle.Stop();
        AT.gameObject.SetActive(false);
    }
}
