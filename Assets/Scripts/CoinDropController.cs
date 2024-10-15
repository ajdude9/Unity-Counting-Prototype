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
    private GameObject coinDropper;
    public GameObject coinPrefab;
    private CounterController gameManager;
    public int coinSpawnAmount;

    // Start is called before the first frame update
    void Start()
    {
        coinSpawnPylonA = GameObject.Find("CoinSpawnPylonA");
        coinSpawnPylonB = GameObject.Find("CoinSpawnPylonB");
        coinDropper = GameObject.Find("Coin Dropper");        
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.viewType == "coin")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space key pressed.");
                spawnCoin("drop");
            }
        }
    }
    public void generateCoins(int coinsToGenerate)
    {
        Debug.Log("Generating coins.");
        for (int i = 0; i < coinsToGenerate; i++)
        {
            spawnCoin("init");
        }
    }

    private void spawnCoin(String type)
    {
        Debug.Log("Attempting to spawn coinPrefab");
        switch (type)
        {
            case "init":
                Quaternion spawnRotation = Quaternion.Euler(Random.Range(0, 80), 0, 0);
                Instantiate(coinPrefab, generateRandomPos(), spawnRotation);
                break;
            case "drop":
                Vector3 dropPos = new Vector3(coinDropper.transform.position.x, coinDropper.transform.position.y, coinDropper.transform.position.z - 0.25f);
                spawnRotation = Quaternion.Euler(90, 0, 0);
                Instantiate(coinPrefab, dropPos, spawnRotation);
                break;
        }
    }

    private Vector3 generateRandomPos()
    {
        return new Vector3(Random.Range(coinSpawnPylonA.transform.position.x, coinSpawnPylonB.transform.position.x), coinSpawnPylonA.transform.position.y, Random.Range(coinSpawnPylonA.transform.position.z, coinSpawnPylonB.transform.position.z));
    }

}
