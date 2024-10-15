using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{

    private AudioSource projAudio;
    public AudioClip hitSound1;
    private Rigidbody coinRb;
    // Start is called before the first frame update
    void Start()
    {
        projAudio = gameObject.GetComponent<AudioSource>();
        coinRb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Adding force");
            coinRb.AddForce(Vector3.up * 50, ForceMode.Impulse);
        }
    }
    
    
    void OnCollisionEnter(Collision collision)//Upon colliding with something
    {
        //Debug.Log("Collided");
        
        projAudio.pitch = Random.Range(0.9f, 1.2f);
        projAudio.PlayOneShot(hitSound1, 0.5f);        
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Collector"))
        {
            Debug.Log("Collected");
            Destroy(gameObject);
        }
        if(other.CompareTag("Killbox"))
        {
            Debug.Log("Destroyed");
            Destroy(gameObject);
        }
    }
    
}
