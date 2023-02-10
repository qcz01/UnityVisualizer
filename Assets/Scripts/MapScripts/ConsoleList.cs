using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleList : MonoBehaviour
{
    [SerializeField]
    private GameObject consoleItemPrefab;

    private List<GameObject> button_list = new List<GameObject>();
    public void DisplayConflictsInfo(ref List<Conflict> cfs)
    {

        clearConsole();
        //Debug.Log("display confict num="+cfs.Count);
        foreach(var conflict in cfs)
        {
            GameObject button = Instantiate(consoleItemPrefab) as GameObject;
            button.SetActive(true);
            string conflict_info = "";
            //vertex conflict
            if (conflict.type == 0)
            {
                conflict_info = "Vertex: A" + conflict.i + ",A" + conflict.j + " at " + conflict.u;
        
            }
            //edge conflict
            else if (conflict.type == 1)
            {
                conflict_info = "Edge: A" + conflict.i + ",A" + conflict.j + " at " + conflict.u + " -> " + conflict.v;
            }
            else if (conflict.type == 2)
            {
                conflict_info = "Obstacle: A" + conflict.i + " at " + conflict.u;
            }
            button.GetComponent<ConsoleItem>().SetText(conflict_info);
            button.transform.SetParent(consoleItemPrefab.transform.parent, false);
            button_list.Add(button);
        }


    }

    public void clearConsole()
    {
        //destroy previous items
        foreach (var button in button_list)
        {
            Destroy(button);
        }
        button_list.Clear();
    }

    void Start()
    {
        //LoadAllScenes();
        // Debug.Log("Maps read!");
        for (int j = 0; j < paths_files.Count; j++)
        {
            GameObject button = Instantiate(consoleItemPrefab) as GameObject;
            button.SetActive(true);

            button.GetComponent<ConsoleItem>().SetText(paths_files[j]);
            button.transform.SetParent(consoleItemPrefab.transform.parent, false);
        }
    }

    List<string> paths_files;
}
