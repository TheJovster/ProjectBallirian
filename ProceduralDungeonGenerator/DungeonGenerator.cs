using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DungeonGenerationState 
{
    inactive,
    generatingMain,
    generatingBranches,
    cleaning,
    completed
}

public class DungeonGenerator : MonoBehaviour
{
    [Header("Debugging Options")]
    [SerializeField] private bool useBoxColliders;
    [SerializeField] private bool useLightsForDebugging;
    [SerializeField] private bool restoreLightsAfterDebugging;
    [SerializeField] private bool isInDebuggingMode;

    [Header("KeyBindings")]
    [SerializeField] private KeyCode reloadSceneKey = KeyCode.G;
    //[SerializeField] private KeyCode toggleMapKey = KeyCode.M;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] tilePrefabs;
    [SerializeField] private GameObject[] startPrefabs;
    [SerializeField] private GameObject[] exitPrefabs;
    [SerializeField] private GameObject[] blockedPrefabs;
    [SerializeField] private GameObject[] doorPrefabs;

    //[Header("Random Prefab Lists")]

    [Header("Lists - Available At RunTime")]
    public List<Tile> generatedTiles = new List<Tile>();
    public DungeonGenerationState generationState = DungeonGenerationState.inactive;

    [Header("Variables")]
    [Range(0, 1f)][SerializeField] private float constructionDelay = .5f;
    [Range(2, 100)][SerializeField] private int mainLength = 10;
    [Tooltip("Ideally, the branch length should be 1/2 of the main length.")] [Range(0, 100)][SerializeField] private int branchLength = 5; // mainLength/2
    [Range(0, 100)][SerializeField] private int doorPercent = 25;
    [Range(0, 25)][SerializeField] private int numberOfBranches = 5;

    //private variables
    private Transform tileFrom;
    private Transform tileTo;
    private Transform tileRoot;
    private Transform container;

    private int attempts;
    private int maxAttempts = 50;

    private List<Connector> availableConnectors = new List<Connector>();

    public bool GenerationComplete { get; private set; }

    void Start()
    {
        StartCoroutine(RunGenerator());
    }

    private IEnumerator RunGenerator()
    {
        // instantiate container and set parenting
        GameObject goContainer = new GameObject("Main Path");
        container = goContainer.transform;
        container.SetParent(transform);
        //creates start tile
        tileRoot = CreateStartTile();
        tileTo = tileRoot;
        generationState = DungeonGenerationState.generatingMain;
        while (generatedTiles.Count < mainLength)
        {
            //creates the main path
            yield return new WaitForSeconds(constructionDelay);
            tileFrom = tileTo;
            if (generatedTiles.Count == mainLength - 1) 
            {
                //generate exit room
                tileTo = CreateExitTile();
                Debug.Log("EndTile constructed");

            }
            else 
            {
                tileTo = CreateTile();
            }
            ConnectTiles();
            CollisionCheck();
            Debug.Log("First CollisionCheckDone");
/*            if (attempts >= maxAttempts)
            {
                Debug.Log("Attempts >= maxAttempts"); I think I removed this before? because it never fired off
                break; 
            } */
        }
        //get all connectors within container that are NOT already connected
        foreach (Connector connector in container.GetComponentsInChildren<Connector>()) 
        {
            if(!connector.isConnected) 
            {
                if(!availableConnectors.Contains(connector)) 
                {
                    availableConnectors.Add(connector);
                }
            }
        }
        //creates the branching paths
        generationState = DungeonGenerationState.generatingBranches;
        for(int b = 0; b < numberOfBranches; b++) 
        {
            if (availableConnectors.Count > 0)
            {
                goContainer = new GameObject("Branch " + (b + 1));
                container = goContainer.transform;
                container.SetParent(transform);
                int availableConnectorsIndex = Random.Range(0, availableConnectors.Count);
                tileRoot = availableConnectors[availableConnectorsIndex].transform.parent.parent;
                availableConnectors.RemoveAt(availableConnectorsIndex);
                tileTo = tileRoot;
                for (int i = 0; i < branchLength - 1; i++)
                {
                    yield return new WaitForSeconds(constructionDelay);
                    tileFrom = tileTo;
                    tileTo = CreateTile();
                    ConnectTiles();
                    CollisionCheck();
                    Debug.Log("Second CollisionCheckDone");
                    if (attempts >= maxAttempts) 
                    {
                        break;
                    }
                }
            }
            else break;
        }
        generationState = DungeonGenerationState.cleaning;
        CreateBlockedPassages();
        CleanupBoxes();
        SpawnDoors();
        generationState = DungeonGenerationState.completed;
        yield return null;
        GenerationComplete = true; //I have no clue what I'm trying to do here
        Debug.Log("Generation Complete");
    }

