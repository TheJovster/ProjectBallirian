using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxLookUpAngle = 60f;
    [SerializeField] private float minLookUpAngle = -60f;

    // Start is called before the first frame update
    void Start()
    {
        if(mainCamera == null) 
        {
            mainCamera = Camera.main;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();
        CameraVerticalRotation();
    }


    private void RotatePlayer()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");

        transform.Rotate(0f, mouseX * rotationSpeed * Time.deltaTime, 0f);
    }

    private void CameraVerticalRotation() //rotates the camera on the X axis
    {
        float mouseY = Input.GetAxisRaw("Mouse Y");

        mainCamera.transform.eulerAngles -= Vector3.right * Mathf.Clamp(mouseY * rotationSpeed * Time.deltaTime, minLookUpAngle, maxLookUpAngle); ;
    }
}
