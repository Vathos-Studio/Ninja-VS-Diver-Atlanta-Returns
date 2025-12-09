using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10;
    public Rigidbody2D rig;
    private float jumpForce;
    public LayerMask layerMask;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        jumpForce = speed *50;
    }

    // Update is called once per frame
    void Update()
    {
        rig.velocity = (transform.right * speed * Input.GetAxis("Horizontal")) + (transform.up * rig.velocity.y);
        if (Input.GetButtonDown("Jump"))
        {
            rig.AddForce(transform.up * jumpForce);
        }
        
        RaycastHit2D suelo = Physics2D.Raycast(transform.position, transform.forward, transform.localScale.y / 2, layerMask);
        if (suelo.collider != null)
        {
            Debug.Log("Golpeo: " + suelo.collider.name);
        }
        Debug.DrawRay(transform.position, transform.forward * (1f), Color.red);
        
    }
}
