using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murcielago_Seguir : StateMachineBehaviour
{
    Transform jugador;
    Murcielago murcielago;
    float tiempoSeguir;
    public float tiempoBase;
    public float velocidadMovimiento;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        tiempoSeguir = tiempoBase;
       jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        murcielago = animator.gameObject.GetComponent<Murcielago>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, jugador.position, velocidadMovimiento * Time.deltaTime);
        murcielago.Girar(jugador.position);
        tiempoSeguir -= Time.deltaTime;
        if (tiempoSeguir <=0)
        {
            animator.SetTrigger("Volver");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
