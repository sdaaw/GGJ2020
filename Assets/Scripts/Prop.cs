using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{

    public float randomScale;
    // Start is called before the first frame update
    void Start()
    {
        randomScale = Random.Range(0.5f, 0.8f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
