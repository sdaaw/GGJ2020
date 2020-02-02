using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public Animator m_anim;
    public GameObject start;
    public GameObject controls;

    private bool hasPlayedStartAnim = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        start.SetActive(true);
        controls.SetActive(false);
        hasPlayedStartAnim = true;
    }

    public void StartGame()
    {
        
        Application.LoadLevel(2);
    }

    public void DisplayStartScreen()
    {
        if (hasPlayedStartAnim)
            m_anim.speed = 100;
        start.SetActive(true);
        controls.SetActive(false);
    }

    public void DisplayControls()
    {
        start.SetActive(false);
        controls.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        // atte haista vittu tää on iha hyvä xD
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Animator[] anims = FindObjectsOfType<Animator>();
            foreach(Animator a in anims)
            {
                if (a.name != "MainCharacter")
                    a.speed = 100f;
            }
        }
    }
}
