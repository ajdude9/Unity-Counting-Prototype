using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Unity.VisualScripting.Dependencies.NCalc;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class CounterController : MonoBehaviour
{
    private CoinDropController coinDropController;//The coin drop controller
    [SerializeField] private Text counterText;//Text object to display the number of projectiles
    [SerializeField] private Text loadedText;//Text object to display the number of projectiles ready
    private TextMeshProUGUI reloadNotify;//Text object to tell the player to reload
    public TextMeshProUGUI reloadNotifyPub
    {
        get { return reloadNotify; }
    }

    private TextMeshProUGUI coinsSavedText;//The text saying how many coins the player has saved
    private TextMeshProUGUI coinsDroppableText;//The text saying how many coins the player can drop
    private int loadedTotal;//The number of projectiles ready to fire, kept privately
    public int loadedTotalPub//A publically accessible variable for loaded total
    {
        get { return loadedTotal; }//Getter for loaded total
        set//Setter for loaded total
        {
            if (value < 0)//Ensure that loaded total cannot be set to a negative number
            {
                Debug.LogError("loadedTotal cannot be negative.");
            }
            else
            {
                loadedTotal = value;
            }
        }
    }
    private int reloadMax;//The total number of projectiles that can be loaded at once.
    private int coinsSaved = 0;//The number of coins the player has 'banked'
    private int coinsDroppable = 0;//The number of coins the player can drop.
    private float reloadSpeed;//How long it takes to reload one projectile
    private float fireRate;//How fast the player can autofire projectiles
    public float fireRatePub
    {
        get { return fireRate; }
        set
        {
            if (value < 0)
            {
                Debug.LogError("fireRate cannot be negative.");
            }
            else
            {
                fireRate = value;
            }
        }

    }
    private string projName;//The name of the projectiles being fired
    private bool reloadingStatus = false;//Whether the player is currently 'reloading'
    public bool reloadingStatusPub
    {
        get { return reloadingStatus; }
    }
    private bool enableCheats = false;//Whether cheats are enabled
    private AudioSource gameAudio;//Source for the universal game audio
    [SerializeField] private AudioClip reloadSound;//Reloading sound (fully reloaded)
    [SerializeField] private AudioClip reloadSingle;//Reloading sound (single projectile)
    [SerializeField] private AudioClip reloadFail;//Reloading fail sound (no ammo)
    [SerializeField] private AudioClip cheater;//Cheating sound
    private Camera throwCamera;//The camera for throwing projectiles into a box
    private Camera coinCamera;//The camera for putting coins in a machine
    private Camera shopCamera;//The camera for viewing the shop
    private Camera gemSelectCamera;//The camera for viewing the gem select UI
    private Camera pauseCamera;//The camera for viewing the pause menu
    private Camera saveLoadCamera;//The camera for viewing the save/load screen
    private string viewType;//Which camera is currently being viewed
    public string viewTypePub
    {
        get { return viewType; }

    }
    private string lastView;//The camera that was viewed last when paused
    private bool firstSwitch = false;//Whether the switch to the coin camera is the first one since the game started
    private int fadeValue;//The opacity of the reload notification text
    public int fadeValuePub
    {
        get { return fadeValue; }
        set
        {
            if (value < 0 || value > 1)
            {
                Debug.LogError("fadeValue cannot be less than zero or greater than one.");
            }
            else
            {
                fadeValue = value;
            }
        }
    }
    private string projectileType;//The type of projectile that has been selected
    private Dictionary<string, int> heldProjectiles;//A key:value list to contain the number of a projectile and its name
    private Dictionary<string, int> projectileValues;//A key:value list to contain the name of a projectile and its value in coins when scored
    private Dictionary<string, GameObject> gemObjects;//A key:value list to contain all the gem objects used in the scene
    private Dictionary<string, TextMeshProUGUI> inventoryText;//A key:value list to contain the text of how many gems the player holds and their values
    private string[] gemNames;//An array containing the names all gems that exist
    private Color emptyColour = new Color(0.0f, 0.0f, 0.0f, 0f);//A black colour
    private UnityEngine.UI.Button changeButton;//The button for opening the menu for changing gems
    private UnityEngine.UI.Button rubyButton;//The button for changing to rubies
    private UnityEngine.UI.Button emeraldButton;//The button for changing to emeralds
    private UnityEngine.UI.Button amethystButton;//The button for changing to amethysts
    private UnityEngine.UI.Button diamondButton;//The button for changing to diamonds
    private UnityEngine.UI.Button[] inventoryButtons;//An array containing each button for ease of access (unused due to buggy behaviour)
    private Dictionary<string, Material> materials;//A key:value list to store all the materials used for projectiles
    private Canvas pauseMenu;
    private Canvas saveSlotCanvas;
    private DataManager dataManager;
    private string saveOrLoad;//Whether or not the player is saving or loading with the saveLoad function
    private int selectedSlot;//The currently selected save slot; '0' indicates a new game.
    [SerializeField] private GameObject projectilePrefab;//The prefab that holds the projectile
    [SerializeField] private GameObject coinPrefab;//The prefab that holds the coin


    void Awake()//As soon as we start
    {
        try
        {
            verifyScene();//See if we have properly entered the scene from the menu
        }
        catch (NullReferenceException)//If not, and DataManager has not been created
        {
            SceneManager.LoadScene(0);//Return to the menu so the game can be properly started
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        findObjects();//Find all the objects in the scene and tie to them to their respective variables
        dataManager.refresh();
        selectedSlot = dataManager.getCurrentProfile();
        //get the game audio and set the default values for variables
        projectileType = "ruby";//Set the default projectile to ruby
        projName = "Gems";
        writeDictionaries();//Fill out all the dictionary variables.               
        loadStats();
        
        //counterText.enabled = true;//Enable the counter text
        //loadedText.enabled = true;//Enable the loaded text
        counterText.text = "Available " + projName + ": " + heldProjectiles[projectileType];//Change the counter text to show how many of the currently selected projectile type the player has
        loadedText.text = "Loaded " + projName + ": " + loadedTotal;//Change the loaded text to show how many of the currently loaded projectile are loaded
        coinsSavedText.text = "Coins Won: " + coinsSaved;//Change the coins saved text to show how many coins the player has won
        coinsDroppableText.text = "Droppable Coins: " + coinsDroppable;//Change the coins droppable text to show how many coins the player can drop
        reloadNotify.color = new Color(reloadNotify.color.r, reloadNotify.color.g, reloadNotify.color.b, 0);//Chane the reload notify colour to red
        pauseCamera.enabled = false;
        pauseMenu.enabled = false;
        saveSlotCanvas.enabled = false;

        if (!dataManager.getLFS())//If we haven't loaded from a saved game
        {
            switchToThrow();//Switch to the throw viewtype, if it wasn't already being viewed.
        }



    }

    void verifyScene()//Check to see if the scene has been entered properly through the creation of the DataManager
    {

        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();//Find the data manager object

    }

    void loadStats()
    {
        loadedTotal = 6;//Set the total amount of projectiles the player currently has loaded to 6
        reloadMax = 6;//Set the maximum amount of projectiles that can be reloaded to 6
        reloadSpeed = 0.15f;//Set the reload speed to 0.15
        fireRate = 0.35f;//Set the firerate to 0.35
        if (selectedSlot == 0)
        {
            setCounter(54, "total", "ruby");
        }
        else
        {
            saveOrLoad = "load";
            saveLoad(selectedSlot);
        }
    }

    void findObjects()
    {
        gameAudio = gameObject.GetComponent<AudioSource>();//Find the game's audio source
        coinDropController = GameObject.Find("Coin Dropper").GetComponent<CoinDropController>();//Find the coin drop controller
        changeButton = GameObject.Find("Change Button").GetComponent<UnityEngine.UI.Button>();//The button for changing gems
        rubyButton = GameObject.Find("Ruby Select Button").GetComponent<UnityEngine.UI.Button>();//The button for changing to rubies
        emeraldButton = GameObject.Find("Emerald Select Button").GetComponent<UnityEngine.UI.Button>();//The button for changing to emeralds
        amethystButton = GameObject.Find("Amethyst Select Button").GetComponent<UnityEngine.UI.Button>();//The button for changing to amethysts
        diamondButton = GameObject.Find("Diamond Select Button").GetComponent<UnityEngine.UI.Button>();//The button for changing to diamonds
        throwCamera = GameObject.Find("Main Camera").GetComponent<Camera>();//Find the camera for viewing the box to throw gems into
        coinCamera = GameObject.Find("Machine Watcher").GetComponent<Camera>();//Find the camera for viewing the coin pushing machine
        shopCamera = GameObject.Find("Shop Camera").GetComponent<Camera>();//Find the camera for viewing the shop
        gemSelectCamera = GameObject.Find("Select Camera").GetComponent<Camera>();//Find the camera for changing the currently selected gem
        pauseCamera = GameObject.Find("Pause Camera").GetComponent<Camera>();//Find the camera for viewing the pause menu
        saveLoadCamera = GameObject.Find("SaveLoadCamera").GetComponent<Camera>();//Find the camera for viewing the save/load menu
        pauseMenu = GameObject.Find("Pause Canvas").GetComponent<Canvas>();//Find the canvas for the pause menu        
        saveSlotCanvas = GameObject.Find("SaveSlotCanvas").GetComponent<Canvas>();//Find the save/load canvas
        counterText = GameObject.Find("Ammo Total").GetComponent<Text>();//Find the text for how many projectiles the player has
        loadedText = GameObject.Find("Ammo Loaded").GetComponent<Text>();//Find the text for how many projectiles the player has that are loaded
        coinsDroppableText = GameObject.Find("Coins Droppable").GetComponent<TextMeshProUGUI>();//Find the text for how many coins the player can drop
        coinsSavedText = GameObject.Find("Coins Banked").GetComponent<TextMeshProUGUI>();//Find the text for how many coins the player can spend
        reloadNotify = GameObject.Find("Reload Notification").GetComponent<TextMeshProUGUI>();//Find the text for the notification that the player needs to reload

    }

    void writeDictionaries()//Set up the dictionary variables
    {
        string[] gemNameList = { "ruby", "emerald", "amethyst", "diamond" };
        gemNames = gemNameList;
        heldProjectiles = new Dictionary<string, int>()//Set up each projectile the player can hold, and give the player 54 rubies to start (with 6 already loaded, for 60 total)
        {
            {"ruby", 0},
            {"emerald", 0},
            {"amethyst", 0},
            {"diamond", 0}
        };
        projectileValues = new Dictionary<string, int>()//Set up each projectile's value when scored
        {
            {"ruby", 1},
            {"emerald", 2},
            {"amethyst", 5},
            {"diamond", 20}
        };
        gemObjects = new Dictionary<string, GameObject>()//Find each display gem object in the UI and the shop
        {
            {"UIRuby", GameObject.Find("UIRuby")},
            {"ShopRuby", GameObject.Find("Shop Ruby")},
            {"UIEmerald", GameObject.Find("UIEmerald")},
            {"ShopEmerald", GameObject.Find("Shop Emerald")},
            {"UIAmethyst", GameObject.Find("UIAmethyst")},
            {"ShopAmethyst", GameObject.Find("Shop Amethyst")},
            {"UIDiamond", GameObject.Find("UIDiamond")},
            {"ShopDiamond", GameObject.Find("Shop Diamond")}
        };
        inventoryText = new Dictionary<string, TextMeshProUGUI>()//Find each text object in the gem switch UI, stating how many the player has and how much each is worth when scored
        {
            {"rubyHeld", GameObject.Find("Ruby Held").GetComponent<TextMeshProUGUI>()},
            {"rubyValue", GameObject.Find("Ruby Value").GetComponent<TextMeshProUGUI>()},
            {"emeraldHeld", GameObject.Find("Emerald Held").GetComponent<TextMeshProUGUI>()},
            {"emeraldValue", GameObject.Find("Emerald Value").GetComponent<TextMeshProUGUI>()},
            {"amethystHeld", GameObject.Find("Amethyst Held").GetComponent<TextMeshProUGUI>()},
            {"amethystValue", GameObject.Find("Amethyst Value").GetComponent<TextMeshProUGUI>()},
            {"diamondHeld", GameObject.Find("Diamond Held").GetComponent<TextMeshProUGUI>()},
            {"diamondValue", GameObject.Find("Diamond Value").GetComponent<TextMeshProUGUI>()},
        };
        materials = new Dictionary<string, Material>()//Find and load each material for each gem
        {
            {"ruby", Resources.Load("Ruby", typeof(Material)) as Material},
            {"emerald", Resources.Load("Emerald", typeof(Material)) as Material},
            {"amethyst", Resources.Load("Amethyst", typeof(Material)) as Material},
            {"diamond", Resources.Load("Diamond", typeof(Material)) as Material},
        };
    }
    // Update is called once per frame

    void Update()
    {
        keycodeController();//Control the inputs
        if (loadedTotal == reloadMax)//If the player's loaded projectiles matches the maximum amount that can be loaded
        {
            textFadeOut(1, reloadNotify);//Fade out the reload notification text, if it's there
        }
    }

    private void keycodeController()//Control the player's inputs
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))//If the player hits the grave key or "backquote" key
        {
            infiniteAmmoCheat();//Enable cheats, giving the player infinite ammo, coins, and spendable coins
        }
        if (Input.GetKeyDown(KeyCode.V))//Switch the camera view forwards along the list
        {
            switch (viewType)//Based on the current viewtype
            {
                case "throw"://If looking at the throw view, move rightwards to the coin pusher
                    switchToCoin();
                    break;
                case "coin"://If looking at the coin view, move rightwards to the shop
                    switchToShop();
                    break;
                case "shop"://If looking at the shop view, move rightwards to the box throwing view
                    switchToThrow();
                    break;
            }
            refreshCounter();//Refresh the UI elements to update them
        }
        if (Input.GetKeyDown(KeyCode.C))//Switch the camera view backwards along the list
        {
            switch (viewType)//See above, simply moves the other direction
            {
                case "throw":
                    switchToShop();
                    break;
                case "coin":
                    switchToThrow();
                    break;
                case "shop":
                    switchToCoin();
                    break;
            }
            refreshCounter();//See above
        }
        if (Input.GetKeyDown(KeyCode.R))//If the player hits R to reload
        {
            fadeValue = 0;//Set the fade value of the text to 0
            StartCoroutine(textFadeOut(1f, reloadNotify));//Start to fade out the reload text
            reload();//Reload the projectiles to the maximum amount
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (viewType != "paused")
            {
                toggleGamePause(true);
            }
            else
            {
                if (saveSlotCanvas.enabled)
                {
                    toggleGamePause(true);
                }
                else
                {
                    toggleGamePause(false);
                }
            }
        }
    }

    public void refreshCounter()//Update the counters to their current values
    {
        if (enableCheats)//If cheats are enabled
        {
            counterText.text = "Available " + projName + ": ∞";//Set them all to the infinity symbol - since they are effectively infinite
            loadedText.text = "Loaded " + projName + ": ∞";
            coinsDroppableText.text = "Droppable Coins: ∞";
            if (viewType == "shop")//If the player is looking at the shop
            {
                coinsSavedText.text = "Coins Available: ∞";//Say the coins won from the pusher are available
            }
            else
            {
                coinsSavedText.text = "Coins Won: ∞";//Say the coins won from the pusher are won
            }
        }
        else
        {
            counterText.text = "Available " + projName + ": " + heldProjectiles[projectileType];//Show how many projectiles are available to be loaded
            loadedText.text = "Loaded " + projName + ": " + loadedTotal;//Show how many projectiles are loaded and able to be fired
            coinsDroppableText.text = "Droppable Coins: " + coinsDroppable;//Show how many coins can be dropped
            if (viewType == "shop")
            {
                coinsSavedText.text = "Coins Available: " + coinsSaved;//Say the coins won from the pusher are available
            }
            else
            {
                coinsSavedText.text = "Coins Won: " + coinsSaved;//Say the coins won from the pusher are won
            }
        }
    }

    public int getCounter(String type, String gemType)//Get a specific counter
    {
        int returnAmount = 0;//Return 0 as a failsafe
        switch (type)//Based on the type input
        {
            case "total"://Get the total amount of projectiles
                returnAmount = heldProjectiles[gemType];//Use the supplied gem type to get the specific projectile
                break;
            case "loaded"://Get the amount of projectiles able to be fired
                returnAmount = loadedTotal;
                break;
            case "coins"://Get the total amount of coins that can be dropped into the machine
                returnAmount = coinsDroppable;
                break;
            case "bank"://Get the total amount of coins won from the machine, but cannot be dropped
                returnAmount = coinsSaved;
                break;
        }
        return returnAmount;
    }

    public void setCounter(int amount, String type, String gemType)//Set a counter to a specific value
    {
        switch (type)
        {
            case "total":
                heldProjectiles[gemType] = amount;
                refreshCounter();
                break;
            case "loaded":
                loadedTotal = amount;
                refreshCounter();
                break;
            case "coins":
                coinsDroppable = amount;
                refreshCounter();
                break;
            case "bank":
                coinsSaved = amount;
                refreshCounter();
                break;
        }
    }

    public void addCounter(int amount, String type, String gemType)//Add to a counter a specific value
    {
        //Debug.Log("addCounter asked to add " + amount + " of type " + type);        
        switch (type)
        {
            case "total":
                heldProjectiles[gemType] += amount;
                refreshCounter();
                break;
            case "loaded":
                loadedTotal += amount;
                refreshCounter();
                break;
            case "coins":
                //Debug.Log("Current coins droppable: " + coinsDroppable);
                coinsDroppable += amount;
                //Debug.Log("New Coins Droppable: " + coinsDroppable);
                refreshCounter();
                break;
            case "bank":
                coinsSaved += amount;
                refreshCounter();
                break;
        }
    }

    public void minusCounter(int amount, String type, String gemType)//Remove a specific value from a specific counter
    {
        switch (type)
        {
            case "total":
                heldProjectiles[gemType] -= amount;
                refreshCounter();
                break;
            case "loaded":
                loadedTotal -= amount;
                refreshCounter();
                break;
            case "coins":
                coinsDroppable -= amount;
                refreshCounter();
                break;
            case "bank":
                coinsSaved -= amount;
                refreshCounter();
                break;
        }
    }

    public void reload()//Reload projectiles
    {

        if (!reloadingStatus && !enableCheats)//If the player isn't already reloading and not cheating
        {
            int amountToReload = reloadMax - loadedTotal;//The amount of projectiles to reload is the maximum allowed, minus however many are already loaded
            if (amountToReload > 0 && getCounter("total", projectileType) > 0)
            {
                reloadingStatus = true;
                if (amountToReload > getCounter("total", projectileType))//If there isn't enough stored projectiles to reload fully
                {
                    amountToReload = getCounter("total", projectileType);//Instead reload however many are left
                }

                StartCoroutine(reloadTimer(amountToReload));//Start to reload a set number of projectiles with a delay between each reload
            }
            else//If the player is out of ammo to load, or is fully loaded
            {
                gameAudio.PlayOneShot(reloadFail, 0.6f);//Play a sound to notify them of this
            }
        }
        else
        {
            if (enableCheats)//If the player is cheating
            {
                gameAudio.PlayOneShot(reloadFail, 0.6f);//Play the reload failure sound - as they don't need to reload due to having infinite ammo loaded
            }
        }
    }

    public void quickReload()//Quickly full-reload
    {
        if (!reloadingStatus)//If the player isn't already reloading
        {
            int amountToReload = 0;//Set the amount to reload to 0 as default
            if (getCounter("total", projectileType) > reloadMax)//If the player has more ammo stored than they can load at once
            {
                amountToReload = reloadMax;//Reload the maximum amount
            }
            else
            {
                if (getCounter("total", projectileType) > 0)//If the player doesn't have enough to full reload, but does have ammo
                {
                    amountToReload = getCounter("total", projectileType);//Reload all the ammo left
                }
            }
            if (amountToReload > 0)//If the player has ammo to reload
            {
                addCounter(amountToReload, "loaded", "");//Add the amount to reload to the loaded variable
                minusCounter(amountToReload, "total", projectileType);//Remove the amount to reload from the total variable
            }
        }

    }

    private IEnumerator reloadTimer(int reloadAmount)//Reload projectiles on a timer rather than all at once
    {

        for (int i = 0; i < reloadAmount; i++)//For each projectile to reload
        {

            yield return new WaitForSeconds(reloadSpeed);//Delay based on reload speed
            addCounter(1, "loaded", "");//Add one to the loaded counter
            minusCounter(1, "total", projectileType);//Remove from from the total counter
            gameAudio.PlayOneShot(reloadSingle, 0.8f);//Play a sound for reloading a single projectile

        }
        yield return new WaitForSeconds(0.1f);
        if (reloadingStatus)//If the player is indeed reloading
        {
            gameAudio.PlayOneShot(reloadSound, 0.8f);//Play the sound for being fully reloaded
            reloadingStatus = false;//State the player is no longer reloading
        }
        yield return null;
    }

    private void infiniteAmmoCheat()//Enable cheats, setting status to immensely high values; dev function used for debugging purposes.
    {
        if (!enableCheats)//If cheats aren't already enabled
        {
            gameAudio.PlayOneShot(cheater);//Tell the player 'You cheated!'
        }
        enableCheats = true;//Set the cheat value to true
        reloadSpeed = 0.01f;//Set the reload value to 0.01 (outdated value; no longer necessary)
        fireRate = 0.05f;//Set the firerate to 0.05 (any faster and gems start to spawn inside of each other and go crazy)
        refreshCounter();//Refresh all the counters to set their values to be infinite
        updateInventory();//Update the inventory to do the same
    }

    private void switchToCoin()//Switch to the coin view
    {
        //Disable all the cameras except the coin camera
        //-
        throwCamera.enabled = false;
        coinCamera.enabled = true;
        shopCamera.enabled = false;
        //-
        viewType = "coin";//Set the viewtype to be coin
        //Disable all UI elements except the relevant coin drop text
        //-
        counterText.enabled = false;
        loadedText.enabled = false;
        coinsSavedText.enabled = true;
        coinsDroppableText.enabled = true;
        changeButton.gameObject.SetActive(false);
        //-
        //coinsSavedText.transform.position = new Vector3(coinsSavedText.transform.position.x, transform.position.y, coinsSavedText.transform.position.z);//Adjust the coins saved text's position
        toggleInventoryButtons(false);//Disable the inventory buttons so they can't be clicked
        if (!firstSwitch)//If this is the first time the player is switching to this view
        {
            firstSwitch = true;//Say they've now switched to the view once
            coinDropController.generateCoins(coinDropController.getCoinSpawnAmount());//Generate a bunch of coins for the machine
        }
        silence("coin", false);//Unsilence all coins
        silence("gem", true);//Silence all gems


    }

    public void switchToThrow()//Switch to the throw view
    {
        if (viewType == "ammo")//If the player was last looking at the ammo view
        {
            quickReload();//Quickly reload their ammo to the maximum possible
        }
        //Same as switchToCoin
        throwCamera.enabled = true;
        gemSelectCamera.enabled = false;
        coinCamera.enabled = false;
        shopCamera.enabled = false;
        counterText.enabled = true;
        loadedText.enabled = true;
        coinsSavedText.enabled = false;
        coinsDroppableText.enabled = false;
        changeButton.gameObject.SetActive(true);
        viewType = "throw";
        toggleInventoryButtons(false);
        silence("coin", true);
        silence("gem", false);
    }

    private void switchToShop()//Switch to the shop view
    {
        //Same as switchToCoin
        throwCamera.enabled = false;
        coinCamera.enabled = false;
        shopCamera.enabled = true;
        counterText.enabled = false;
        loadedText.enabled = false;
        coinsSavedText.enabled = true;
        coinsDroppableText.enabled = false;
        changeButton.gameObject.SetActive(false);
        //coinsSavedText.transform.position = new Vector3(coinsSavedText.transform.position.x, transform.position.y, coinsSavedText.transform.position.z);
        coinsSavedText.text = "Coins Available: " + coinsSaved;
        viewType = "shop";
        toggleInventoryButtons(false);
        silence("coin", true);
        silence("gem", true);
    }

    public void switchToAmmo()//Switch to the ammo view
    {
        if (!reloadingStatus)//If the player isn't actively reloading (to prevent quickReload bugs and cheese)
        {
            gemSelectCamera.enabled = true;//Enable the gem selection camera on top of the throw view camera
            counterText.enabled = false;//Disable the total and loaded texts
            loadedText.enabled = false;
            changeButton.gameObject.SetActive(false);//Disable the button the player just pressed to access this view
            viewType = "ammo";
            updateInventory();//Update the player's inventory
            toggleInventoryButtons(true);//Enable the buttons for the inventory
            silence("coin", true);
            silence("gem", false);
        }
    }

    private void toggleGamePause(bool status)
    {
        pauseMenu.enabled = status;
        if (status)
        {
            storeView(true);
            viewType = "paused";
            saveSlotCanvas.enabled = false;
            gemSelectCamera.enabled = false;
            saveLoadCamera.enabled = false;
            pauseCamera.enabled = true;
            changeButton.gameObject.SetActive(false);
            silence("coin", true);
            silence("gem", true);
            toggleInventoryButtons(false);
            Time.timeScale = 0;
        }
        else
        {
            pauseCamera.enabled = false;
            Time.timeScale = 1;
            switch (storeView(false))
            {
                case "throw":
                    switchToThrow();
                    break;
                case "coin":
                    switchToCoin();
                    break;
                case "shop":
                    switchToShop();
                    break;
                case "ammo":
                    switchToAmmo();
                    break;
            }
        }


    }

    public void unpause()
    {
        toggleGamePause(false);
    }

    public void openSaveLoad(string function)
    {

        dataManager.loadSlots();
        saveSlotCanvas.enabled = true;
        pauseCamera.enabled = false;
        saveLoadCamera.enabled = true;
        pauseMenu.enabled = false;
        saveOrLoad = function;
    }

    public void saveLoad(int slot)//Save or load the game.
    {
        switch (saveOrLoad)
        {
            case "save":
                dataManager.save(slot);
                dataManager.loadSlots();
                break;
            case "load":
                dataManager.load(slot);
                saveSlotCanvas.enabled = false;
                unpause();
                break;
        }
    }

    private string storeView(bool function)
    {
        if (function)//If true, meaning you are storing the view
        {
            if (viewType != "paused")//Don't store the view if it's the paused view
            {
                lastView = viewType;
            }
        }

        return lastView;
    }

    private void updateInventory()//Update the player's inventory in the ammo view
    {
        if (!enableCheats)//If the player isn't cheating
        {
            heldProjectiles[projectileType] = heldProjectiles[projectileType] + loadedTotal;//Take all the loaded projectiles and add them back to the total of the currently selected projectile type
            loadedTotal = 0;//Set the loaded total to 0, as the player has just unloaded
            //Set the text of the inventory to show how many gems the player is holding and their current score value
            inventoryText["rubyHeld"].text = "Held: " + heldProjectiles["ruby"];
            inventoryText["emeraldHeld"].text = "Held: " + heldProjectiles["emerald"];
            inventoryText["amethystHeld"].text = "Held: " + heldProjectiles["amethyst"];
            inventoryText["diamondHeld"].text = "Held: " + heldProjectiles["diamond"];
            inventoryText["rubyValue"].text = "Value: " + projectileValues["ruby"];
            inventoryText["emeraldValue"].text = "Value: " + projectileValues["emerald"];
            inventoryText["amethystValue"].text = "Value: " + projectileValues["amethyst"];
            inventoryText["diamondValue"].text = "Value: " + projectileValues["diamond"];
            checkEmpty();//See if the player has 0 of any of the gems, and if so, change their colour
        }
        else//If the player is cheating
        {
            //Show the player has infinite of every gem, and "set" their "value" to 0 - as the player has an infinite amount of money and thus has no need of earning more; truly a cruel twist of fate to have an infinite amount of valueless gems
            inventoryText["rubyHeld"].text = "Held: ∞";
            inventoryText["emeraldHeld"].text = "Held: ∞";
            inventoryText["amethystHeld"].text = "Held: ∞";
            inventoryText["diamondHeld"].text = "Held: ∞";
            inventoryText["rubyValue"].text = "Value: 0";
            inventoryText["emeraldValue"].text = "Value: 0";
            inventoryText["amethystValue"].text = "Value: 0";
            inventoryText["diamondValue"].text = "Value: 0";
            //Set all the gems to their 'active' colours, as the player has an "infinite" amount
            modifyColour("ruby", false);
            modifyColour("emerald", false);
            modifyColour("amethyst", false);
            modifyColour("diamond", false);
        }
    }

    private void checkEmpty()//Check if the player has run out of gems of a certain type
    {
        foreach (KeyValuePair<string, int> entry in heldProjectiles)//For every gem that exists
        {
            if (entry.Value < 1)//If there's more than one held
            {
                modifyColour(entry.Key, true);//Set it to its appropriate colour
            }
            else//Otherwise
            {
                modifyColour(entry.Key, false);//Set it to an empty, transparent colour
            }
        }
    }

    private void modifyColour(string keyString, bool empty)//Change the material of a UI object
    {
        string selectedGem = "";//Set the selected gem to an empty value as a failsafe
        switch (keyString)//Based on the keyString input, select the matching gem object
        {
            case "ruby":
                selectedGem = "UIRuby";
                break;
            case "emerald":
                selectedGem = "UIEmerald";
                break;
            case "amethyst":
                selectedGem = "UIAmethyst";
                break;
            case "diamond":
                selectedGem = "UIDiamond";
                break;
        }
        if (empty)//If the player doesn't hold any of the selected gem, set it to the defined emptyColour
        {
            gemObjects[selectedGem].GetComponent<Renderer>().material.SetColor("_Color", emptyColour);
            gemObjects[selectedGem].GetComponent<Renderer>().material.SetFloat("_Metallic", 0f);
            gemObjects[selectedGem].GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
        }
        else//Otherwise, set it to its relevant active material
        {
            gemObjects[selectedGem].GetComponent<Renderer>().material = materials[keyString];
        }
    }

    void toggleInventoryButtons(bool activeStatus)//Toggle the active status of all inventory buttons, so they can or cannot be interacted with
    {
        rubyButton.gameObject.SetActive(activeStatus);
        emeraldButton.gameObject.SetActive(activeStatus);
        amethystButton.gameObject.SetActive(activeStatus);
        diamondButton.gameObject.SetActive(activeStatus);

        /**
            Dev Note:
            This was originally intended to iterate through an array containing each button and set them to the supplied active status
            either enabling them or disabling them. However, attempting to change the active status of a game object accessed through
            an array appears to not work and throws a null object reference error. Each button has instead been hard coded to toggle
            its active status instead. Ineffecient and not scalable, but strangely it works despite accesing it in the same way.

            May return to this at a later date to try to bugfix.
        */

        /**
        for(int i = 0; i < inventoryButtons.Length; i++)
        {
            inventoryButtons[i].gameObject.SetActive(activeStatus);
        }
        */

    }


    public void silence(string silentType, bool silentStatus)//Silence all objects of a certain type, provided they have a function in their attached script to do so
    {
        switch (silentType)//Depending on the string provided
        {
            case "gem"://If it's gems
                GameObject[] allGems = GameObject.FindGameObjectsWithTag("Projectile");//Find all the projectiles and put them in an array
                foreach (GameObject gems in allGems)//For each game object in the array
                {
                    BallForward gem = gems.GetComponent<BallForward>();//Set 'gem' to the BallForward script of the currently selected gem in the loop ("BallForward" is ProjectileController)
                    if (!gem.getScored())//If the gem hasn't already been scored
                    {
                        gem.setSilent(silentStatus);//Set the silent status of the gem to the supplied value
                    }
                }
                break;
            case "coin"://Same as above but with coins
                GameObject[] allCoins = GameObject.FindGameObjectsWithTag("Coin");
                foreach (GameObject coins in allCoins)
                {
                    CoinController coin = coins.GetComponent<CoinController>();
                    coin.setSilent(silentStatus);
                }
                break;


        }

    }

    public void callFadeIn(float time, TextMeshProUGUI text)//Start to fade in the reload text
    {
        StartCoroutine(textFadeIn(time, text));
    }

    private IEnumerator textFadeIn(float time, TextMeshProUGUI text)//Fade out the reload text
    {
        //Debug.Log("Attempting to fade in text");
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        while (text.color.a < fadeValue)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / time));
            yield return null;
        }
    }

    public void callFadeOut(float time, TextMeshProUGUI text)//Start to fade out the reload text
    {
        StartCoroutine(textFadeOut(time, text));
    }

    private IEnumerator textFadeOut(float time, TextMeshProUGUI text)//Fade out the reload text
    {
        //Debug.Log("Attempting to fade out text");

        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        while (text.color.a > fadeValue)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
            yield return null;
        }
    }

    public void changeProjectile(string newProjectile)//Change the projectile type
    {
        if (projectileType != newProjectile && !reloadingStatus)//If the new projectile isn't the same as the one currently selected, and the player isn't currently reloading
        {
            projectileType = newProjectile;//The currently selected projectile is now the new projectile
            gameAudio.PlayOneShot(reloadSound, 0.8f);//Play the reloading sound
            refreshCounter();//Refresh the counters to reflect the change
        }
    }

    public string getProjectileType()//Get the currently selected projectile that will be fired with the spacebar
    {
        return projectileType;
    }

    public void setProjectileType(string newProjectileType)//Get the currently selected projectile that will be fired with the spacebar
    {
        projectileType = newProjectileType;
    }

    private void setProjectileValue(string projectileType, int newValue)//Set the worth of a certain projectile to a new value
    {
        projectileValues[projectileType] = newValue;
    }

    public int getProjectileValue(string projectileType)//Get the current worth of a certain projectile
    {
        return projectileValues[projectileType];
    }

    public bool getCheatStatus()//See if the player has enabled cheats or not
    {
        return enableCheats;//Returns 'true' if cheats are enabled.
    }

    public bool getFirstSwitch()
    {
        return firstSwitch;
    }

    public void setFirstSwitch(bool newValue)
    {
        firstSwitch = newValue;
    }



    /**
    * This is an irrelevant function, as the context for its use would always return 'paused'
    public string getViewType()
    {
        return viewType;
    }
    */

    public string getLastViewType()
    {
        return lastView;
    }

    public void setViewType(string newViewType)
    {
        switch (newViewType)//Based on the current viewtype
        {
            case "throw":
                switchToThrow();
                break;
            case "coin":
                switchToCoin();
                break;
            case "shop":
                switchToShop();
                break;
            default:
                Debug.Log("Fell back to throw.");
                switchToThrow();//If no cases match, fall back to the throw view
                break;
        }
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

    public int[] gatherGems()
    {
        int[] returnList = { getCounter("total", "ruby"), getCounter("total", "emerald"), getCounter("total", "amethyst"), getCounter("total", "diamond") };
        return returnList;
    }

    public void depositGems(int[] gemList)
    {
        for (int i = 0; i < gemNames.Length; i++)
        {
            setCounter(gemList[i], "total", gemNames[i]);
        }
    }

    public void createGem(bool silent, bool scored, bool recreated, Vector3 location, Vector3 velocity, string type)
    {
        //Debug.Log("Creating new gem.");
        //Debug.Log("BOOL: Silence: " + silent);
        //Debug.Log("BOOL: Scored: " + scored);
        //Debug.Log("BOOL: Recreated: " + recreated);
        //Debug.Log("VECTOR: Location: " + location);
        //Debug.Log("VECTOR: Velocity: " + velocity);
        //Debug.Log("STRING: Type: " + type);
        GameObject newGem = GameObject.Instantiate(projectilePrefab);
        BallForward newGemScript = newGem.GetComponent<BallForward>();
        //Debug.Log("Gem Sanity Check: " + newGemScript.sanityCheckReturn());
        newGemScript.resetValues();
        newGemScript.setSilent(silent);
        newGemScript.setScored(scored);
        newGemScript.setRecreated(recreated);
        newGemScript.setLocation(location);
        newGemScript.setVelocity(velocity);
        newGemScript.setProjType(type);
    }

    public void createCoin(bool silent, bool collected, bool recreated, bool stuck, bool parentable, Vector3 location, Vector3 velocity, int destroyDelay, PhysicMaterial material, Quaternion rotation)
    {
        GameObject newCoin = GameObject.Instantiate(coinPrefab);
        CoinController newCoinScript = newCoin.GetComponent<CoinController>();
        newCoinScript.resetValues();
        newCoinScript.setSilent(silent);
        newCoinScript.setCollected(collected);
        newCoinScript.setRecreated(recreated);
        newCoinScript.setStuck(stuck);
        newCoinScript.setParentable(parentable);
        newCoinScript.setLocation(location);
        newCoinScript.setVelocity(velocity);
        newCoinScript.setStuckTimer(destroyDelay);
        newCoinScript.setMaterial(material);
        newCoinScript.setRotation(rotation);
    }

    public void runDebug()
    {
        GameObject[] allGems = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject gem in allGems)
        {
            BallForward gemScript = gem.GetComponent<BallForward>();
            //Debug.Log(gemScript.getLocation());
            List<List<Vector3>> testList = new List<List<Vector3>>();
            testList.Add(gemScript.gatherVectors());
            for (int i = 0; i < testList.Count; i++)
            {
                Debug.Log(testList[i]);
                for (int j = 0; j < testList[i].Count; j++)
                {
                    Debug.Log(i + "-" + j + ": " + testList[i][j]);
                }
            }

        }
    }

    public void clear()
    {
        GameObject[] allGems = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject gem in allGems)
        {
            Destroy(gem);
        }
    }
}
