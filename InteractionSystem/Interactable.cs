using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;

    //call this function from the PlayerController/PlayerInteract script
    public void BaseInteract() 
    {
        Interact();
    }

    protected virtual void Interact() 
    {
        //class kept empty
        //overriden by classes inheriting from this class
    }
}
