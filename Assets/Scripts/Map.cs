using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Tile obstacleTile;
    [SerializeField]
    Tile freeTile;
 

    [SerializeField]
    Tile highlightTile;
    [SerializeField]
    Tilemap mapf_map;
       
    //public string map_file="./Assets/Resources/Maps/random-32-32-10.map";

    int xmax;
    int ymax;
    

    void resetCamera(int mX,int mY){
       
        Debug.Log("resetting camera");
        Debug.Log(mX + " " + mY);
        Camera.main.orthographicSize = mY / 2.0f + 1.0f;
        Camera.main.transform.position = new Vector3(mX / 2.0f - 0.5f, mY / 2.0f - 0.5f, -100.0f);
    }
 
  
    void Start()
    {
     
    }


    public void highlight_conflict(int x,int y,bool conflict)
    {
        if(conflict==true) mapf_map.SetTile(new Vector3Int(x, y, 0), highlightTile);
        else mapf_map.SetTile(new Vector3Int(x, y, 0), freeTile);
    }

    public void build_map(string file_name)
    {
        //mapf_map = GetComponent<Tilemap>();
        //Debug.Log("map name=" + map_file);
        Graph graph = new Graph(file_name);
        resetCamera(graph.getXmax(), graph.getYmax());
        xmax = graph.getXmax();
        ymax = graph.getYmax();
        var camera=GameObject.Find("Main Camera").GetComponent<CameraControl>();
        camera.setCameraBound(-1, xmax + 1, -1, ymax + 1);
        for (int xi = 0; xi < graph.getXmax(); xi++)
        {
            for (int yi = 0; yi < graph.getYmax(); yi++)
            {
                if (graph.isBlocked(xi, yi) == 0) mapf_map.SetTile(new Vector3Int(xi, yi, 0), freeTile);
                else mapf_map.SetTile(new Vector3Int(xi, yi, 0), obstacleTile);
            }
        }
    }

    public int getXmax()
    {
        return xmax;
    }

    public int getYmax()
    {
        return ymax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
