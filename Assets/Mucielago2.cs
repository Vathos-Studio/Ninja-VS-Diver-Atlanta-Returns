using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murcielago2 : MonoBehaviour
{
    private enum estados
    {
        Idle, Flying, Volver
    }
    estados estado = estados.Idle;
private Transform jugador;
    [SerializeField] private float distancia;
    public Vector3 puntoInicial;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    float tiempoSeguir;
    public float tiempoBase;
    public float velocidadMovimiento;


    private void Start()
    {
        jugador = FindAnyObjectByType<Player>().GetComponent<Transform>();
        animator = GetComponent<Animator>();
        puntoInicial = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        distancia = Vector2.Distance(transform.position, jugador.position);
        animator.SetFloat("Distancia", distancia);

        switch (estado)
        {
            case estados.Idle:
                IdleEstate();
                break;
            case estados.Flying:
                FlyingState();
                break;
            case estados.Volver:
                VolverState();
                break;

        }
    }

    private void VolverState()
    {
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, puntoInicial, velocidadMovimiento * Time.deltaTime);
        this.Girar(puntoInicial);
        if (animator.transform.position == puntoInicial)
        {
            StartCoroutine(waitToCheckAgain());
        }
    }
    IEnumerator waitToCheckAgain()
    {
        animator.SetBool("Vuela", false);
        yield return new WaitForSeconds(2f);
        
        estado = estados.Idle;
        StopAllCoroutines();
    }
    private void FlyingState()
    {
        animator.SetBool("Vuela",true);
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, jugador.position, velocidadMovimiento * Time.deltaTime);
        this.Girar(jugador.position);
        tiempoSeguir -= Time.deltaTime;
        if (tiempoSeguir <= 0)
        {
            
            estado = estados.Volver;
        }
    }

    private void IdleEstate()
    {
        tiempoSeguir = tiempoBase;
        if (distancia < 9)
        {
            estado = estados.Flying;
        }
    }

    public void Girar(Vector3 objetivo)
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
