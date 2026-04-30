using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Rino : MonoBehaviour
{
    private enum Estados
    {
        Idle,
        Run,
        Collision
    }

    private Estados estado = Estados.Idle;

    private Transform[] jugadores;
    private Transform objetivo;

    [SerializeField] private float distanciaDeteccion = 9f;
    [SerializeField] private float velocidadMovimiento = 3f;

    private float distanciaActual;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool yaColisiono = false;

    private void Start()
    {
        // Solo el Master controla este enemigo
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(gameObject);
            return;
        }

        // Buscar jugadores
        Player[] jugadorPlayer = FindObjectsByType<Player>(FindObjectsSortMode.None);
        jugadores = new Transform[jugadorPlayer.Length];

        for (int i = 0; i < jugadorPlayer.Length; i++)
        {
            jugadores[i] = jugadorPlayer[i].transform;
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (estado != Estados.Collision)
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

    private void BuscarJugadorMasCercano()
    {
        float distanciaMinima = Mathf.Infinity;
        Transform masCercano = null;

        foreach (Transform j in jugadores)
        {
            float dis = Vector2.Distance(transform.position, j.position);

            if (dis < distanciaMinima)
            {
                distanciaMinima = dis;
                masCercano = j;
            }
        }

        if (masCercano != null)
        {
            distanciaActual = distanciaMinima;
            objetivo = masCercano;
        }
    }

    private void IdleState()
    {
        if (objetivo == null) return;

        Girar(objetivo.position);

        if (distanciaActual < distanciaDeteccion)
        {
            estado = Estados.Run;
        }
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
        StartCoroutine(DestruirDespues());
    }

    private IEnumerator DestruirDespues()
    {
        yield return new WaitForSeconds(1f); // Ajusta al tiempo de tu animación
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estado == Estados.Collision) return;
        if (collision.collider.CompareTag("Suelo")) return;
        estado = Estados.Collision;
    }

    private void Girar(Vector3 objetivo)
    {
        if (transform.position.x < objetivo.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}