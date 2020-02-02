using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swapSceneTimer : MonoBehaviour
{
    public float timeToSwap;
    private float m_timer;

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer > timeToSwap)
            Application.LoadLevel(1);
        if(Input.GetKeyDown(KeyCode.Space))
            Application.LoadLevel(1);
    }
}