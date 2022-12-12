using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapList : MonoBehaviour
{
    //List all the map names
    [SerializeField]
    private GameObject buttonPrefab;
    private void LoadAllMaps(){
        string path="./Assets/Resources/Maps/";
        mapnames=new List<string>();
        DirectoryInfo dir=new DirectoryInfo(path);
        FileInfo[] info=dir.GetFiles("*.map");
        Debug.Log(info.Length);
        foreach(FileInfo f in info){
            mapnames.Add(f.Name);
            Debug.Log(f.Name);
        }
        
    }

    void Start(){
        LoadAllMaps();
        Debug.Log("Maps read!");
        for(int j=0;j<mapnames.Count;j++){
            GameObject button=Instantiate(buttonPrefab) as GameObject;
            button.SetActive(true);

            button.GetComponent<MapItem>().SetText(mapnames[j]);
            button.transform.SetParent(buttonPrefab.transform.parent,false);
        }
    }

    List<string> mapnames;
}
