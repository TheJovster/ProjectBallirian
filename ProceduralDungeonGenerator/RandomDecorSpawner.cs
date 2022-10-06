using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDecorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] decorPrefabs;
    [SerializeField] private Transform roomCenter;

    DungeonGenerator myDungeonGenerator;
    private bool isCompleted = false;

    void Start()
    {
        myDungeonGenerator = GameObject.Find("DungeonGenerator").GetComponent<DungeonGenerator>();
    }
    void Update()
    {
        if(!isCompleted && myDungeonGenerator.generationState == DungeonGenerationState.completed) 
        {
            isCompleted = true;
            int decorIndex = Random.Range(0, decorPrefabs.Length);
            GameObject goDecor = Instantiate(decorPrefabs[decorIndex], roomCenter.position, roomCenter.transform.rotation, transform) as GameObject;
            goDecor.name = decorPrefabs[decorIndex].name;
        }
    }
}
