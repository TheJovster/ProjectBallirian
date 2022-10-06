using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float searchDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    private PlayerUI playerUI;

    private void Awake()
    {
        playerUI = GetComponent<PlayerUI>();
    }

    private void Update()
    {
        playerUI.UpdateText(string.Empty);
        DetectInteractable();
    }

    private void DetectInteractable() 
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * searchDistance);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, searchDistance, interactableLayer)) 
        {
            if(hitInfo.collider.gameObject.GetComponent<Interactable>() != null) 
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if(Input.GetKeyDown(KeyCode.E)) 
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