    private void SpawnDoors()
    {
        if(doorPercent > 0) 
        {
            Connector[] allConnectors = transform.GetComponentsInChildren<Connector>();
            for(int i = 0; i < allConnectors.Length; i++) 
            {
                Connector myConnector = allConnectors[i];
                if(myConnector.isConnected) 
                {
                    //random chance of spawning a door
                    int roll = Random.Range(1, 101);
                    if(roll <= doorPercent) 
                    {
                        //determines door spawn position and instantiates it
                        Vector3 halfExts = new Vector3(myConnector.size.x, 1f, myConnector.size.x ); //it was named halfExtents before - renamed for testing
                        Vector3 position = myConnector.transform.position;
                        Vector3 offset = Vector3.up * 0.5f;
                        Collider[] hits = Physics.OverlapBox(position + offset, halfExts, Quaternion.identity, LayerMask.GetMask("Door"));
                        if(hits.Length == 0) 
                        {
                            int doorIndex = Random.Range(0, doorPrefabs.Length);
                            GameObject goDoor = Instantiate(doorPrefabs[doorIndex], position, myConnector.transform.rotation, myConnector.transform) as GameObject;
                            goDoor.name = doorPrefabs[doorIndex].name;
                        }
                    }
                }
            }
        }
    }

    private void CreateBlockedPassages() //creates blocked passages and walls
    {
        foreach(Connector connector in transform.GetComponentsInChildren<Connector>()) 
        {
            if(!connector.isConnected) 
            {
                RoomCollisionData collisionData = connector.transform.parent.parent.GetComponent<RoomCollisionData>();
                Vector3 position = connector.transform.position; 
                int blockedPrefabIndex = Random.Range(0, blockedPrefabs.Length);
                GameObject goWall = Instantiate(blockedPrefabs[blockedPrefabIndex], position, connector.transform.rotation, connector.transform) as GameObject;
                goWall.name = blockedPrefabs[blockedPrefabIndex].name;
            }
        }
    }

    private void Update() //Update() used for debugging
    {
        if (Input.GetKeyDown(reloadSceneKey) && isInDebuggingMode)
        {
            Debug.Log("Regenerating Room");
            SceneManager.LoadScene(0);
        }
    }

    //dungeon generator proper 
    private void ConnectTiles()
    {
        Transform connectFrom = GetRandomConnector(tileFrom);
        if(connectFrom == null) { return; }

        Transform connectTo = GetRandomConnector(tileTo);
        if (connectTo == null) { return; }

        connectTo.SetParent(connectFrom);
        tileTo.SetParent(connectTo);
        connectTo.localPosition = Vector3.zero;
        connectTo.localRotation = Quaternion.identity;
        connectTo.Rotate(Vector3.up * 180f);
        tileTo.SetParent(container);
        connectTo.SetParent(tileTo.Find("Connectors"));

        generatedTiles.Last().connector = connectFrom.GetComponent<Connector>();
    }

    private Transform GetRandomConnector(Transform tile)
    {
        if (tile == null)
        {
            return null;
        }

        List<Connector> connectorList = tile.GetComponentsInChildren<Connector>().ToList<Connector>().FindAll(x => x.isConnected == false);
        if (connectorList.Count > 0)
        {
            int connectorIndex = Random.Range(0, connectorList.Count);
            connectorList[connectorIndex].isConnected = true;
            if(tile == tileFrom) 
            {
                BoxCollider box = tile.GetComponent<BoxCollider>();
                if(box == null) 
                {
                    box = tile.gameObject.AddComponent<BoxCollider>();
                    box.isTrigger = true;
                }
            }

            return connectorList[connectorIndex].transform;
        }
        return null;
    }


    //tile generations
    private Transform CreateStartTile() 
    {
        int startingRoomIndex = Random.Range(0, startPrefabs.Length);
        GameObject goTile = Instantiate(startPrefabs[startingRoomIndex], Vector3.zero, Quaternion.identity, container) as GameObject;
        goTile.name = startPrefabs[startingRoomIndex].name;
        float yRot = Random.Range(0, 4) * 90f;
        goTile.transform.Rotate(0f, yRot, 0f);

        //add to generated List
        generatedTiles.Add(new Tile(goTile.transform, null));

        return goTile.transform;
    }

    private Transform CreateExitTile() //I'll fix this later if it turns into a problem
    {
        int exitRoomIndex = Random.Range(0, exitPrefabs.Length);
        GameObject goTile = Instantiate(exitPrefabs[exitRoomIndex], Vector3.zero, Quaternion.identity, container) as GameObject;
        goTile.name = "DungeonExitTile";

        //add to generated List
        Transform origin = generatedTiles[generatedTiles.FindIndex(x => x.tile == tileFrom)].tile;
        generatedTiles.Add(new Tile(goTile.transform, origin));

        return goTile.transform;
    }

