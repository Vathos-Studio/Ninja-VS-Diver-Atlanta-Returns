using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Rino : MonoBehaviourPun, IPunObservable
{
    private enum Estados
    {
        Idle,
        Run,
        Collision
    }

    private Estados estado = Estados.Idle;

    private Transform objetivo;

    [SerializeField] private float distanciaDeteccion = 9f;
    [SerializeField] private float velocidadMovimiento = 3f;

    private float distanciaActual;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool yaColisiono = false;

    // ===== NETWORK =====
    private Vector3 networkPosition;
    private bool networkFlipX;
    private int networkState;

    private bool isMaster;

    private void Start()
    {
        isMaster = PhotonNetwork.IsMasterClient;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        networkPosition = transform.position;
    }

    private void Update()
    {
        if (isMaster)
        {
            MasterLogic();
        }
        else
        {
            ClientVisual();
        }
    }

    // =========================
    // MASTER (IA REAL)
    // =========================
    private void MasterLogic()
    {
        if (estado == Estados.Idle || estado == Estados.Run)
        {
            BuscarJugadorMasCercano();
        }

        switch (estado)
        {
            case Estados.Idle:
                IdleState();
                break;

            case Estados.Run:
                RunState();
                break;

            case Estados.Collision:
                CollisionState();
                break;
        }
    }

    //  FIX IMPORTANTE: detecta TODOS los jugadores en tiempo real
    private void BuscarJugadorMasCercano()
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");

        float distanciaMinima = Mathf.Infinity;
        Transform masCercano = null;

        foreach (GameObject j in jugadores)
        {
            if (j == null) continue;

            float dis = Vector2.Distance(transform.position, j.transform.position);

            if (dis < distanciaMinima)
            {
                distanciaMinima = dis;
                masCercano = j.transform;
            }
        }

        if (masCercano != null)
        {
            objetivo = masCercano;
            distanciaActual = distanciaMinima;
        }
    }

    private void IdleState()
    {
        if (objetivo == null)
        {
            animator.SetBool("Run", false);
            return;
        }

        Girar(objetivo.position);

        if (distanciaActual < distanciaDeteccion)
        {
            estado = Estados.Run;
        }

        animator.SetBool("Run", false);
    }

    private void RunState()
    {
        if (objetivo == null) return;

        animator.SetBool("Run", true);

        transform.position = Vector2.MoveTowards(
            transform.position,
            objetivo.position,
            velocidadMovimiento * Time.deltaTime
        );

        Girar(objetivo.position);
    }

    private void CollisionState()
    {
        if (yaColisiono) return;

        yaColisiono = true;

        animator.SetBool("Collision", true);

        StartCoroutine(ResetAfterCollision());
    }

    private IEnumerator ResetAfterCollision()
    {
        yield return new WaitForSeconds(5f);

        estado = Estados.Idle;
        yaColisiono = false;
        animator.SetBool("Collision", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estado == Estados.Collision) return;
        if (collision.collider.CompareTag("Suelo")) return;

        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<Player>()?.Die();
        }

        estado = Estados.Collision;
    }

    // =========================
    // CLIENT (solo visual)
    // =========================
    private void ClientVisual()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            networkPosition,
            Time.deltaTime * 10f
        );

        spriteRenderer.flipX = networkFlipX;
        estado = (Estados)networkState;
    }

    // =========================
    // SYNC PHOTON
    // =========================
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(spriteRenderer.flipX);
            stream.SendNext((int)estado);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkFlipX = (bool)stream.ReceiveNext();
            networkState = (int)stream.ReceiveNext();
        }
    }

    // =========================
    // UTIL
    // =========================
    private void Girar(Vector3 objetivoPos)
    {
        spriteRenderer.flipX = transform.position.x > objetivoPos.x;
    }
}