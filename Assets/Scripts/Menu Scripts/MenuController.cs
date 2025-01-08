using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNew()
    {
        SceneManager.LoadScene(0);//Load the main scene
    }

    public void changeScene(int scenePos)
    {
        SceneManager.LoadScene(scenePos);
    }

    public void Exit()
    {
        #if UNITY_EDITOR//Specialised compiler if statement to detected editor mode
            EditorApplication.ExitPlaymode();//Exit playmode if viewing the editor
        #else
            Application.Quit();//Quit the game and close the window
        #endif
        
    }
}