    private Transform CreateTile() 
    {
        int tileIndex = Random.Range(0, tilePrefabs.Length);
        GameObject goTile = Instantiate(tilePrefabs[tileIndex], Vector3.zero, Quaternion.identity, container) as GameObject;
        goTile.name = tilePrefabs[tileIndex].name;

        //add to generated List
        Transform origin = generatedTiles[generatedTiles.FindIndex(x => x.tile == tileFrom)].tile;
        generatedTiles.Add(new Tile(goTile.transform, origin));

        return goTile.transform;
    }


    //collisions
    private void CleanupBoxes() 
    {
        if(!useBoxColliders) 
        {
            foreach(Tile myTile in generatedTiles) 
            {
                BoxCollider box = myTile.tile.GetComponent<BoxCollider>();
                if(box != null) 
                {
                    Destroy(box);
                }
            }
        }
    }

    //checks for collisions and decides where to place tiles
    private void CollisionCheck() 
    {
        //do the collision check
        BoxCollider box = tileTo.GetComponent<BoxCollider>();
        RoomCollisionData colliderData = tileTo.GetComponent<RoomCollisionData>();
        if (box == null)
        {
            box = tileTo.gameObject.AddComponent<BoxCollider>();
            box.size = colliderData.roomColliderSize; //might wanna figure out how to do this programatically
            box.center = colliderData.roomColliderOffset;
            box.isTrigger = true;
        }
        
        Vector3 offset = (tileTo.right * box.center.x) + (tileTo.up * box.center.y) + (tileTo.forward * box.center.z);
        Vector3 halfExtents = box.bounds.extents;
        List<Collider> hits = Physics.OverlapBox(tileTo.position + offset, halfExtents, Quaternion.identity, LayerMask.GetMask("Tile")).ToList();
        if(hits.Count > 0) 
        {
            if(hits.Exists(x => x.transform != tileFrom && x.transform != tileTo)) 
            {
                //hit something other than tileFrom and/or tileTo
                attempts++;
                int toIndex = generatedTiles.FindIndex(x => x.tile == tileTo);
                if (generatedTiles[toIndex].connector != null) 
                {
                    generatedTiles[toIndex].connector.isConnected = false;
                }
                generatedTiles.RemoveAt(toIndex);
                DestroyImmediate(tileTo.gameObject);
                
                //backtracking 
                if(attempts >= maxAttempts)
                {
                    int fromIndex = generatedTiles.FindIndex(x => x.tile == tileFrom);
                    Tile myTileFrom = generatedTiles[fromIndex];

                    if (tileFrom != tileRoot)
                    {
                        if (myTileFrom.connector != null) 
                        {
                            myTileFrom.connector.isConnected = false;        
                        }
                        availableConnectors.RemoveAll(x => x.transform.parent.parent == tileFrom);
                        generatedTiles.RemoveAt(fromIndex);
                        DestroyImmediate(tileFrom.gameObject);
                        if(myTileFrom.origin != tileRoot) 
                        {
                            tileFrom = myTileFrom.origin;
                        }
                        else if (container.name.Contains("Main"))
                        {
                            if(myTileFrom.origin != null) 
                            {
                                tileRoot = myTileFrom.origin;
                                tileFrom = tileRoot;
                            }
                        }
                        else if (availableConnectors.Count > 0)
                        {
                            int availableIndex = Random.Range(0, availableConnectors.Count);
                            tileRoot = availableConnectors[availableIndex].transform.parent.parent;
                            availableConnectors.RemoveAt(availableIndex);
                            tileFrom = tileRoot;
                        }
                        else return;

                    }
                    else if (container.name.Contains("Main"))
                    {
                        if(myTileFrom.origin != null) 
                        {
                            tileRoot = myTileFrom.origin;
                            tileFrom = tileRoot;
                        }
                    }
                    else if (availableConnectors.Count > 0)
                    {
                        int availableIndex = Random.Range(0, availableConnectors.Count);
                        tileRoot = availableConnectors[availableIndex].transform.parent.parent;
                        availableConnectors.RemoveAt(availableIndex);
                        tileFrom = tileRoot;
                    }
                    else return;
                }
                //retry
                if (tileFrom != null)
                {
                    if (generatedTiles.Count == mainLength - 1)
                    {
                        //generate exit room
                        tileTo = CreateExitTile();

                    }
                    else
                    {
                        tileTo = CreateTile();
                    }
                    ConnectTiles();
                    CollisionCheck(); //?
                }

            }
            else
            {
                attempts = 0;
                //nothing else but tileTo and tileFrom was hit
                //restore attempts to zero
            }

        }

    }
}
