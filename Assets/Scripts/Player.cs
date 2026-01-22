using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class Player : MonoBehaviour
{
    public float speed = 10;
    public Rigidbody2D rig;
    private float jumpForce;
    public LayerMask layerMask;
    private Animator animator;


    
    void Start()
    {
        if (GetComponent<PhotonView>().IsMine) { 
            rig = GetComponent<Rigidbody2D>();
        
            animator = GetComponent<Animator>();

            Camera.main.transform.SetParent(transform);
            Camera.main.transform.position = transform.position + (Vector3.up) + transform.forward * -10;
        }

        jumpForce = speed * 50;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            rig.velocity = (transform.right * speed * Input.GetAxis("Horizontal")) + (transform.up * rig.velocity.y);
            if (Input.GetButtonDown("Jump"))
            {
                Salta();
            }

            animator.SetFloat("VelocityY", rig.velocity.y);
            animator.SetFloat("VelocityX", Mathf.Abs(rig.velocity.x));
            animator.SetBool("SueloToca", TocaSuelo());

            if (rig.velocity.x > 0.1f )
            {
                GetComponent<PhotonView>().RPC("RotateSprite", RpcTarget.All, true);

            }
            else if (rig.velocity.x < -0.1f )
            {
                GetComponent<PhotonView>().RPC("RotateSprite", RpcTarget.All, false);
            }
        }
    }
    [PunRPC]
    private void RotateSprite(bool rotate)
    {
        GetComponent<SpriteRenderer>().flipX = rotate;
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
