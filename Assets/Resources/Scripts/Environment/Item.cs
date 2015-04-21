using UnityEngine;
using System.Collections.Generic;

using SimpleJSON;

public class Item
{
    string name;

    public Item(string name_)
    {
        name = name_;
    }

    public string GetName()
    {
        return name;
    }

    public Item Clone()
    {
        Item item = new Item(HTNPlanner.GenerateUniqueID(name));

        HTNWorldManager.instance.GetGlobalState().AddObject(item.GetName(), item);

        return item;
    }

    #region static

    public static void OnStartup()
    {
        Load("Data/Items");
    }

    static Dictionary<string, Item> dictItems = new Dictionary<string, Item>();
    public static Dictionary<string, Item> DictItems
    {
        get { return dictItems; }
    }

    public static bool Load(string filePath_)
    {
        TextAsset asset = (TextAsset)Resources.Load(filePath_);

        if (asset == null)
        {
            Debug.Log("file '" + filePath_ + "' could not be loaded");
            return false;
        }

        JSONNode jsonData = JSON.Parse(asset.text);

        for (int i = 0; i < jsonData.Count; ++i)
        {
            string name = jsonData[i]["Name"];

            Item item = new Item(name);

            dictItems.Add(item.GetName(), item);
        }

        Debug.Log(dictItems.Count + " Items loaded from '" + filePath_ + "'");
        return true;

    }

    public static Item GetItem(string name_)
    {
        if(dictItems.ContainsKey(name_))
        {
            return dictItems[name_];
        }

        return null;
    }

    #endregion static

}
