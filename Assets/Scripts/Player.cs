using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10;
    public Rigidbody2D rig;
    private float jumpForce;
    public LayerMask layerMask;
    private Animator animator;

    private bool esSuelo = false;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        jumpForce = speed *50;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rig.velocity = (transform.right * speed * Input.GetAxis("Horizontal")) + (transform.up * rig.velocity.y);
        if (Input.GetButtonDown("Jump"))
        {
            Salta();
        }
        
        animator.SetFloat("VelocityY", rig.velocity.y);
        animator.SetFloat("VelocityX", Mathf.Abs(rig.velocity.x));
        animator.SetBool("SueloToca", TocaSuelo());

        if (rig.velocity.x > 0.1f)
        {
            GetComponent<SpriteRenderer>().flipX = false;

        }
        else if (rig.velocity.x < -0.1f)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private bool TocaSuelo()
    {
        

        RaycastHit2D suelo = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y + 0.1f, layerMask);
        Debug.DrawRay(transform.position, Vector2.down, Color.red);
        if (suelo.collider != null)
        {
            Debug.Log("Golpeo: " + suelo.collider.name);
            return  true;
        } else
        {
            return  false;
        }
            
        
    }

    private void Salta()
    {
        if (TocaSuelo()) { 
            rig.AddForce(transform.up * jumpForce);
            
        }
    }
}
