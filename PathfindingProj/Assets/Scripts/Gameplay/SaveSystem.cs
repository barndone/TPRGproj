using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{ 
    //  singleton
    static public SaveSystem instance;

    //  filepath for the saved binary file
    string filePath;

    //  filepath for the saved binary file -- stats after winning/lopsing
    string statsPath;

    private void Awake()
    {
        //  if there is no other instance of this class in the scene:
        if (!instance)
        {
            //  set the instance to this object
            instance = this;
        }
        //  otherwise:
        else
        {
            //  destroy this object
            Destroy(this);
        }

        filePath = Application.persistentDataPath + "/party.data";
        statsPath = Application.persistentDataPath + "/endOfLevelStats.data";
    }

    //  Save your party
    public void SaveParty(List<int> party)
    {
        //  create a new file stream in create mode
        FileStream fs = new FileStream(filePath, FileMode.Create);

        //  create our binary formatter to encrypt our party
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, party);

        //  close the file stream
        fs.Close();
    }

    //  Load our party
    public List<int> LoadParty()
    {
        //  if the file exists...
        if (File.Exists(filePath))
        {
            //  create a new file stream in open mode
            FileStream fs = new FileStream(filePath, FileMode.Open);

            //  create a binary formatter to decrypt our party
            BinaryFormatter bf = new BinaryFormatter();
            //  decryption
            List<int> party = bf.Deserialize(fs) as List<int>;

            //  close the file stream
            fs.Close();

            //  return the party
            return party;

        }
        //  otherwise, it doesn't exist
        else
        {
            Debug.LogError("Party not found in " + filePath);
            return null;
        }
    }

    public void SaveStats()
    {
        //  create a new file stream in create mode
        FileStream fs = new FileStream(statsPath, FileMode.Create);

        //  create a binary formatter to encrypt our stats
        BinaryFormatter bf = new BinaryFormatter();

        //  populate a list of stats
        //  1.  class - unit id (0 - 3)
        //  2.  dmg done
        //  3.  dmg taken
        //  4.  healing done
        //  5.  healing taken
        //  6.  alive/dead (0/1)
        
        //  cache the player controller to have access to the party
        PlayerController player = FindObjectOfType<PlayerController>();

        List<int> stats = new List<int>();

        //  iterate through the party, 
        foreach (Unit unit in player.party)
        {
            //  add in the unit id (to represent the class)
            stats.Add(unit.unitID);
            //  add in the damage done
            stats.Add(unit.dmgDone_val);
            //  add in the damage taken
            stats.Add(unit.dmgTaken_val);
            //  add in the healing done
            stats.Add(unit.healingDone_val);
            //  add in the healing taken
            stats.Add(unit.healingTaken_val);

            //  if the unit is dead: int = 1 (T)
            if (unit.IsDead) {  stats.Add(1);   }

            //  otherwise int = 0 (F)
            else { stats.Add(0); }
        }

        //  encrypt our stats
        bf.Serialize(fs, stats);

        //  close the file stream
        fs.Close();
    }

    public List<int> LoadStats()
    {
        //  if the file exists...
        if (File.Exists(statsPath))
        {
            //  create a new file stream in open mode
            FileStream fs = new FileStream(statsPath, FileMode.Open);

            //  create a binary formatter to decrypt our stats
            BinaryFormatter bf = new BinaryFormatter();
            //  decryption
            List<int> stats = bf.Deserialize(fs) as List<int>;

            //  close the file stream
            fs.Close();

            //  return the stats
            return stats;

        }
        //  otherwise, it doesn't exist
        else
        {
            Debug.LogError("Stats not found in " + statsPath);
            return null;
        }
    }
}
