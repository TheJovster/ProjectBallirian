using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] private Animator animator;
    private bool isOpen = false;

    protected override void Interact()
    {
        if(!isOpen) 
        {
            OpenDoor();
            isOpen = !isOpen;
            //use audio manager instance to trigger open/close door sound
        }
        else if(isOpen) 
        {
            CloseDoor();
            isOpen = !isOpen;
            //use audio manager instance to trigger open/close door sound
        }
    }

    private void OpenDoor() 
    {
        animator.SetBool("CloseDoor", false);
        animator.SetBool("OpenDoor", true);
    }

    private void CloseDoor() 
    {
        animator.SetBool("CloseDoor", true);
        animator.SetBool("OpenDoor", false);
    }
}
