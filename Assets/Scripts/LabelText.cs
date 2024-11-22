using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelText : MonoBehaviour
{

    [SerializeField] private string text;//The text for the label
    // Start is called before the first frame update
    void Start()
    {

        GameObject label = new GameObject();//Create a new label gameobject
        
        TextMesh labelText = label.AddComponent<TextMesh>();//Add a textmesh to the label object
        labelText.text = text;//Set the text to what's stored in the text string
        labelText.fontSize = 20;//Set the font size
        labelText.transform.parent = this.transform;//Set the text to be a child of the label object
        labelText.transform.localEulerAngles += new Vector3(0, 180, 0);//Flip the text around
        labelText.transform.localPosition = new Vector3(0.3f, 0.25f, 0f);//Adjust the position
    }

    // Update is called once per frame
    void Update()
    {

    }
}
