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
    private Color missColour = new Color(0.6f, 0.6f, 0.6f, 1.0f);
    private Color scoreColour = new Color(1f, 0.93f, 0f, 1.0f);
    private bool scored = false;    
    // Start is called before the first frame update
    void Start()
    {                
        projRb = gameObject.GetComponent<Rigidbody>();
        projRenderer = gameObject.GetComponent<Renderer>();
        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 5;
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.Log("Calculated Power: " + (calculateThrowPower(mousePos) / 100));
        transform.position = new Vector3(15.6f, 3.2f, -0.22f);//Set the projectile to the bottom of the screen
        projRb.transform.LookAt(worldPos);//Look toward where the cursor is on the screen

        projRb.AddRelativeForce(projRb.transform.forward * (calculateThrowPower(mousePos) / 100), ForceMode.Impulse);//Launch the projectile forwards
        projRb.AddRelativeForce(Vector3.up * (worldPos.y / 2) , ForceMode.Impulse);//Give the projectile upwards force to lift it
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
        projRenderer.material.SetColor("_Color", scoreColour);
        scored = true;
        StartCoroutine(tilDeath(5));//Destroy the projectile after a set time
    }

    void OnCollisionEnter(Collision collision)//Upon colliding with something
    {
        if(collision.gameObject.CompareTag("Floor") && !scored)//If the projectile touches the floor and hasn't been scored
        {
            projRenderer.material.SetColor("_Color", missColour);
            StartCoroutine(tilDeath(15));//Destroy the projectile after a set time
        }
    }
    
    float calculateThrowPower(Vector3 mousePosition)
    {
        if(mousePosition.x < 1260)
        {
            float difference = 1260 - mousePosition.x;
            return 1260 + (difference - difference / 2);
        }
        else
        {
            float difference = mousePosition.x - 1260;
            return 1260 + (difference - difference / 2);
        }
    }
}
