using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]private GameObject playerCoreObject;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer() 
    {
        Instantiate(playerCoreObject, transform.position + Vector3.up * .05f, Quaternion.identity);
    }
}
