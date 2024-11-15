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

public class CounterController : MonoBehaviour
{
    private CoinDropController coinDropController;
    public Text counterText;//Text object to display the number of projectiles
    public Text loadedText;//Text object to display the number of projectiles ready
    public TextMeshProUGUI reloadNotify;//Text object to tell the player to reload
    public TextMeshProUGUI coinsSavedText;
    public TextMeshProUGUI coinsDroppableText;
    private int loadedTotal;//The number of projectiles ready to fire
    private int reloadMax;//The total number of projectiles that can be loaded at once.
    private int coinsSaved = 0;//The number of coins the player has 'banked'
    private int coinsDroppable = 0;//The number of coins the player can drop.
    private float reloadSpeed;//How long it takes to reload one projectile
    public float fireRate;//How fast the player can autofire projectiles
    public String projType;//The name of the projectiles being fired
    public bool reloadingStatus = false;//Whether the player is currently 'reloading'
    public bool enableCheats = false;//Whether cheats are enabled
    private AudioSource gameAudio;//Source for the universal game audio
    public AudioClip reloadSound;//Reloading sound (fully reloaded)
    public AudioClip reloadSingle;//Reloading sound (single projectile)
    public AudioClip reloadFail;//Reloading fail sound (no ammo)
    private Camera throwCamera;//The camera for throwing projectiles into a box
    private Camera coinCamera;//The camera for putting coins in a machine
    private Camera shopCamera;//The camera for viewing the shop
    private Camera gemSelectCamera;//The camera for viewing the gem select UI
    public String viewType;//Which camera is currently being viewed
    public bool firstSwitch = false;//Whether the switch to the coin camera is the first one since the game started
    public int fadeValue;
    private string projectileType;//The type of projectile that has been selected
    private Dictionary<string, int> heldProjectiles;//A key:value list to contain the number of a projectile and its name
    private Dictionary<string, int> projectileValues;//A key:value list to contain the name of a projectile and its value in coins when scored
    private Dictionary<string, GameObject> gemObjects;//A key:value list to contain all the gem objects used in the scene
    private Dictionary<string, TextMeshProUGUI> inventoryText;//A key:value list to contain the text of how many gems the player holds and their values
    private UnityEngine.UIElements.Button changeGemButton;
    private Color emptyColour = new Color(0.0f, 0.0f, 0.0f, 0f);//A black colour
    private UnityEngine.UI.Button changeButton;
    private UnityEngine.UI.Button[] inventoryButtons;

    // Start is called before the first frame update
    void Start()
    {
        //get the game audio and set the default values for variables
        projectileType = "ruby";
        gameAudio = gameObject.GetComponent<AudioSource>();
        coinDropController = GameObject.Find("Coin Dropper").GetComponent<CoinDropController>();
        heldProjectiles = new Dictionary<string, int>()
        {
            {"ruby", 54},
            {"emerald", 7},
            {"amethyst", 0},
            {"diamond", 3}
        };
        projectileValues = new Dictionary<string, int>()
        {
            {"ruby", 1},
            {"emerald", 2},
            {"amethyst", 5},
            {"diamond", 20}
        };
        gemObjects = new Dictionary<string, GameObject>()
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
        inventoryText = new Dictionary<string, TextMeshProUGUI>()
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
        changeButton = GameObject.Find("Change Button").GetComponent<UnityEngine.UI.Button>();
        GameObject[] inventoryButtonHolder = GameObject.FindGameObjectsWithTag("Inventory Button");
        
        for(int i = 0; i < inventoryButtonHolder.Length; i++)
        {
            Debug.Log(inventoryButtonHolder[i]);
            //inventoryButtons[i] = inventoryButtonHolder[i].GetComponent<UnityEngine.UI.Button>();
        }
        loadedTotal = 6;
        reloadMax = 6;
        reloadSpeed = 0.15f;
        fireRate = 0.35f;
        counterText.enabled = true;
        loadedText.enabled = true;
        counterText.text = "Available " + projType + ": " + heldProjectiles[projectileType];
        loadedText.text = "Loaded " + projType + ": " + loadedTotal;
        coinsSavedText.text = "Coins Won: " + coinsSaved;
        coinsDroppableText.text = "Droppable Coins: " + coinsDroppable;
        reloadNotify.color = new Color(reloadNotify.color.r, reloadNotify.color.g, reloadNotify.color.b, 0);
        throwCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        coinCamera = GameObject.Find("Machine Watcher").GetComponent<Camera>();
        shopCamera = GameObject.Find("Shop Camera").GetComponent<Camera>();
        gemSelectCamera = GameObject.Find("Select Camera").GetComponent<Camera>();
        //changeGemButton = GameObject.Find("Change Button").GetComponent<UnityEngine.UIElements.Button>();
        viewType = "throw";
        switchToThrow();
        
    }

