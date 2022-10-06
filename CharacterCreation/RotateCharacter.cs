using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    [SerializeField] private Transform playerModel;
    [SerializeField] private float rotationSpeed = 5f;
    private Animator animator;

    //animation controller variables

    private float animationTimer = 0f;

    private void Awake()
    {
        animator = playerModel.GetComponent<Animator>();
    }
    void Update()
    {
        animationTimer += Time.deltaTime;
        AnimationControl();
        if (Input.GetAxis("Horizontal") > 0f)
        {
            RotateRight();
        }
        else if (Input.GetAxis("Horizontal") < 0f)
        {
            NewMethod();
        }
        else
        {
            StopRotation();
        }
    }

    private void StopRotation()
    {
        playerModel.Rotate(Vector3.zero);
    }

    private void NewMethod()
    {
        playerModel.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void RotateRight()
    {
        playerModel.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }

    private void AnimationControl() 
    {
        if(animationTimer > 5f) 
        {
            animator.SetBool("IdleState1", true);
        }
        if(animationTimer > 10f) 
        {
            animator.SetBool("IdleState1", true);
            animationTimer = 0f;
        }
    }
}
