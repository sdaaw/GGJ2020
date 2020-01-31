using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool AllowMovement;
    public float speed;

    private Rigidbody m_rigidbody;
    private Vector3 m_moveVector;
    private Vector3 m_xMoveVector;
    private Transform m_transform;

    [SerializeField]
    private Camera m_playerCamera;

    [SerializeField]
    private LayerMask m_layerMask;

    public float turnSpeed;

    public Animator animator;

    public List<AudioClip> clips = new List<AudioClip>();
    public AudioSource bgMusic;

    public void SetMusic(bool wierdMusic)
    {
        if (bgMusic == null)
            return;

        if (!wierdMusic && bgMusic.clip != clips[0])
            bgMusic.clip = clips[0];
        else if (wierdMusic)
            bgMusic.clip = clips[1];

        if (!bgMusic.isPlaying)
            bgMusic.Play();
    }

    void Awake()
    {
        m_transform = transform;
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (AllowMovement)
            DoMovement();
        if (animator != null)
        {
            animator.SetFloat("speed", m_rigidbody.velocity.magnitude);
            //Debug.Log(m_rigidbody.velocity.magnitude);
        }
    }


    /// <summary>
    /// Do player movement
    /// </summary>
    void DoMovement()
    {
        m_rigidbody.velocity = Vector3.zero;
        m_moveVector = m_xMoveVector;

        m_xMoveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        m_xMoveVector = GetVectorRelativeToObject(m_xMoveVector, m_playerCamera.transform);

        if (m_xMoveVector.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(m_xMoveVector, Vector3.up);
            m_transform.rotation = Quaternion.Slerp(m_transform.rotation, targetRotation, 0.2f);
        }

        m_rigidbody.velocity = m_moveVector * speed;
    }

    public static Vector3 GetVectorRelativeToObject(Vector3 inputVector, Transform camera)
    {
        Vector3 objectRelativeVector = Vector3.zero;
        if (inputVector != Vector3.zero)
        {
            Vector3 forward = camera.TransformDirection(Vector3.forward);
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

            Vector3 relativeRight = inputVector.x * right;
            Vector3 relativeForward = inputVector.z * forward;

            objectRelativeVector = relativeRight + relativeForward;

            if (objectRelativeVector.magnitude > 1f) objectRelativeVector.Normalize();
        }
        return objectRelativeVector;
    }
}