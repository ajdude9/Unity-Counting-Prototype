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
    private DataManager dataManager;
    private bool silencedStatus = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        saveLoadCanvas = GameObject.Find("SaveSlotCanvas").GetComponent<Canvas>();
        saveLoadCanvas.enabled = false;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startGame(bool newGame)
    {
        if(newGame)
        {
            dataManager.setLFS(false);
        }        
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
        switch (currentView)
        {
            case "menu":
                mainCamera.transform.position = new Vector3(-25, 1, 15);
                mainCamera.transform.eulerAngles = new Vector3(0, -90, 0);
                saveLoadCanvas.enabled = true;
                currentView = "saves";
                silencedStatus = true;
                silence(silencedStatus);
                dataManager.loadSlots();
                
                break;
            case "saves":
                mainCamera.transform.position = new Vector3(0, 1, -10);
                mainCamera.transform.eulerAngles = new Vector3(0, 0, 0);
                saveLoadCanvas.enabled = false;
                currentView = "menu";
                silencedStatus = false;
                silence(silencedStatus);
                break;
        }
    }

    public void silence(bool silentStatus)//Silence all objects of a certain type, provided they have a function in their attached script to do so
    {
        GameObject[] allGems = GameObject.FindGameObjectsWithTag("Projectile");//Find all the projectiles and put them in an array
        foreach (GameObject gems in allGems)//For each game object in the array
        {
            MenuGemProjectile gem = gems.GetComponent<MenuGemProjectile>();//Set 'gem' to the BallForward script of the currently selected gem in the loop ("BallForward" is ProjectileController)
            if (!gem.getScored())//If the gem hasn't already been scored
            {
                gem.setSilent(silentStatus);//Set the silent status of the gem to the supplied value
            }
        }
    }

    public bool getSilencedStatus()
    {
        return silencedStatus;
    }

    public void loadGameSlot(int slot)//Load the game from a selected slot
    {
        dataManager.setCurrentProfile(slot);//Set the current profile to the selected slot
        startGame(false);//Start the game without it being a new game
    }

    public void continueGame()
    {
        dataManager.quickLoad();
        startGame(false);
    }
}
