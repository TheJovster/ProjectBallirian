using UnityEngine;

public class MiniMapHandler : MonoBehaviour
{
    [SerializeField] private Camera miniMapCamera;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Vector3 transformOffset;

    void Start()
    {
        if(playerTransform == null) 
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void LateUpdate()
    {
        miniMapCamera.transform.position = playerTransform.position + transformOffset;
    }
}
