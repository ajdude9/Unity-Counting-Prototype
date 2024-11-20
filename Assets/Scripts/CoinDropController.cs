using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoinDropController : MonoBehaviour
{
    private GameObject coinSpawnPylonA;
    private GameObject coinSpawnPylonB;
    private GameObject coinSpawnPylonC;
    private GameObject coinSpawnPylonD;
    private GameObject coinDropper;
    [SerializeField] private GameObject coinPrefab;
    private GameObject rampWall;
    private CounterController gameManager;
    [SerializeField] private Vector3 rightBoundLocation;
    [SerializeField] private int coinSpawnAmount;
    [SerializeField] private Vector3 leftBoundLocation;
    [SerializeField] private float speed;


    // Start is called before the first frame update
    void Start()
    {
        coinSpawnPylonA = GameObject.Find("CoinSpawnPylonA");
        coinSpawnPylonB = GameObject.Find("CoinSpawnPylonB");
        coinSpawnPylonC = GameObject.Find("CoinSpawnPylonC");
        coinSpawnPylonD = GameObject.Find("CoinSpawnPylonD");
        rampWall = GameObject.Find("Ramp Wall");
        coinDropper = GameObject.Find("Coin Dropper");
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.viewType == "coin")
        {
            inputManager();
        }

    }

    void inputManager()
    {
        var step = speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            bool spawnable = true;
            Debug.Log("Space key pressed.");
            if(checkOverlap(transform.position, 0.5f))
            {
                spawnable = false;
            }
            if (spawnable)
            {
                spawnCoin("drop");
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = Vector3.MoveTowards(coinDropper.transform.position, rightBoundLocation, step);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = Vector3.MoveTowards(coinDropper.transform.position, leftBoundLocation, step);
        }
    }

    public void generateCoins(int coinsToGenerate)
    {
        Debug.Log("Generating coins.");
        for (int i = 0; i < coinsToGenerate / 2; i++)
        {
            spawnCoin("init1");
        }
        for (int i = 0; i < coinsToGenerate / 2; i++)
        {
            spawnCoin("init2");
        }
        StartCoroutine(breakWall());
    }

    private void spawnCoin(String type)
    {
        float spawnCheckRadius = 0.33f;
        Debug.Log("Attempting to spawn coinPrefab");
        switch (type)
        {
            case "init1":
                Vector3 spawnPosition = generateRandomPos(coinSpawnPylonA, coinSpawnPylonB);
                if(!checkOverlap(spawnPosition, spawnCheckRadius))
                {
                    Quaternion spawnRotation = Quaternion.Euler(Random.Range(0, 15), Random.Range(0, 15), Random.Range(0, 15));                
                    Instantiate(coinPrefab, spawnPosition, spawnRotation);
                }
                break;
            case "init2":
                {
                    spawnPosition = generateRandomPos(coinSpawnPylonC, coinSpawnPylonD);
                    if(!checkOverlap(spawnPosition, spawnCheckRadius))
                    {
                        Quaternion spawnRotation = Quaternion.Euler(Random.Range(0, 80), 0, 0);
                        Instantiate(coinPrefab, spawnPosition, spawnRotation);
                    }
                    break;
                }
            case "drop":
                if (gameManager.getCounter("coins", "") > 0)
                {
                    Vector3 dropPos = new Vector3(coinDropper.transform.position.x, coinDropper.transform.position.y, coinDropper.transform.position.z - 0.15f);
                    Quaternion spawnRotation = Quaternion.Euler(90, 0, 0);
                    Instantiate(coinPrefab, dropPos, spawnRotation);
                    gameManager.minusCounter(1, "coins", "");
                }
                break;
        }
    }

    private Vector3 generateRandomPos(GameObject pylonA, GameObject pylonB)
    {
        return new Vector3(Random.Range(pylonA.transform.position.x, pylonB.transform.position.x), pylonA.transform.position.y, Random.Range(pylonA.transform.position.z, pylonB.transform.position.z));
    }

    IEnumerator breakWall()
    {
        yield return new WaitForSeconds(1);
        Destroy(rampWall);
    }

    public int getCoinSpawnAmount()
    {
        return coinSpawnAmount;
    }

    public bool checkOverlap(Vector3 pos, float checkRadius)
    {
        bool overlapping = false;
        Collider[] overlappingColliders = Physics.OverlapSphere(pos, checkRadius);
        foreach (var overlap in overlappingColliders)
        {
            if (overlap.gameObject.CompareTag("Coin"))
            {
                overlapping = true;
                Debug.Log("Found overlapping coin.");
            }
        }
        return overlapping;
    }
}
