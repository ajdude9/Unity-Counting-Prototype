using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;


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

        if (instance != null)//If Instance already exists; so has already been created
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
        if (sceneName == "Main Game")
        {
            Debug.Log("Searching for game controller.");
            gameController = GameObject.Find("Game Manager").GetComponent<CounterController>();
            box = GameObject.Find("Box").GetComponent<BoxMovement>();

        }

    }

    [System.Serializable]
    public class BoolListWrapper
    {
        public List<bool> boolList = new List<bool>();        

        public void add(bool newValue)
        {
            Debug.Log("Attempting to add to Boolean wrapper list");
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
        public void checkExistence()
        {
            try
            {
                Debug.Log("Boolean Existence Check Passed");
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Boolean Existence Error: " + e);
            }
        }
    }

    [System.Serializable]
    public class VectorListWrapper
    {
        public List<Vector3> vectorList = new List<Vector3>();

        public void add(Vector3 newValue)
        {
            Debug.Log("Attempting to add to VectorList wrapper list");
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
        public void checkExistence()
        {
            try
            {
                Debug.Log("Vector Existence Check Passed");
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Vector Existence Error: " + e);
            }
        }
    }



    [System.Serializable]
    class SaveData
    {
        //Basic Saving Variables
        public string gemSelected;//The currently selected gem
        public int[] gemsSaved;//All the gems the player has
        public int gemsLoaded;//The amount of gems that are loaded
        public int coinsBanked;//The amount of coins the player can spend
        public int coinsWon;//The amount of gems the player can drop        
        public Vector3 boxLocation;//Where the box currently is.
        public Vector3[] boxDestinations;//Where the box is moving to.
        public bool firstSwitch;//If the player has switched to the coin view for the first time, and whether or not to create coins.

        //Projectile Object Saving Variables
        public List<BoolListWrapper> projectileBooleans;//An array containing every projectile and its three boolean values
        public List<VectorListWrapper> projectileVectors;//An array containing every projectile and its location and velocity
        public List<string> projectileTypes;//An array containing every projectile and its material type
        public int projectilesSaved;

        //Coin Object Saving Variables
        public List<BoolListWrapper> coinBooleans;
        public List<VectorListWrapper> coinVectors;
        public List<int> coinDelays;
        public List<PhysicMaterial> coinMaterials;
        public List<Quaternion> coinRotations;
        public int coinsSaved;
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
        data.firstSwitch = gameController.getFirstSwitch();
        

        GameObject[] allGems = GameObject.FindGameObjectsWithTag("Projectile");//Find all the gems in the scene
        GameObject[] allCoins = GameObject.FindGameObjectsWithTag("Coin");//Find all the coins in the scene
        data.projectilesSaved = allGems.Length;//We know the total number of gems we'll save based on the number of gems that exist
        data.coinsSaved = allCoins.Length;//Same with coins
        //Debug.Log("Test Wrapper Test: ");
        //data.testWrapper.checkExistence();

        /**
        List<List<Vector3>> tempVector3s = new List<List<Vector3>>();
        List<List<bool>> tempBools = new List<List<bool>>();
        List<string> tempTypes = new List<string>();
        */
        List<Vector3> tempVector3s = new List<Vector3>();
        List<bool> tempBools = new List<bool>();
        List<string> tempTypes = new List<string>();
        int k = 0;
        try
        {
            //Debug.Log("Assigning values to List containers");
            data.projectileBooleans = new List<BoolListWrapper>();
            data.projectileVectors = new List<VectorListWrapper>();
            data.projectileTypes = new List<string>();

            data.coinBooleans = new List<BoolListWrapper>();
            data.coinVectors = new List<VectorListWrapper>();
            data.coinDelays = new List<int>();
            data.coinMaterials = new List<PhysicMaterial>();


            //Debug.Log("List Sizes:");
            //Debug.Log("Boolean: " + data.projectileBooleans.Count);
            //Debug.Log("Vector: " + data.projectileBooleans.Count);
            //Debug.Log("Running ForEach");
            foreach (GameObject gems in allGems)
            {
                //Debug.Log("Adding new main lists");
                // -- Projectile Data Lists
                data.projectileBooleans.Add(new BoolListWrapper());//Add a new instance of a nested list, as natural nested lists don't work
                data.projectileVectors.Add(new VectorListWrapper());
                BallForward gem = gems.GetComponent<BallForward>();
                

                //Debug.Log("New List Sizes:");
                //Debug.Log("Boolean: " + data.projectileBooleans.Count);
                //Debug.Log("Vector: " + data.projectileBooleans.Count);
                //Debug.Log("Item contained in Boolean List: " + data.projectileBooleans[0]);
                //data.projectileBooleans[0].checkExistence();
                //Debug.Log("Test Addition");
                //data.projectileBooleans[0].add(true);

                //Debug.Log("Gathering Data");
                //tempBools = gem.gatherBooleans();
                //tempVector3s = gem.gatherVectors();
                //tempTypes.Add(gem.getProjType());

                /**
                data.projectileBooleans.Add(tempBools);
                data.projectileVectors.Add(tempVector3s);
                data.projectileTypes.Add(gem.getProjType());
                */

                //Debug.Log("Assigning Data");
                data.projectileBooleans[k].add(gem.getSilent());
                data.projectileBooleans[k].add(gem.getScored());
                data.projectileBooleans[k].add(gem.getRecreated());
                data.projectileVectors[k].add(gem.getLocation());
                data.projectileVectors[k].add(gem.getVelocity());
                data.projectileTypes.Add(gem.getProjType());
                k++;
            }
            k = 0;//Reset k for use with coins
            foreach(GameObject coins in allCoins)
            {
                data.coinBooleans.Add(new BoolListWrapper());
                data.coinVectors.Add(new VectorListWrapper());
                CoinController coin = coins.GetComponent<CoinController>();

                data.coinBooleans[k].add(coin.getSilent());
                data.coinBooleans[k].add(coin.getCollected());
                data.coinBooleans[k].add(coin.getRecreated());
                data.coinBooleans[k].add(coin.getStuck());
                data.coinBooleans[k].add(coin.getParentable());

                data.coinVectors[k].add(coin.getLocation());
                data.coinVectors[k].add(coin.getVelocity());

                data.coinDelays.Add(coin.getStuckTimer());
                data.coinMaterials.Add(coin.getMaterial());
                //data.coinRotations.Add(coin.getRotation());

                k++;
            }
        }
        catch (NullReferenceException e)
        {
            Debug.Log("An error has occurred: " + e);                        
            /**
            Debug.Log("Data List Count: " + k);
            Debug.Log("Existence Check: ");            
            data.projectileBooleans[k].checkExistence();
            data.projectileVectors[k].checkExistence();
            Debug.Log("Data Lists: " + data.projectileBooleans + ", and " + data.projectileVectors);
            Debug.Log("Data List Information: " + data.projectileBooleans[k] + ", and " + data.projectileVectors[k]);
            */
            
        }
        //data.projectileBooleans = tempBools;
        //data.projectileVectors = tempVector3s;


        data.projectileTypes = tempTypes;
        for (int i = 0; i < data.projectileVectors.Count; i++)
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
            //Clear the current field of any lingering objects
            gameController.clear();
            //Load game data
            gameController.setProjectileType(data.gemSelected);
            gameController.depositGems(data.gemsSaved);
            gameController.setCounter(data.gemsLoaded, "loaded", "");
            gameController.setCounter(data.coinsBanked, "bank", "");
            gameController.setCounter(data.coinsWon, "coins", "");
            gameController.setFirstSwitch(data.firstSwitch);
            //Load box data
            box.setPos(data.boxLocation);
            box.setDestinations(data.boxDestinations[0], data.boxDestinations[1]);
            //Load projectile data, one at a time

            /**
            for (int i = 0; i < data.projectileBooleans.Count; i++)
            {
            
                bool loadedSilent = data.projectileBooleans[i][0];
                bool loadedScored = data.projectileBooleans[i][1];
                bool loadedRecreated = data.projectileBooleans[i][2];
                Vector3 loadedLocation = data.projectileVectors[i][0];
                Vector3 loadedVelocity = data.projectileVectors[i][1];                
                string loadedType = data.projectileTypes[i];
                //gameController.createGem(loadedSilent, loadedScored, loadedRecreated, loadedLocation, loadedVelocity, loadedType);
            }
            */
            int i = 0;
            while(i < data.projectilesSaved)
            {
                gameController.createGem(data.projectileBooleans[i].retrieve(0), data.projectileBooleans[i].retrieve(1), data.projectileBooleans[i].retrieve(2), data.projectileVectors[i].retrieve(0), data.projectileVectors[i].retrieve(1), data.projectileTypes[i]);
                i++;
            }
            i = 0;//Reset i for use with coins
            while(i < data.coinsSaved)
            {
                gameController.createCoin(data.coinBooleans[i].retrieve(0), data.coinBooleans[i].retrieve(1), data.coinBooleans[i].retrieve(2), data.coinBooleans[i].retrieve(3), data.coinBooleans[i].retrieve(4), data.coinVectors[i].retrieve(0), data.coinVectors[i].retrieve(1), data.coinDelays[i], data.coinMaterials[i]);
                i++;
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
