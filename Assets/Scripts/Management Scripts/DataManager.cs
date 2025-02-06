using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private CounterController gameController;   
    private BoxMovement box;

    private Scene m_Scene;
    private string sceneName;
    private int currentProfile;
    private void Awake()
    {
        
        if(instance != null)//If Instance already exists; so has already been created
        {            
            Destroy(gameObject);//Destroy this game object; to prevent duplication.
        }
        else
        {
            instance = this;//Set the MainManager script variable, Instance, to this current instance
            DontDestroyOnLoad(gameObject);//Don't destroy this game object when changing scenes            
        }
        
        m_Scene = SceneManager.GetActiveScene();
        sceneName = m_Scene.name;
        if(sceneName == "Main Game")
        {
            Debug.Log("Searching for game controller.");
            gameController = GameObject.Find("Game Manager").GetComponent<CounterController>();
            box = GameObject.Find("Box").GetComponent<BoxMovement>();

        }        

    }


    [System.Serializable]
    class SaveData
    {
        public string gemSelected;//The currently selected gem
        public int[] gemsSaved;//All the gems the player has
        public int gemsLoaded;//The amount of gems that are loaded
        public int coinsBanked;//The amount of coins the player can spend
        public int coinsWon;//The amount of gems the player can drop        
        public Vector3 boxLocation;//Where the box currently is.
        public Vector3[] boxDestinations;//Where the box is moving to.
    }

    [System.Serializable]
    class PersistentData
    {
        public int lastSavedSlot;
    }

    public void save(int saveSlot)
    {
        SaveData data = new SaveData();
        PersistentData pers = new PersistentData();
        data.gemSelected = gameController.getProjectileType();
        data.gemsSaved = gameController.gatherGems();
        data.gemsLoaded = gameController.getCounter("loaded", "");
        data.coinsBanked = gameController.getCounter("bank", "");
        data.coinsWon = gameController.getCounter("coins", "");
        data.boxLocation = box.getPos();
        data.boxDestinations = box.getDestinations();

        pers.lastSavedSlot = saveSlot;

        string newSave = JsonUtility.ToJson(data);//Convert everything in the data SaveData class into json string under 'newSave'
        string persistentSave = JsonUtility.ToJson(pers);

        File.WriteAllText(Application.persistentDataPath + "/save_slot_" + saveSlot + ".json", newSave);
        File.WriteAllText(Application.persistentDataPath + "/persistentData.json", persistentSave);
    }

    public void load(int saveSlot)
    {
        string savePath = Application.persistentDataPath + "/save_slot_" + saveSlot + ".json";//Define the path as the path set in the save data            
        if (File.Exists(savePath))//If we find a file in the filepath
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            gameController.setProjectileType(data.gemSelected);
            gameController.depositGems(data.gemsSaved);
            gameController.setCounter(data.gemsLoaded, "loaded", "");
            gameController.setCounter(data.coinsBanked, "bank", "");
            gameController.setCounter(data.coinsWon, "coins", "");

            box.setPos(data.boxLocation);
            box.setDestinations(data.boxDestinations[0], data.boxDestinations[1]);

        }
    }

    

    public void quickLoad()
    {
        string persPath = Application.persistentDataPath + "/persistentData.json";        
        if (File.Exists(persPath))
        {
            string persJson = File.ReadAllText(persPath);
            PersistentData persistence = JsonUtility.FromJson<PersistentData>(persJson);
            setCurrentProfile(persistence.lastSavedSlot);
        }
    }

    public void loadSlots()
    {
        for (int i = 1; i < 4; i++)
        {
            string savePath = Application.persistentDataPath + "/save_slot_" + i + ".json";//Define the path as the path set in the save data   
            Debug.Log("Looking for Slot" + i + "GemsText");
            //TextMeshPro gemSlot = GameObject.Find("Slot" + i + "GemsText").GetComponent<TextMeshPro>();
            TextMeshProUGUI gemSlot = GameObject.Find("Slot" + i + "GemsText").gameObject.GetComponent<TextMeshProUGUI>();            
            TextMeshProUGUI coinsSlot = GameObject.Find("Slot" + i + "CoinsText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI bankSlot = GameObject.Find("Slot" + i + "BankText").GetComponent<TextMeshProUGUI>();
            if (File.Exists(savePath))//If we find a file in the filepath
            {
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                gemSlot.text = "Gems: " + data.gemsSaved[0] + ", " + data.gemsSaved[1] + ", " + data.gemsSaved[2] + ", " + data.gemsSaved[3];
                coinsSlot.text = "Coins: " + data.coinsWon;
                bankSlot.text = "Bank: " + data.coinsBanked;
            }
            else
            {
                gemSlot.text = "Gems: ";
                coinsSlot.text = "Coins: ";
                bankSlot.text = "Bank: ";
            }
        }
    }
    
    public void setCurrentProfile(int newValue)
    {
        currentProfile = newValue;
    }

    public int getCurrentProfile()
    {
        return currentProfile;
    }

    public void refresh()
    {
        gameController = GameObject.Find("Game Manager").GetComponent<CounterController>();
        box = GameObject.Find("Box").GetComponent<BoxMovement>();

    }

    
}
 