using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    private string currentView = "menu";
    private Camera mainCamera;
    private Canvas saveLoadCanvas;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();        
        saveLoadCanvas = GameObject.Find("SaveSlotCanvas").GetComponent<Canvas>();
        saveLoadCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNew()
    {
        SceneManager.LoadScene(1);//Load the main scene
    }

    public void changeScene(int scenePos)
    {
        SceneManager.LoadScene(scenePos);
    }

    public void exit()
    {
        #if UNITY_EDITOR//Specialised compiler if statement to detected editor mode
            EditorApplication.ExitPlaymode();//Exit playmode if viewing the editor
        #else
            Application.Quit();//Quit the game and close the window
        #endif
        
    }

    public void switchViews()
    {
        switch(currentView)
        {
            case "menu":
                mainCamera.transform.position = new Vector3(-25, 1, 15);
                mainCamera.transform.eulerAngles = new Vector3(0, -90, 0);
                saveLoadCanvas.enabled = true;
                currentView = "saves";
            break;
            case "saves":
                mainCamera.transform.position = new Vector3(0, 1, -10);
                mainCamera.transform.eulerAngles = new Vector3(0, 0, 0);
                saveLoadCanvas.enabled = false;
                currentView = "menu";
            break;
        }
    }
}
