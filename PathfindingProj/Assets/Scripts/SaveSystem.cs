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
}