    // Update is called once per frame

    void Update()
    {
        keycodeController();
        if (loadedTotal == reloadMax)
        {
            textFadeOut(1, reloadNotify);
        }
    }

    private void keycodeController()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            infiniteAmmoCheat();//Enable cheats
        }
        if (Input.GetKeyDown(KeyCode.V))//Switch the camera view forwards along the list
        {
            switch (viewType)
            {
                case "throw":
                    switchToCoin();
                    break;
                case "coin":
                    switchToShop();
                    break;
                case "shop":
                    switchToThrow();
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.C))//Switch the camera view backwards along the list
        {
            switch (viewType)
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
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            fadeValue = 0;
            StartCoroutine(textFadeOut(1f, reloadNotify));
            reload();
        }
    }

    public void refreshCounter()//Update the counters to their current values
    {
        counterText.text = "Available " + projType + ": " + heldProjectiles[projectileType];
        loadedText.text = "Loaded " + projType + ": " + loadedTotal;
        coinsDroppableText.text = "Droppable Coins: " + coinsDroppable;
        if (viewType == "shop")
        {
            coinsSavedText.text = "Coins Available: " + coinsSaved;
        }
        else
        {
            coinsSavedText.text = "Coins Won: " + coinsSaved;
        }
    }

    public int getCounter(String type, String gemType)//Get a specific counter
    {
        int returnAmount = 0;
        switch (type)
        {
            case "total"://Get the total amount of projectiles
                returnAmount = heldProjectiles[gemType];
                break;
            case "loaded"://Get the amount of projectiles able to be fired
                returnAmount = loadedTotal;
                break;
            case "coins":
                returnAmount = coinsDroppable;
                break;
            case "bank":
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

        if (!reloadingStatus)//If the player isn't already reloading
        {
            int amountToReload = reloadMax - loadedTotal;//The amount of projectiles to reload is the maximum allowed, minus however many are already loaded
            if (amountToReload > 0 && getCounter("total", projectileType) > 0)
            {
                reloadingStatus = true;
                if (amountToReload > getCounter("total", projectileType))//If there isn't enough stored projectiles to reload fully
                {
                    amountToReload = getCounter("total", projectileType);//Instead reload however many are left
                }

                StartCoroutine(reloadTimer(amountToReload));
            }
            else
            {
                gameAudio.PlayOneShot(reloadFail, 0.6f);
            }
        }
    }

    public void quickReload()//Quickly full-reload
    {
        if(!reloadingStatus)//If the player isn't already reloading
        {
            int amountToReload = 0;//Set the amount to reload to 0 as default
            if(getCounter("total", projectileType) > reloadMax)//If the player has more ammo stored than they can load at once
            {
                amountToReload = reloadMax;//Reload the maximum amount
            }
            else
            {
                if(getCounter("total", projectileType) > 0)//If the player doesn't have enough to full reload, but does have ammo
                {
                    amountToReload = getCounter("total", projectileType);//Reload all the ammo left
                }
            }
            if(amountToReload > 0)//If the player has ammo to reload
            {
                addCounter(amountToReload, "loaded", "");
                minusCounter(amountToReload, "total", projectileType);
            }
        }

    }

    private IEnumerator reloadTimer(int reloadAmount)//Reload projectiles on a timer rather than all at once
    {

        for (int i = 0; i < reloadAmount; i++)
        {

            yield return new WaitForSeconds(reloadSpeed);//Delay based on reload speed
            addCounter(1, "loaded", "");
            minusCounter(1, "total", projectileType);
            gameAudio.PlayOneShot(reloadSingle, 0.8f);

        }
        yield return new WaitForSeconds(0.1f);
        if (reloadingStatus)
        {
            gameAudio.PlayOneShot(reloadSound, 0.8f);
            reloadingStatus = false;
        }
        yield return null;
    }

    private void infiniteAmmoCheat()//Enable cheats, setting status to immensely high values
    {
        enableCheats = true;       
        heldProjectiles["ruby"] = 1000000;
        heldProjectiles["emerald"] = 1000000;
        heldProjectiles["amethyst"] = 1000000;
        heldProjectiles["diamond"] = 1000000;
        loadedTotal = 1000000;
        reloadMax = 1000000;
        reloadSpeed = 0.01f;
        fireRate = 0.05f;
        coinsDroppable = 1000000;
        coinsSaved = 1000000;
        refreshCounter();
    }

    private void switchToCoin()
    {
        throwCamera.enabled = false;
        coinCamera.enabled = true;
        shopCamera.enabled = false;
        viewType = "coin";
        counterText.enabled = false;
        loadedText.enabled = false;
        coinsSavedText.enabled = true;
        coinsDroppableText.enabled = true;
        changeButton.gameObject.SetActive(false);
        coinsSavedText.transform.position = new Vector3(coinsSavedText.transform.position.x, 1000, coinsSavedText.transform.position.z);
        if (!firstSwitch)
        {
            firstSwitch = true;
            coinDropController.generateCoins(coinDropController.getCoinSpawnAmount());
        }


    }

    public void switchToThrow()
    {
        if(viewType == "ammo")
        {
            quickReload();
        }
        throwCamera.enabled = true;
        gemSelectCamera.enabled = false;
        coinCamera.enabled = false;
        shopCamera.enabled = false;
        counterText.enabled = true;
        loadedText.enabled = true;
        coinsSavedText.enabled = false;
        coinsDroppableText.enabled = false;
        changeButton.gameObject.SetActive(true);
        GameObject.Find("Change Button").SetActive(true);
        viewType = "throw";
    }

    private void switchToShop()
    {
        throwCamera.enabled = false;
        coinCamera.enabled = false;
        shopCamera.enabled = true;
        counterText.enabled = false;
        loadedText.enabled = false;
        coinsSavedText.enabled = true;
        coinsDroppableText.enabled = false;
        changeButton.gameObject.SetActive(false);
        coinsSavedText.transform.position = new Vector3(coinsSavedText.transform.position.x, 1100, coinsSavedText.transform.position.z);
        coinsSavedText.text = "Coins Available: " + coinsSaved;
        viewType = "shop";
    }

    public void switchToAmmo()
    {
        if (!reloadingStatus)
        {
            gemSelectCamera.enabled = true;
            counterText.enabled = false;
            loadedText.enabled = false;
            changeButton.gameObject.SetActive(false);
            viewType = "ammo";
            updateInventory();
        }
    }

    private void updateInventory()
    {
        heldProjectiles[projectileType] = heldProjectiles[projectileType] + loadedTotal;
        loadedTotal = 0;
        inventoryText["rubyHeld"].text = "Held: " + heldProjectiles["ruby"];
        inventoryText["emeraldHeld"].text = "Held: " + heldProjectiles["emerald"];
        inventoryText["amethystHeld"].text = "Held: " + heldProjectiles["amethyst"];
        inventoryText["diamondHeld"].text = "Held: " + heldProjectiles["diamond"];
        inventoryText["rubyValue"].text = "Value: " + projectileValues["ruby"];
        inventoryText["emeraldValue"].text = "Value: " + projectileValues["emerald"];
        inventoryText["amethystValue"].text = "Value: " + projectileValues["amethyst"];
        inventoryText["diamondValue"].text = "Value: " + projectileValues["diamond"];
        checkEmpty();
    }

    private void checkEmpty()
    {
        foreach (KeyValuePair<string, int> entry in heldProjectiles)
        {
            if (entry.Value == 0)
            {
                modifyColor(entry.Key, true);
            }
        }
    }

    private void modifyColor(string keyString, bool empty)
    {
        string selectedGem = "";
        switch (keyString)
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
        if (empty)
        {
            gemObjects[selectedGem].GetComponent<Renderer>().material.SetColor("_Color", emptyColour);
            gemObjects[selectedGem].GetComponent<Renderer>().material.SetFloat("_Metallic", 0f);
            gemObjects[selectedGem].GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
        }
        else
        {

        }
    }

    void toggleInentoryButtons(bool activeStatus)
    {
        for(int i = 0; i < inventoryButtons.Length; i++)
        {
            inventoryButtons[i].gameObject.SetActive(activeStatus);
        }
    }

    public void callFadeIn(float time, TextMeshProUGUI text)
    {
        StartCoroutine(textFadeIn(time, text));
    }

    private IEnumerator textFadeIn(float time, TextMeshProUGUI text)
    {
        //Debug.Log("Attempting to fade in text");
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        while (text.color.a < fadeValue)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / time));
            yield return null;
        }
    }

    public void callFadeOut(float time, TextMeshProUGUI text)
    {
        StartCoroutine(textFadeOut(time, text));
    }

    private IEnumerator textFadeOut(float time, TextMeshProUGUI text)
    {
        //Debug.Log("Attempting to fade out text");

        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        while (text.color.a > fadeValue)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
            yield return null;
        }
    }

    public void changeProjectile(string newProjectile)
    {
        if (projectileType != newProjectile && !reloadingStatus)
        {
            projectileType = newProjectile;
            gameAudio.PlayOneShot(reloadSound, 0.8f);
            refreshCounter();
        }
    }

    public string getProjectileType()
    {
        return projectileType;
    }

    private void setProjectileValue(string projectileType, int newValue)
    {
        projectileValues[projectileType] = newValue;
    }

    public int getProjectileValue(string projectileType)
    {
        return projectileValues[projectileType];
    }    

}
