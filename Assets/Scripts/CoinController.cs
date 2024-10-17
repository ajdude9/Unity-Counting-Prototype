using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinController : MonoBehaviour
{

    private AudioSource coinAudio;
    public AudioClip hitSound1;
    public AudioClip collectSound;
    private Rigidbody coinRb;
    public bool parentable = true;
    private CounterController gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        coinAudio = gameObject.GetComponent<AudioSource>();
        coinRb = gameObject.GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Adding force");
            coinRb.AddForce(Vector3.up * 0.05f, ForceMode.Impulse);
        }
        coinRb.AddForce(Vector3.down * 0.0001f, ForceMode.Impulse);//Stops coin from becoming completely still and not registering collisions and adds extra gravity
    }


    void OnCollisionEnter(Collision collision)//Upon colliding with something
    {
        //Debug.Log("Collided");
        if(!collision.gameObject.CompareTag("Coin"))
        {
            coinAudio.pitch = Random.Range(0.9f, 1.2f);
            coinAudio.PlayOneShot(hitSound1, 0.5f);
        }
        if (collision.gameObject.CompareTag("Pusher") && parentable)
        {
            transform.parent = collision.transform.parent;            
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            parentable = false;
            transform.parent = null;            
        }
        

    }

    void OnCollisionExit(Collision collision)//Upon no longer colliding with something
    {
        //transform.parent = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collector"))
        {
            Debug.Log("Collected");
            coinAudio.pitch = Random.Range(0.9f, 1.2f);
            coinAudio.PlayOneShot(collectSound, 0.8f);
            gameManager.addCounter(1, "bank");
            Destroy(gameObject);
        }
        if (other.CompareTag("Killbox"))
        {
            Debug.Log("Destroyed");
            coinAudio.pitch = Random.Range(0.2f, 0.6f);
            coinAudio.PlayOneShot(collectSound, 0.8f);
            Destroy(gameObject);
        }
    }

}
