using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BallForward : MonoBehaviour
{

    private Rigidbody projRb;//The projectile's rigidbody
    private Vector3 mousePos;//Where the mouse is
    private Vector3 worldPos;//Where the mouse is relative to the world
    private float torque = 50;//The amount of torque to apply
    private int throwPower;//How much power to put behind a fired projectile
    private Renderer projRenderer;//The renderer for the projectile's current material
    private Color missColour = new Color(0.1f, 0.1f, 0.1f, 1.0f);//A dull black colour for when a projectile misses
    private Color scoreColour = new Color(1f, 0.93f, 0f, 1.0f);//A golden yellow colour for when a projectile is scored
    private bool scored = false;//If the projectile has already been scored
    // The audio clips for the sounds when the projectile hits something
    [SerializeField] private AudioClip hitSound1;
    [SerializeField] private AudioClip hitSound2;
    [SerializeField] private AudioClip hitSound3;
    [SerializeField] private AudioClip hitSound4;
    //The audio clip for when the projectile is scored
    [SerializeField] private AudioClip convertSound;
    private AudioSource projAudio;//The audiosource for the projectiles
    private bool silent = false;//If the projectile should make sound or not

    private bool landed = false;//If the projectile has landed
    private GameObject boxFloor;//The bottom face of the box, so the projectile knows where to aim itself

    private int scoreValue;//How valuable the gem is when scored
    private string projType;//The type of projectile to fire

    private CounterController gameManager;//The game controller

    private bool recreated;//If the projectile is new or loaded from a save

    private Dictionary<string, Material> materials;//A key:value list to store all the materials used for projectiles

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<CounterController>();//Find the game controller
        projType = gameManager.getProjectileType();//Get the currently selected projectile from the game controller
        projRb = gameObject.GetComponent<Rigidbody>();//Get the projectile's rigidbody
        writeMaterials();
        scoreValue = gameManager.getProjectileValue(projType);//Get the current projectile's value from the game manager by using the current projectile type
        projRenderer = gameObject.GetComponent<Renderer>();//Get the projectile's material renderer
        projRenderer.material = materials[projType];//Get the current material for the projectile from the game manager using the the material key:value list as reference                
        projAudio = gameObject.GetComponent<AudioSource>();//Get the projectile's audio source
        if (!recreated)
        {
            mousePos = Input.mousePosition;//The current mouse position is the literal position on the screen
            mousePos.z = Camera.main.nearClipPlane + 5;//Set the mouse position's z axis to be slightly forward, for accuracy purposes
            worldPos = Camera.main.ScreenToWorldPoint(mousePos);//Figure out where the mouse is actually pointing in the 3D environment
            boxFloor = GameObject.Find("Box Floor");//Find the box's bottom face
            //Debug.Log("Calculated Power: " + (calculateThrowPower(mousePos) / 100));
            transform.position = new Vector3(15.6f, 3.2f, -0.22f);//Set the projectile to the bottom of the screen
            projRb.transform.LookAt(worldPos);//Look toward where the cursor is on the screen

            fire();
        }

    }

    void fire()
    {

        projRb.AddRelativeForce(projRb.transform.forward * calculateThrowPower(mousePos), ForceMode.Impulse);//Launch the projectile forwards
        projRb.AddRelativeForce(Vector3.up * (worldPos.y / 4), ForceMode.Impulse);//Give the projectile upwards force to lift it
        projRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);//Apply random torque to the projectile to make it spin
        recreated = true;


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void writeMaterials()
    {
        materials = new Dictionary<string, Material>()//Set up the material list of gem materials
        {
            {"ruby", Resources.Load("Ruby", typeof(Material)) as Material},
            {"emerald", Resources.Load("Emerald", typeof(Material)) as Material},
            {"amethyst", Resources.Load("Amethyst", typeof(Material)) as Material},
            {"diamond", Resources.Load("Diamond", typeof(Material)) as Material},
        };
    }

    IEnumerator tilDeath(int lifetime)//Destroy the game object after a certain amount of time
    {
        yield return new WaitForSeconds(lifetime);//Wait for the amount of seconds based on the given lifetime variable. This varies between scores and misses.
        Destroy(gameObject);//Destroy it
    }

    float RandomTorque()//Generate a random torque value
    {
        return Random.Range(-torque, torque);//The random torque value is anywhere between the negative amount of torque and its positive counterpart (i.e. -50 to 50)
    }

    private void OnTriggerEnter(Collider other)//Upon touching a trigger
    {
        if (!landed)//If the projectile hasn't already landed
        {
            if (other.CompareTag("Box"))//If lands in the box's trigger zone
            {
                //Debug.Log("Box trigger activated.");
                projRenderer.material.SetColor("_Color", scoreColour);//Set the colour to the score colour
                if (!silent)//If its not been silenced
                {
                    projAudio.pitch = Random.Range(0.9f, 1.2f);
                    projAudio.PlayOneShot(convertSound, 0.8f);//Play the sound of being scored
                }
                Vector3 towardFloor = boxFloor.transform.position - transform.position;//Aim towards the bottom of the box
                projRb.AddForce(towardFloor * 2, ForceMode.Impulse);//Throw the projectile to the bottom of the box, to prevent it from spilling out
                scored = true;//State that the projectile has been scored
                silent = true;//Make the projectile silent so it stops making noise after being scored
                StartCoroutine(tilDeath(30));//Destroy the projectile after a set time  
                //Debug.Log("Scored, adding " + scoreValue + " coins.");
                gameManager.addCounter(scoreValue, "coins", "");//Add the value of the projectile to the amount of coins the player can drop
            }
        }
    }

    void OnCollisionEnter(Collision collision)//Upon colliding with something
    {

        if (!silent)//If its not silent
        {
            projAudio.pitch = Random.Range(0.8f, 1.2f);//Set the pitch of the sound effects to a random value
            switch (Random.Range(0, 4))//Pick a random sound effect to play out of a potential four
            {
                case 0:
                    projAudio.PlayOneShot(hitSound1, 0.5f);
                    break;
                case 1:
                    projAudio.PlayOneShot(hitSound2, 0.5f);
                    break;
                case 2:
                    projAudio.PlayOneShot(hitSound3, 0.5f);
                    break;
                case 3:
                    projAudio.PlayOneShot(hitSound4, 0.5f);
                    break;
            }
        }
        if (collision.gameObject.CompareTag("Floor") && !scored)//If the projectile touches the floor and hasn't been scored
        {
            landed = true;//State the projectile has landed
            projRenderer.material.SetColor("_Color", missColour);//Set the colour to the missed colour
            projRenderer.material.SetFloat("_Metallic", 0f);//Remove all metallic from the projectile
            projRenderer.material.SetFloat("_Glossiness", 0f);//Remove the projectile's glossiness
            StartCoroutine(tilDeath(15));//Destroy the projectile after a set time
        }
    }


    float calculateThrowPower(Vector3 mousePosition)//Calculate the throw power of the projectile based on the cursor's X position
    {
        float finalValue;//Define the final calculated final value
        float difference;//Define the difference in power
        if (mousePosition.x < 1260)//If the mouse position is on the left side of the middle of the scren
        {
            difference = 1260 - mousePosition.x;//Calculate the difference with the initial power being the higher value
        }
        else
        {
            difference = mousePosition.x - 1260;//Calculate the difference with the mouse position being the higher value
        }
        finalValue = (1260 + (difference - difference / 2.5f)) / 90;//The final value is the middle of the screen plus the difference minus the difference divided by 2.5, then divided by 90
        return finalValue;//Return the final value
    }

    public void setSilent(bool value)//Make the projectile either silent or able to make noise
    {
        silent = value;
    }


    public bool getSilent()
    {
        return silent;
    }

    public void setScored(bool newValue)
    {
        scored = newValue;
    }

    public bool getScored()//Return if the projectile has been scored or not
    {
        return scored;
    }

    public void setRecreated(bool newValue)
    {
        recreated = newValue;
    }

    public bool getRecreated()
    {
        return recreated;
    }

    public void setVelocity(Vector3 newVelocity)
    {
        try
        {
            projRb.velocity = newVelocity;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("An error has ocurred when attemtping to assign Velocity.");
            Debug.Log("The error is: " + e);
            Debug.Log("Printing Values: ");
            Debug.Log("Silent: " + getSilent());
            Debug.Log("Scored: " + getScored());
            Debug.Log("Recreated: " + getRecreated());
            Debug.Log("Location: " + getLocation());
            Debug.Log("Velocity: " + getVelocity());
            Debug.Log("Type: " + getProjType());
        }
    }

    public Vector3 getVelocity()
    {
        return projRb.velocity;
    }

    public void setLocation(Vector3 newLocation)
    {
        transform.position = newLocation;
    }

    public Vector3 getLocation()
    {
        return transform.position;
    }

    public void setTorque(Vector3 newTorque)
    {
        projRb.AddTorque(newTorque);
    }
    /**
    public Vector3 getTorque()
    {
        ¯\_(ツ)_/¯
    }
    */
    public void setProjType(string newMaterial)
    {
        projType = newMaterial;
        projRenderer.material = materials[newMaterial];
    }

    public string getProjType()
    {
        return projType;
    }

    /**
    * Essential Values:
    * Silent (Boolean) - Whether or not it can make noise
    * Scored (Boolean) - Whether or not it has been scored
    * Recreated (Boolean) - Whether or not this is the original or it has been reloaded
    * Velocity (Vector3) - How fast its moving and in which direction it's going
    * Location (Vector3) - Where it is in the world
    * ProjType (string) - What type of gem it is (Ruby, Emerald, Amethyst or Diamond)
    */

    public List<bool> gatherBooleans()
    {
        List<bool> booleans = new List<bool> { getSilent(), getScored(), getRecreated() };
        return booleans;
    }

    public List<Vector3> gatherVectors()
    {
        List<Vector3> vectors = new List<Vector3> { getLocation(), getVelocity() };
        return vectors;
    }


    public void sanityCheck()
    {
        Debug.Log("Successful Check.");
    }

    public string sanityCheckReturn()
    {
        return "Successful Check.";
    }

    public void resetValues()
    {
        projRb = gameObject.GetComponent<Rigidbody>();
        projRenderer = gameObject.GetComponent<Renderer>();
        writeMaterials();
    }

}
