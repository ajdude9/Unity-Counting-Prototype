using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallForward : MonoBehaviour
{

    private Rigidbody projRb;
    private Vector3 mousePos;
    public Vector3 worldPos;
    public float torque = 50;
    private int throwPower;
    public Renderer projRenderer;//The renderer for the projectile's current material
    private Color missColour = new Color(0.1f, 0.1f, 0.1f, 1.0f);
    private Color scoreColour = new Color(1f, 0.93f, 0f, 1.0f);
    private bool scored = false;
    public AudioClip hitSound1;
    public AudioClip hitSound2;
    public AudioClip hitSound3;
    public AudioClip hitSound4;
    public AudioClip convertSound;
    private AudioSource projAudio;
    private bool silent = false;
    private bool landed = false;
    private GameObject boxFloor;
    // Start is called before the first frame update
    void Start()
    {
        projRb = gameObject.GetComponent<Rigidbody>();
        projRenderer = gameObject.GetComponent<Renderer>();
        projAudio = gameObject.GetComponent<AudioSource>();
        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 5;
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        boxFloor = GameObject.Find("Box Floor");
        //Debug.Log("Calculated Power: " + (calculateThrowPower(mousePos) / 100));
        transform.position = new Vector3(15.6f, 3.2f, -0.22f);//Set the projectile to the bottom of the screen
        projRb.transform.LookAt(worldPos);//Look toward where the cursor is on the screen

        projRb.AddRelativeForce(projRb.transform.forward * calculateThrowPower(mousePos), ForceMode.Impulse);//Launch the projectile forwards
        projRb.AddRelativeForce(Vector3.up * (worldPos.y / 4), ForceMode.Impulse);//Give the projectile upwards force to lift it
        projRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);//Apply random torque to the projectile to make it spin

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator tilDeath(int lifetime)//Destroy the game object after a certain amount of time
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    float RandomTorque()//Generate a random torque value
    {
        return Random.Range(-torque, torque);
    }

    private void OnTriggerEnter(Collider other)//Upon touching a trigger
    {
        if (!landed)
        {
            if (other.CompareTag("Box"))
            {
                projRenderer.material.SetColor("_Color", scoreColour);
                projAudio.pitch = Random.Range(0.9f, 1.2f);
                projAudio.PlayOneShot(convertSound, 0.8f);
                Vector3 towardFloor = boxFloor.transform.position - transform.position;
                projRb.AddForce(towardFloor * 2, ForceMode.Impulse);
                scored = true;
                silent = true;
                StartCoroutine(tilDeath(30));//Destroy the projectile after a set time
            }
        }
    }

    void OnCollisionEnter(Collision collision)//Upon colliding with something
    {

        if (!silent)
        {
            projAudio.pitch = Random.Range(0.8f, 1.2f);
            switch (Random.Range(0, 4))
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
            landed = true;
            projRenderer.material.SetColor("_Color", missColour);
            projRenderer.material.SetFloat("_Metallic", 0f);
            projRenderer.material.SetFloat("_Glossiness", 0f);
            StartCoroutine(tilDeath(15));//Destroy the projectile after a set time
        }
    }


    float calculateThrowPower(Vector3 mousePosition)
    {
        float finalValue;
        float difference;
        if (mousePosition.x < 1260)
        {
            difference = 1260 - mousePosition.x;
        }
        else
        {
            difference = mousePosition.x - 1260;
        }
        finalValue = (1260 + (difference - difference / 2.5f)) / 90;
        return finalValue;
    }
}
