using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Agents : MonoBehaviour
{
    private int number_robots;
    private int rotation_cost;

    // Start is called before the first frame update
    [SerializeField]
    GameObject holoAgentPrefab;
    [SerializeField]
    GameObject rotateAgentPrefab;
    List<GameObject> robot_objects=new List<GameObject>();
    // HolonomicAgent[] robots;

    //public string solutionFile;

    public void send_cmd(int i,ACTION action)
    {
        if (rotation_cost == 0)
        {
            var rk = robot_objects[i];
            var agent = rk.GetComponent<HolonomicAgent>();
            agent.AddCmd(action);
        }
        else
        {
            var rk = robot_objects[i];
            var agent = rk.GetComponent<KivaRobot>();
            agent.AddCmd(action);
        }
     
    }

    public void enable_labels()
    {
        if (rotation_cost == 0)
        {
            for (int k = 0; k < robot_objects.Count; k++)
            {
                var rk = robot_objects[k];
                var agent = rk.GetComponent<HolonomicAgent>();
                agent.enableLabel();
            }
        }
        else
        {
            for(int k = 0; k < robot_objects.Count; k++)
            {
                var rk = robot_objects[k];
                var agent = rk.GetComponent<KivaRobot>();
                agent.enableLabel();
            }
            
        }
    }

    public void disable_labels()
    {
        if (rotation_cost == 0)
        {
            for (int k = 0; k < robot_objects.Count; k++)
            {
                var rk = robot_objects[k];
                var agent = rk.GetComponent<HolonomicAgent>();
                agent.disableLabel();
            }
        }
        else
        {
            for (int k = 0; k < robot_objects.Count; k++)
            {
                var rk = robot_objects[k];
                var agent = rk.GetComponent<KivaRobot>();
                agent.disableLabel();
            }
        }
    }


    public int get_num_agents()
    {
        return robot_objects.Count;
    }

    public void begin_simulate()
    {
        //Debug.Log("begin simulate");
        if (rotation_cost == 0)
        {
            for (int i = 0; i < robot_objects.Count; i++)
            {
                var agent = robot_objects[i].GetComponent<HolonomicAgent>();
                agent.simCmds();
            }
        }
        else
        {
            for (int i = 0; i < robot_objects.Count; i++)
            {
                var agent = robot_objects[i].GetComponent<KivaRobot>();
                agent.simCmds();
            }
        }
     
    }

    public void build_from_file(string file_name, int xmax = 32, int ymax = 32,bool use_labels=false,int rotation_cost=0)
    {
        //List<Vector2Int> starts = new List<Vector2Int>();
        string[] lines = File.ReadAllLines(file_name);
        int k = 0;
        this.rotation_cost = rotation_cost;
        foreach (string line in lines)
        {
            string[] sg_strings = line.Split(' ');
            if (sg_strings.Length == 4)
            {
                int sx = int.Parse(sg_strings[0]);
                int sy = int.Parse(sg_strings[1]);
                int gx = int.Parse(sg_strings[2]);
                int gy = int.Parse(sg_strings[3]);
                Vector3 position = new Vector3(sx + 0.5f, sy + 0.5f, -1);
                if (rotation_cost == 0)
                {
                    GameObject rk = Instantiate(holoAgentPrefab, position, Quaternion.identity);
                    rk.transform.SetParent(transform);
                    HolonomicAgent ri = rk.GetComponent<HolonomicAgent>();
                    if (use_labels) ri.setLabel(k);
                    rk.name = "robot_" + k;
                    robot_objects.Add(rk);
                    Color ci = new Color(1.0f * gx / xmax, 0.80f * gy / ymax, 0.75f, 1.0f);
                    ri.setColor(ci);
                    ri.setLabel(k);
                }
                else
                {
                    GameObject rk = Instantiate(rotateAgentPrefab, position, Quaternion.identity);
                    rk.transform.SetParent(transform);
                    //rk.GetComponent<HolonomicAgent>().enabled = false;
                    //rk.GetComponent<KivaRobot>().enabled = false;
                    KivaRobot ri = rk.GetComponent<KivaRobot>();
                    //if (use_labels) ri.setLabel(k);
                    rk.name = "robot_" + k;
                    robot_objects.Add(rk);
                    Color ci = new Color(1.0f * gx / xmax, 0.80f * gy / ymax, 0.75f, 1.0f);
                    ri.setColor(ci);
                    ri.setLabel(k);
                }
               

                k++;         
            }
        }
    }

    //void construct_from_file(string file_name){
    //    if(file_name.Length==0){
    //        //Debug.Log("path file empty");
    //        return;
    //    } 
    //    Paths paths=new Paths(file_name);
    //    number_robots=paths.getNumAgents();
    //    robot_objects=new GameObject[number_robots];
    //    Vector2Int dims=paths.getDim();
    //    Debug.Log("dims="+dims);
    //    for(int k=0;k<number_robots;k++){
    //        var sk=paths.getVertex(k,0);
    //        Vector3 position=new Vector3(sk.x+0.5f,sk.y+0.5f,-1);
    //        robot_objects[k]=Instantiate(robotPrefab,position,Quaternion.identity);
    //        robot_objects[k].transform.SetParent(transform);
    //        robot_objects[k].name="robot_"+k;
    //        HolonomicAgent ri=robot_objects[k].GetComponent<HolonomicAgent>();
    //        /////////////////////set color////////////////////////////////////
    //        List<Vector2> path=paths.getPath(k);
    //        Vector2 goal=path[path.Count-1];
    //        Color ci=new Color(1.0f*goal[0]/dims[0],0.80f*goal[1]/dims[1],0.75f,1.0f);
    //        // Debug.Log("color="+ci);
    //        ri.setColor(ci);
    //        for(int tk=0;tk<path.Count;tk++){
    //            Vector2 pi=paths.getVertex(k,tk);
    //            // Debug.Log("debug"+pi);
    //            // Assert.IsTrue((pi-lastPoint).magnitude<=1.0f);
    //            ri.AddWayPoint(pi);
    //        }
    //    }
    //}
    void Start()
    {
        
        //var selcted_map  =  PlayerPrefs.GetString("selcted_map");
        //Debug.Log(selcted_map+"  selected!!!!!");
        //construct_from_file(solutionFile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
