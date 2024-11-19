using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelText : MonoBehaviour
{

    [SerializeField] private string text;
    // Start is called before the first frame update
    void Start()
    {

        GameObject label = new GameObject();
        
        TextMesh labelText = label.AddComponent<TextMesh>();
        labelText.text = text;
        labelText.fontSize = 20;
        labelText.transform.parent = this.transform;
        labelText.transform.localEulerAngles += new Vector3(0, 180, 0);
        labelText.transform.localPosition = new Vector3(0.3f, 0.25f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
