using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed = 10;
    public Rigidbody2D rig;
    private float jumpForce;
    public LayerMask layerMask;
    private Animator animator;
    public int points;

    private Vector2 posicioninicial;
    public string NickName;

    private bool die = false;

    // ===== NETWORK =====
    private Vector3 networkPosition;
    private Vector2 networkVelocity;
    private bool networkFlipX;

    private PhotonView pv;
    private SpriteRenderer sprite;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        sprite = GetComponent<SpriteRenderer>();

        if (pv.IsMine)
        {
            rig = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            Camera.main.transform.SetParent(transform);
            Camera.main.transform.position = transform.position + (Vector3.up) + transform.forward * -10;

            posicioninicial = transform.position;
        }

        jumpForce = speed * 50;

        Debug.Log("Mi nickname es: " + NickName);

        networkPosition = transform.position;
    }

    void Update()
    {
        if (pv.IsMine)
        {
            if (die) return;

            // ===== TU MOVIMIENTO (igual) =====
            rig.velocity = (transform.right * speed * Input.GetAxis("Horizontal")) + (transform.up * rig.velocity.y);

            if (Input.GetButtonDown("Jump"))
            {
                Salta();
            }

            // ===== ANIMACIONES (igual) =====
            animator.SetFloat("VelocityY", rig.velocity.y);
            animator.SetFloat("VelocityX", Mathf.Abs(rig.velocity.x));
            animator.SetBool("SueloToca", TocaSuelo());

            // ===== FLIP (SIN RPC SPAM) =====
            if (rig.velocity.x > 0.1f)
            {
                sprite.flipX = true;   // derecha  flip
            }
            else if (rig.velocity.x < -0.1f)
            {
                sprite.flipX = false;  // izquierda  normal
            }
        }
        else
        {
            // ===== REMOTO (SUAVIZADO) =====
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 12f);

            rig.velocity = networkVelocity;
            sprite.flipX = networkFlipX;

            // animaciones también en remoto
            if (animator != null)
            {
                animator.SetFloat("VelocityY", rig.velocity.y);
                animator.SetFloat("VelocityX", Mathf.Abs(rig.velocity.x));
                animator.SetBool("SueloToca", TocaSuelo());
            }
        }
    }

    // =========================
    // SYNC PHOTON
    // =========================
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(rig.velocity);
            stream.SendNext(sprite.flipX);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkVelocity = (Vector2)stream.ReceiveNext();
            networkFlipX = (bool)stream.ReceiveNext();
        }
    }

    private bool TocaSuelo()
    {
        RaycastHit2D suelo = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            transform.localScale.y + 0.1f,
            layerMask
        );

        Debug.DrawRay(transform.position, Vector2.down, UnityEngine.Color.red);

        if (suelo.collider != null)
        {
            Debug.Log("Golpeo: " + suelo.collider.name);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Salta()
    {
        if (TocaSuelo())
        {
            rig.AddForce(transform.up * jumpForce);
        }
    }

    [PunRPC]
    void RPC_AddPoints(int _points)
    {
        points += _points;

        GetComponentInChildren<TextMeshPro>().text = points.ToString();

        Debug.Log(points);
    }

    public void Die()
    {
        pv.RPC(nameof(RPC_Die), RpcTarget.All);
    }

    [PunRPC]
    void RPC_Die()
    {
        GetComponent<SpriteRenderer>().color = UnityEngine.Color.clear;
        GetComponentInChildren<TextMeshPro>().color = UnityEngine.Color.clear;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        die = true;
        StartCoroutine(Reactivate());
    }

    IEnumerator Reactivate()
    {
        yield return new WaitForSeconds(2f);

        GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
        GetComponentInChildren<TextMeshPro>().color = UnityEngine.Color.white;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = false;

        die = false;
        transform.position = posicioninicial;
    }
}