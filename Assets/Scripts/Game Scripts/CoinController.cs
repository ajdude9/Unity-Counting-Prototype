using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinController : MonoBehaviour
{

    private AudioSource coinAudio;//The audio source for coins
    public AudioClip hitSound1;//The sound of colliding with something
    public AudioClip collectSound;//The sound of being collected
    private Rigidbody coinRb;//The coin's rigidbody
    private Collider coinCollider;//The coin's collider
    private bool parentable = false;//If the object can be made the child of another object
    private bool pushable = false;//If the object can be pushed by a pusher object
    private CounterController gameManager;//The game manager
    private bool collected = false;//If the coin has been collected
    private bool silent = false;//If the coin can make sound or not
    private RaycastHit hit;//The raycast for coins
    public PhysicMaterial highFriction;//A high friction physic material
    public PhysicMaterial repelling;//A physic material that's low friction and bouncy
    public bool stuck = true;//If the coin is stuck in the machine
    private int stuckTimer;//How long to wait to consider the coin truly stuck in the machine
    private bool recreated = false;

    // Start is called before the first frame update
    void Start()
    {
        coinAudio = gameObject.GetComponent<AudioSource>();//Find the audiosource for the coin
        coinRb = gameObject.GetComponent<Rigidbody>();//Find the coin's rigidbody
        coinCollider = gameObject.GetComponent<Collider>();//Find the coin's collider
        coinCollider.material = highFriction;//Set the coin's physic material to high friction
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();//Find the game manager object  
        stuckTimer = 16;
        StartCoroutine(stuckPrevention());//Start the counter for detecting if the coin is stuck
    }

    // Update is called once per frame
    void Update()
    {
        raycastCheck();//Look beneath the coin to see what's there.

        //Unused devcode for stuck prevention
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
        if (!collision.gameObject.CompareTag("Coin") && !silent)//If colliding with something that isn't another coin
        {
            coinAudio.pitch = Random.Range(0.9f, 1.2f);
            coinAudio.PlayOneShot(hitSound1, 0.5f);//Play the hit sound at a random pitch
        }
        if (collision.gameObject.CompareTag("Pusher"))//If colliding with the pusher
        {
            if (pushable)
            {
                Vector3 awayFromPusher = collision.gameObject.transform.position - transform.position;
                coinRb.AddForce(awayFromPusher * 0.0005f, ForceMode.Impulse);//Push away from the pusher
            }
        }
        if (collision.gameObject.CompareTag("Floor"))//If colliding with the machine floor
        {
            parentable = false;
            transform.parent = null;//Stop being able to become the child of another object and disconnect from any parents
        }


    }

    void OnCollisionExit(Collision collision)//Upon no longer colliding with something
    {
        //Unused code for trying to remove parents
        //transform.parent = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collector"))//When entering a collector, add one to the bank
        {
            if (!gameManager.getCheatStatus())
            {
                gameManager.addCounter(1, "bank", "");
            }
            StartCoroutine("destroyDelay");
            collected = true;
        }
        if (other.CompareTag("Killbox"))//When entering a killbox, destroy the object
        {
            if (!collected)
            {
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("PusherTrigger") && !parentable)//When entering a pusher's trigger, if not the child of it, push away from it
        {
            Vector3 awayFromPusher = other.gameObject.transform.position - transform.position;
            coinRb.AddForce(awayFromPusher * 0.0005f, ForceMode.Impulse);
        }
        if (other.CompareTag("DropperExit"))//When exiting the dropdown part of the machine, change physic material and no longer be considered stuck
        {
            parentable = true;
            stuck = false;
            coinCollider.material = repelling;
        }
    }

    IEnumerator destroyDelay()//Destroy the object after a short delay, and play the collect sound if not silent
    {
        if (!silent)
        {
            coinAudio.PlayOneShot(collectSound, 0.8f);
        }
        yield return new WaitForSeconds(1.202f);
        Destroy(gameObject);
    }

    //Variable getters and setters
    public void setSilent(bool value)//Stop the object from making sound
    {
        silent = value;
    }

    public bool getSilent()
    {
        return silent;
    }

    public bool getParentable()//Get the parentable status
    {
        return parentable;
    }

    public void setParentable(bool parentableValue)//Set the parentable status
    {
        parentable = parentableValue;
    }

    public int getStuckTimer()
    {
        return stuckTimer;
    }

    public void setStuckTimer(int newValue)
    {
        stuckTimer = newValue;
    }

    public void setVelocity(Vector3 newVelocity)
    {

        coinRb.velocity = newVelocity;

    }

    public Vector3 getVelocity()
    {
        return coinRb.velocity;
    }

    public void setLocation(Vector3 newLocation)
    {
        transform.position = newLocation;
    }

    public Vector3 getLocation()
    {
        return transform.position;
    }

    public bool getStuck()
    {
        return stuck;
    }

    public void setStuck(bool newValue)
    {
        stuck = newValue;
    }

    public bool getPushable()
    {
        return pushable;
    }

    public void setPushable(bool newValue)
    {
        pushable = newValue;
    }

    public bool getRecreated()
    {
        return recreated;
    }

    public void setRecreated(bool newValue)
    {
        recreated = newValue;
    }

    public PhysicMaterial getMaterial()
    {
        return coinCollider.material;
    }

    public void setMaterial(PhysicMaterial newValue)
    {
        coinCollider.material = newValue;
    }

    public void setCollected(bool newValue)
    {
        collected = newValue;
    }

    public bool getCollected()
    {
        return collected;
    }

    public Quaternion getRotation()
    {        
        return transform.rotation;
    }

    public void setRotation(Quaternion rotation)
    {
        gameObject.transform.rotation = rotation;
    }

    void raycastCheck()//Call a raycast downwards
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            if (hit.collider.tag == "Pusher" && parentable)//If the pusher is below, and the object can be made a child of it, do so
            {
                transform.parent = hit.transform.parent;
            }
            if (hit.collider.tag == "Floor")//If the floor is below, stop being a child and be able to be pushed
            {
                transform.parent = null;
                pushable = true;
            }
        }
    }

    IEnumerator stuckPrevention()//If the coin is still in the machine after X seconds of being dropped, destroy it as it is likely stuck and return the coin to the player
    {
        while (stuckTimer > 0)
        {
            yield return new WaitForSeconds(1);
            stuckTimer = stuckTimer - 1;
        }
        if (stuck)
        {
            if (!gameManager.getCheatStatus())
            {
                gameManager.addCounter(1, "coins", "");
            }
            Destroy(gameObject);
        }
    }

    /**
    * Key Variables to gather:
    Vector3: Position
    Vector3: Velocity

    Bool: Silent
    Bool: Collected
    Bool: Stuck
    Bool: Parentable
    Bool: Recreated

    int: DestroyDelay

    PhysicMaterial: Material

    */

    public List<Vector3> gatherVectors()
    {
        List<Vector3> vectors = new List<Vector3> { getLocation(), getVelocity() };
        return vectors;
    }

    public List<bool> gatherBooleans()
    {
        List<bool> bools = new List<bool> { getSilent(), getCollected(), getRecreated(), getParentable(), getStuck() };
        return bools;
    }

    public void resetValues()
    {
        coinRb = gameObject.GetComponent<Rigidbody>();
        coinCollider = gameObject.GetComponent<Collider>();

    }

}
