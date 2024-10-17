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
    public GameObject coinPrefab;
    private GameObject rampWall;
    private CounterController gameManager;
    public int coinSpawnAmount;
    public Vector3 rightBoundLocation; 
    public Vector3 leftBoundLocation;   
    public float speed;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed.");
            spawnCoin("drop");
        }
        if(Input.GetKey(KeyCode.D))
        {            
            transform.position = Vector3.MoveTowards(coinDropper.transform.position, rightBoundLocation, step);
        }
        if(Input.GetKey(KeyCode.A))
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
        Debug.Log("Attempting to spawn coinPrefab");
        switch (type)
        {
            case "init1":
                Quaternion spawnRotation = Quaternion.Euler(Random.Range(0, 15), Random.Range(0, 15), Random.Range(0, 15));
                Instantiate(coinPrefab, generateRandomPos(coinSpawnPylonA, coinSpawnPylonB), spawnRotation);
                break;
            case "init2":
            {
                spawnRotation = Quaternion.Euler(Random.Range(0, 80), 0, 0);
                Instantiate(coinPrefab, generateRandomPos(coinSpawnPylonC, coinSpawnPylonD), spawnRotation);
                break;
            }
            case "drop":
                if (gameManager.getCounter("coins") > 0)
                {
                    Vector3 dropPos = new Vector3(coinDropper.transform.position.x, coinDropper.transform.position.y, coinDropper.transform.position.z - 0.15f);
                    spawnRotation = Quaternion.Euler(90, 0, 0);
                    Instantiate(coinPrefab, dropPos, spawnRotation);
                    gameManager.minusCounter(1, "coins");
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
}
