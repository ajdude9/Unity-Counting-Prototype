using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MenuGems : MonoBehaviour
{
    private GameObject spawnPylonA;//The leftmost spawning position
    private GameObject spawnPylonB;//The rightmost spawning position
    public GameObject gemPrefab;//A prefab containing the gems to spawn
    // Start is called before the first frame update
    void Start()
    {
        spawnPylonA = GameObject.Find("SpawnPylonA");
        spawnPylonB = GameObject.Find("SpawnPylonB");
        InvokeRepeating("spawnGem", 0, 0.3f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnGem()
    {
        float placement = Random.Range(spawnPylonA.transform.position.x, spawnPylonB.transform.position.x);    
        this.gameObject.transform.position = new Vector3(placement, transform.position.y, transform.position.z);
        Instantiate(gemPrefab, transform.position, gemPrefab.transform.rotation);
    }
}
