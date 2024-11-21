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
    private Collider coinCollider;
    private bool parentable = false;//If the object can be made the child of another object
    private bool pushable = false;//If the object can be pushed by a pusher object
    private CounterController gameManager;
    public AudioController audioController;
    private bool collected = false;
    private bool silent = false;
    private RaycastHit hit;
    public PhysicMaterial highFriction;
    public PhysicMaterial repelling;
    public bool stuck = true;

    // Start is called before the first frame update
    void Start()
    {
        coinAudio = gameObject.GetComponent<AudioSource>();
        coinRb = gameObject.GetComponent<Rigidbody>();
        coinCollider = gameObject.GetComponent<Collider>();
        coinCollider.material = highFriction;
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();
        audioController = GameObject.Find("Audio Manager").GetComponent<AudioController>();
        StartCoroutine(stuckPrevention());
    }

    // Update is called once per frame
    void Update()
    {
        raycastCheck();
        /**
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Adding force");
            coinRb.AddForce(Vector3.up * 0.05f, ForceMode.Impulse);
        }
        coinRb.AddForce(Vector3.down * 0.0001f, ForceMode.Impulse);//Stops coin from becoming completely still and not registering collisions and adds extra gravity
        */
    }


    void OnCollisionEnter(Collision collision)//Upon colliding with something
    {
        //Debug.Log("Collided");
        if (!collision.gameObject.CompareTag("Coin") && !silent)
        {
            coinAudio.pitch = Random.Range(0.9f, 1.2f);
            coinAudio.PlayOneShot(hitSound1, 0.5f);
        }
        if (collision.gameObject.CompareTag("Pusher"))
        {
            if (pushable)
            {
                Vector3 awayFromPusher = collision.gameObject.transform.position - transform.position;
                coinRb.AddForce(awayFromPusher * 0.0005f, ForceMode.Impulse);
            }
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
            if(!gameManager.getCheatStatus())
            {
                gameManager.addCounter(1, "bank", "");
            }
            StartCoroutine("destroyDelay");
            collected = true;
        }
        if (other.CompareTag("Killbox"))
        {
            if (!collected)
            {
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("PusherTrigger") && !parentable)
        {
            Vector3 awayFromPusher = other.gameObject.transform.position - transform.position;
            coinRb.AddForce(awayFromPusher * 0.0005f, ForceMode.Impulse);
        }
        if (other.CompareTag("DropperExit"))
        {
            parentable = true;
            stuck = false;
            coinCollider.material = repelling;
        }
    }

    IEnumerator destroyDelay()
    {
        coinAudio.PlayOneShot(collectSound, 0.8f);
        yield return new WaitForSeconds(1.202f);
        Destroy(gameObject);
    }

    public void setSilent(bool value)
    {
        silent = value;
    }

    public bool getParentable()
    {
        return parentable;
    }

    public void setParentable(bool parentableValue)
    {
        parentable = parentableValue;
    }

    void raycastCheck()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            if (hit.collider.tag == "Pusher" && parentable)
            {
                transform.parent = hit.transform.parent;
            }
            if (hit.collider.tag == "Floor")
            {
                transform.parent = null;
                pushable = true;
            }
        }
    }

    IEnumerator stuckPrevention()
    {
        yield return new WaitForSeconds(16);
        if (stuck)
        {
            if (!gameManager.getCheatStatus())
            {
                gameManager.addCounter(1, "coins", "");
            }
            Destroy(gameObject);
        }
    }

}
