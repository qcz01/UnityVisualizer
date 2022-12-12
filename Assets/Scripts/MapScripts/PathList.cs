using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class PathList : MonoBehaviour
{
    // Start is called before the first frame update   
    [SerializeField]
    private GameObject pathItemPrefab;
    private void LoadAllScenes(){
        string path="./Assets/Resources/PathFile/";
        paths_files=new List<string>();
        DirectoryInfo dir=new DirectoryInfo(path);
        FileInfo[] info=dir.GetFiles("*.txt");
        Debug.Log(info.Length);
        foreach(FileInfo f in info){
            paths_files.Add(f.Name);
            Debug.Log(f.Name);
        }
        
    }

    void Start(){
        LoadAllScenes();
        // Debug.Log("Maps read!");
        for(int j=0;j<paths_files.Count;j++){
            GameObject button=Instantiate(pathItemPrefab) as GameObject;
            button.SetActive(true);

            button.GetComponent<MapItem>().SetText(paths_files[j]);
            button.transform.SetParent(pathItemPrefab.transform.parent,false);
        }
    }

    List<string> paths_files;
}
