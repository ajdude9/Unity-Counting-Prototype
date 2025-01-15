using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    private CounterController gameController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    class SaveData
    {
        public int[] gemsSaved;
        public int coinsSaved;
        public int coinsWon;
    }

    public void save()
    {
        SaveData data = new SaveData();
        data.coinsSaved = gameController.getCounter("bank", "");
    }
}
