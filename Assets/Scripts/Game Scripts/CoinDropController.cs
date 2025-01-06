using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoinDropController : MonoBehaviour
{
    private GameObject coinSpawnPylonA;//A boundary for spawning coins on the floor of the machine
    private GameObject coinSpawnPylonB;//A boundary for spawning coins on the floor of the machine
    private GameObject coinSpawnPylonC;//A boundary for spawning coins on the pusher
    private GameObject coinSpawnPylonD;//A boundary for spawning coins on the pusher
    private GameObject coinDropper;//The object that drops the coins
    [SerializeField] private GameObject coinPrefab;//The coin
    private GameObject rampWall;//The wall that stops coins from sliding down the ramp at the start
    private GameObject parentableTrigger;//The trigger that sets pre-spawned coins to their proper states
    private CounterController gameManager;//The game manager
    [SerializeField] private Vector3 rightBoundLocation;//The furthest right the dropper can move
    [SerializeField] private int coinSpawnAmount;//The amount of coins to spawn
    [SerializeField] private Vector3 leftBoundLocation;//The furthest left the dropper can move
    [SerializeField] private float speed;//How fast the dropper moves


    // Start is called before the first frame update
    void Start()
    {
        //Find the boundaries for spawning coins
        coinSpawnPylonA = GameObject.Find("CoinSpawnPylonA");
        coinSpawnPylonB = GameObject.Find("CoinSpawnPylonB");
        coinSpawnPylonC = GameObject.Find("CoinSpawnPylonC");
        coinSpawnPylonD = GameObject.Find("CoinSpawnPylonD");
        rampWall = GameObject.Find("Ramp Wall");//Find the ramp wall
        parentableTrigger = GameObject.Find("Parentable Trigger");//Find the parentable trigger
        coinDropper = GameObject.Find("Coin Dropper");//Find the coin dropper
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();//Find the game manager
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.viewType == "coin")//If the coin machine is being looked at, allow input
        {
            inputManager();
        }

    }

    void inputManager()//Manage player input
    {
        var step = speed * Time.deltaTime;//Step is speed regulated by deltaTime
        if (Input.GetKey(KeyCode.Space))//If the space key is pressed
        {
            bool spawnable = true;//Allow a coin to be spawned
            //Debug.Log("Space key pressed.");
            if(checkOverlap(transform.position, 0.5f))
            {
                spawnable = false;//If a coin is already in the way though, don't allow it to be spawned
            }
            if (spawnable)//If the coin can be spawned
            {
                spawnCoin("drop");//Spawn a single coin
            }
        }
        if (Input.GetKey(KeyCode.D))//If the D key is pressed
        {
            transform.position = Vector3.MoveTowards(coinDropper.transform.position, rightBoundLocation, step);//Move to the right
        }
        if (Input.GetKey(KeyCode.A))//If the A key is pressed
        {
            transform.position = Vector3.MoveTowards(coinDropper.transform.position, leftBoundLocation, step);//Move toward the left
        }
    }

    public void generateCoins(int coinsToGenerate)//Generate the initial coins to populate the machine
    {
        //Debug.Log("Generating coins.");
        for (int i = 0; i < coinsToGenerate / 2; i++)//Spawn half the coins on the floor of the machine
        {
            spawnCoin("init1");
        }
        for (int i = 0; i < coinsToGenerate / 2; i++)//Spawn half the coins on the pusher
        {
            spawnCoin("init2");
        }

        StartCoroutine(breakWall());//Destroy the wall stopping coins falling down the ramp after a delay
    }

    private void spawnCoin(String type)//Spawn a coin of a certain type
    {
        float spawnCheckRadius = 0.33f;//Set the radius for checking if a coin is overlapping
        Debug.Log("Attempting to spawn coinPrefab");
        switch (type)//Depending on the type of command issued
        {
            case "init1"://If initialising the first set
                Vector3 spawnPosition = generateRandomPos(coinSpawnPylonA, coinSpawnPylonB);//Generate a random position between two coin spawn pylons on the floor
                if(!checkOverlap(spawnPosition, spawnCheckRadius))//If not overlapping with another coin
                {
                    Quaternion spawnRotation = Quaternion.Euler(Random.Range(0, 15), Random.Range(0, 15), Random.Range(0, 15));//Generate a random rotation for the coin between 0 and 15
                    Instantiate(coinPrefab, spawnPosition, spawnRotation);//Spawn the coin
                }
                break;
            case "init2"://If initialising the second set
                {
                    spawnPosition = generateRandomPos(coinSpawnPylonC, coinSpawnPylonD);//Generate a random position between two coin spawn pylons on the pusher
                    if(!checkOverlap(spawnPosition, spawnCheckRadius))//If not overlapping with another coin
                    {
                        Quaternion spawnRotation = Quaternion.Euler(Random.Range(0, 80), 0, 0);//Generate a random rotation on the X axis between 0 and 80
                        Instantiate(coinPrefab, spawnPosition, spawnRotation);//Spawn the coin
                    }
                    break;
                }
            case "drop"://If dropping a coin into the machine
                if (gameManager.getCounter("coins", "") > 0 || gameManager.getCheatStatus())//If the player has coins to drop, or is cheating for infinite coins
                {
                    Vector3 dropPos = new Vector3(coinDropper.transform.position.x, coinDropper.transform.position.y, coinDropper.transform.position.z - 0.15f);//Place the drop position slightly in front of the dropper
                    Quaternion spawnRotation = Quaternion.Euler(90, 0, 0);//Set the coin to be rotated flat
                    Instantiate(coinPrefab, dropPos, spawnRotation);//Spawn the coin
                    if(!gameManager.getCheatStatus())//If the player isn't cheating
                    {
                        gameManager.minusCounter(1, "coins", "");//Subtract a coin from the total coins droppable
                    }
                }
                break;
        }
    }

    private Vector3 generateRandomPos(GameObject pylonA, GameObject pylonB)//Generate a random position between two locations
    {
        return new Vector3(Random.Range(pylonA.transform.position.x, pylonB.transform.position.x), pylonA.transform.position.y, Random.Range(pylonA.transform.position.z, pylonB.transform.position.z));
    }

    IEnumerator breakWall()//Destroy the ramp wall and the parentableTrigger after 1 second
    {
        yield return new WaitForSeconds(1);
        Destroy(rampWall);
        Destroy(parentableTrigger);
    }

    public int getCoinSpawnAmount()//Get how many coins are to be spawned
    {
        return coinSpawnAmount;
    }

    public bool checkOverlap(Vector3 pos, float checkRadius)//Check if an object is overlapping with another
    {
        bool overlapping = false;//State the the object isn't initially overlapping until proven otherwise
        Collider[] overlappingColliders = Physics.OverlapSphere(pos, checkRadius);//Check for all colliders in a sphere around the object, within a radius
        foreach (var overlap in overlappingColliders)//For every overlapping collider found
        {
            if (overlap.gameObject.CompareTag("Coin"))//If any of the colliders belong to a coin
            {
                overlapping = true;//State the object is overlapping
                //Debug.Log("Found overlapping coin.");
            }
        }
        return overlapping;//Return the status
    }
}
