using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private CounterController gameController;
    
    private void Awake()
    {
        /**
        if(instance != null)//If Instance already exists; so has already been created
        {            
            Destroy(gameObject);//Destroy this game object; to prevent duplication.
        }
        else
        {
            instance = this;//Set the MainManager script variable, Instance, to this current instance
            DontDestroyOnLoad(gameObject);//Don't destroy this game object when changing scenes            
        }
        */
        gameController = GameObject.Find("Game Manager").GetComponent<CounterController>();
    }
    

    [System.Serializable]
    class SaveData
    {
        public string gemSelected;
        public int[] gemsSaved;
        public int gemsLoaded;        
        public int coinsSaved;
        public int coinsWon;
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
        data.coinsSaved = gameController.getCounter("bank", "");
        data.coinsWon = gameController.getCounter("coins", "");

        pers.lastSavedSlot = saveSlot;
        
        string newSave = JsonUtility.ToJson(data);//Convert everything in the data SaveData class into json string under 'newSave'
        string persistentSave = JsonUtility.ToJson(pers);

        File.WriteAllText(Application.persistentDataPath + "/save_slot_" + saveSlot + ".json", newSave);
        File.WriteAllText(Application.persistentDataPath + "/persistentData.json", persistentSave);
    }

    public void load(int saveSlot)
    {
        string savePath = Application.persistentDataPath + "/save_slot_" + saveSlot + ".json";//Define the path as the path set in the save data            
        if(File.Exists(savePath))//If we find a file in the filepath
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            gameController.setProjectileType(data.gemSelected);
            gameController.depositGems(data.gemsSaved);
            gameController.setCounter(data.gemsLoaded, "loaded", "");
            gameController.setCounter(data.coinsSaved, "bank", "");
            gameController.setCounter(data.coinsWon, "coins", "");
        }
    }

    public void quickLoad()
    {
        string persPath = Application.persistentDataPath + "/persistentData.json";
        int selectedFile = 0;
        if(File.Exists(persPath))
        {
            string persJson = File.ReadAllText(persPath);
            PersistentData persistence = JsonUtility.FromJson<PersistentData>(persJson);
            selectedFile = persistence.lastSavedSlot;      
        }

        if(selectedFile != 0)
        {
            load(selectedFile);
        }
        else
        {
            //No saves exist and/or save slot hasn't been recorded
        }
    }
}
