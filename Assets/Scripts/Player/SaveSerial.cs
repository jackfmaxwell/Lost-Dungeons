
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


//This class saves the game data (level, inventory, equipment, equipped skills) to a serialized file, and loads from that file when needed
/// <NOTE>
/// We may need to load on each level since data doesnt persist between levels
/// </NOTE>
public class SaveSerial : MonoBehaviour
{
    //Refrence
    private Player_Inventory inventory; //the player inventory

    private void Awake()
    {
        try
        {
            inventory = this.GetComponent<Player_Inventory>();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
    private void Start()
    {
        //SaveGame();
    }
    //This method will eventually be called when the player quits the game, or autosave on a timer
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/MySaveData.dat");
        Debug.Log(Application.persistentDataPath);

        SaveData data = new SaveData();
        int[] inventoryItemIndexs = inventory.getInventoryIndexs();
        int[] equippedItemIndexs = inventory.getEquippedGearIndexs();
        int[] skillIndexs = inventory.getSkillIndexs();

        data.savedInventory = inventoryItemIndexs;
        data.savedEquippedItems = equippedItemIndexs;
        data.savedSkills = skillIndexs;
        data.level = inventory.level;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved");
    }

    //Load when the player opens the game
    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/MySaveData.dat", FileMode.Open);

            SaveData data = (SaveData)bf.Deserialize(file);

            inventory.setInventoryIndexs(data.savedInventory);
            inventory.populateScriptableInventory();
            
            inventory.setEquippedGearIndexs(data.savedEquippedItems); //problem area
            inventory.populateScriptableGear();

            inventory.setSkillIndexs(data.savedSkills);
            inventory.populateScriptableSkills();

            inventory.level = data.level;
            file.Close();


            Debug.Log("Game data loaded");
        }
        else
        {
            Debug.LogError("There is no save data");
        }
    }
}

//The save variables (scriptable objects can be serialized, instead we use an item index system)
[Serializable]
class SaveData
{
    
    public int[] savedInventory = new int[63];
    public int[] savedEquippedItems = new int[7];
    public int[] savedSkills = new int[2];
    public int level;
}