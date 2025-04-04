using System;
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
    public class BoolListWrapper
    {
        public List<bool> boolList;

        public void add(bool newValue)
        {
            boolList.Add(newValue);
        }
        public bool retrieve(int pos)
        {
            return boolList[pos];
        }
        public void replace(bool newValue, int pos)
        {
            boolList[pos] = newValue;
        }
        public void inherit(List<bool> newList)
        {
            boolList = newList;
        }
    }

    [System.Serializable]
    public class VectorListWrapper
    {
        public List<Vector3> vectorList;

        public void add(Vector3 newValue)
        {
            vectorList.Add(newValue);
        }
        public Vector3 retrieve(int pos)
        {
            return vectorList[pos];
        }
        public void replace(Vector3 newValue, int pos)
        {
            vectorList[pos] = newValue;
        }
        public void inherit(List<Vector3> newList)
        {
            vectorList = newList;
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
        
        public List<BoolListWrapper> projectileBooleans;//An array containing every projectile and its three boolean values
        public List<VectorListWrapper> projectileVectors;//An array containing every projectile and its location and velocity
        public List<string> projectileTypes;//An array containing every projectile and its material type
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
        
        GameObject[] allGems = GameObject.FindGameObjectsWithTag("Projectile");
        
        /**
        List<List<Vector3>> tempVector3s = new List<List<Vector3>>();
        List<List<bool>> tempBools = new List<List<bool>>();
        List<string> tempTypes = new List<string>();
        */
        List<Vector3> tempVector3s = new List<Vector3>();
        List<bool> tempBools = new List<bool>();
        List<string> tempTypes = new List<string>();
        int k = 0;
        foreach(GameObject gems in allGems)
        {            
            BallForward gem = gems.GetComponent<BallForward>();               
            
            
            tempBools = gem.gatherBooleans();
            tempVector3s = gem.gatherVectors();
            tempTypes.Add(gem.getProjType());
            
            /**
            data.projectileBooleans.Add(tempBools);
            data.projectileVectors.Add(tempVector3s);
            data.projectileTypes.Add(gem.getProjType());
            */
            data.projectileBooleans[k].inherit(tempBools);
            data.projectileVectors[k].inherit(gem.gatherVectors());
            data.projectileTypes.Add(gem.getProjType());
            k++;
        }
        //data.projectileBooleans = tempBools;
        //data.projectileVectors = tempVector3s;
        data.projectileTypes = tempTypes;
        for(int i = 0; i < data.projectileVectors.Count; i++)
        {       
            /**     
            for(int j = 0; j < data.projectileVectors[i].Count; j++)
            {                                        
                Debug.Log("Vector " + i + "-" + j + ": " + data.projectileVectors[i][j]);
            }
            */
        }

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
            //Load game data
            gameController.setProjectileType(data.gemSelected);
            gameController.depositGems(data.gemsSaved);
            gameController.setCounter(data.gemsLoaded, "loaded", "");
            gameController.setCounter(data.coinsBanked, "bank", "");
            gameController.setCounter(data.coinsWon, "coins", "");
            //Load box data
            box.setPos(data.boxLocation);
            box.setDestinations(data.boxDestinations[0], data.boxDestinations[1]);
            //Load projectile data, one at a time
            
            for(int i = 0; i < data.projectileBooleans.Count; i++)
            {
                /**
                bool loadedSilent = data.projectileBooleans[i][0];
                bool loadedScored = data.projectileBooleans[i][1];
                bool loadedRecreated = data.projectileBooleans[i][2];
                Vector3 loadedLocation = data.projectileVectors[i][0];
                Vector3 loadedVelocity = data.projectileVectors[i][1];
                */
                string loadedType = data.projectileTypes[i];
                //gameController.createGem(loadedSilent, loadedScored, loadedRecreated, loadedLocation, loadedVelocity, loadedType);
            }
            

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
            //Debug.Log("Looking for Slot" + i + "GemsText");
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

    public void logObjects()
    {
        
    }
    
}
 