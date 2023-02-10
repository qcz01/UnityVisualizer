using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Agents : MonoBehaviour
{
    private int number_robots;
    private int rotation_cost;

    // Start is called before the first frame update
    [SerializeField]
    GameObject holoAgentPrefab;
    [SerializeField]
    GameObject rotateAgentPrefab;
    [SerializeField] private Map map;
    List<GameObject> robot_objects=new List<GameObject>();
    // HolonomicAgent[] robots;
    [SerializeField] playerSlider m_slider;
    KivaPlan actual;

    //[SerializeField] ConsoleList m_console;

   
    private int makespan;
    //public string solutionFile;


    private List<List<Conflict>> conflicts_info = new List<List<Conflict>>();



    public List<Conflict> get_conflicts_at_time(int t)
    {
        if (t < conflicts_info.Count) return conflicts_info[t];
        else return new List<Conflict>();
    }

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

    public void show_goals(bool goal_enable)
    {
        for(int i = 0; i < number_robots; i++)
        {
            robot_objects[i].GetComponent<KivaRobot>().showGoal(goal_enable);
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


    private void find_all_conflicts(ref KivaPlan plan)
    {
        int xmax = map.getXmax();
        int ymax = map.getYmax();
        //bruteforce
        for(int t = 1; t < makespan; t++)
        {
            List<Conflict> cfs = new List<Conflict>();
            for(int i = 0; i < number_robots; i++)
            {
                Vector2Int vit = (Vector2Int)plan.getState(i, t);
                Vector2Int vitt = (Vector2Int)plan.getState(i, t - 1);
                for(int j = i + 1; j < number_robots; j++)
                {
                    var vjt = (Vector2Int)plan.getState(j, t);
                    var vjtt =(Vector2Int) plan.getState(j, t - 1);
                    //Debug.Log(vit+" "+vitt+" "+vjt+" "+vjtt);
                    if(vit==vjt)
                    {
                        cfs.Add(new Conflict(i, j, vit, t));
                    }
                    if(vit==vjtt && vitt == vjt)
                    {
                        cfs.Add(new Conflict(i, j, vit, vitt, t));
                    }
                    
                }
            }
            conflicts_info.Add(cfs);
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


    public int get_current_timestep()
    {
        if (robot_objects.Count == 0) return 0;
        var r0 = robot_objects[0].GetComponent<KivaRobot>();
        return r0.getCounter();
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


    public void reset_agents(int t)
    {
        for(int i = 0; i < number_robots; i++)
        {
            robot_objects[i].GetComponent<KivaRobot>().resetState(t);
        }
    }

    public void initialize_from_startKits(string actual_file, string task_file,float speed=1.0f)
    {
        rotation_cost = 1;

        actual = new KivaPlan(actual_file);
        makespan = actual.getMakespan();
        m_slider.set_makespan(makespan);
        number_robots = actual.size();
        Debug.Log("number of robots=" + number_robots);
        find_all_conflicts(ref actual);
        Debug.Log("find all conflicts finished");
        var starts = actual.getConfig(0);
        //List<Conflict> test = new List<Conflict>();
        //test.Add(new Conflict(1, 2,new Vector2Int(1, 1), 0));
        //m_console.DisplayConflictsInfo(ref test);
       
        int xmax = map.getXmax();
        int ymax = map.getYmax();
        for (int k = 0; k < number_robots; k++)
        {
            var sk = starts[k];
            Vector3 position = new Vector3(sk.x + 0.5f, sk.y + 0.5f, -1);
            GameObject rk = Instantiate(rotateAgentPrefab, position, Quaternion.identity);

            rk.transform.SetParent(transform);
            rk.name = "robot_" + k;
            KivaRobot ri = rk.GetComponent<KivaRobot>();
            float init_angle = 0f;
            switch (sk.z)
            {
                case 0: init_angle = 0f; break;
                case 1: init_angle = 270f; break;
                case 2: init_angle = 180f; break;
                case 3: init_angle = 90f; break;
            }
            rk.transform.eulerAngles = new Vector3(0, 0, init_angle);
            ri.setHeading(init_angle);
            ri.setLabel(k);
            ri.setSpeed(speed);
            /////////////////////set color////////////////////////////////////

            Vector3Int goal = actual.getGoal(k);

            Color ci = new Color(1.0f * goal[0] / xmax, 0.80f * goal[1] / ymax, 0.75f, 1.0f);
            
            foreach(Vector3Int state in actual.getPlan(k))
            {
                float euler_angle=0f;
                switch (state.z)
                {
                    case 0: euler_angle = 0f; break;
                    case 1: euler_angle = 270f; break;
                    case 2: euler_angle = 180f; break;
                    case 3: euler_angle = 90f; break;
                }
                ri.AddState(new Vector3(state.x,state.y,euler_angle));
            }

            var actions = transform_from_states(actual.getPlan(k));
            ri.setColor(ci);
            for (int tk = 0; tk < actions.Count; tk++)
            {
                ri.AddCmd(actions[tk]);
            }
            robot_objects.Add(rk);
        }
        //read tasks
        string[] lines = File.ReadAllLines(task_file);
        int agent_id = -1;
        foreach (string line in lines)
        {
            if (line.Length == 0) continue;
            if (line[0] == '#') continue;
            string[] vs = line.Split(',');
            if (vs.Length == 1 && agent_id == -1) agent_id++;
            else
            {
                for(int i=2;i<vs.Length;i++)
                {
                    var vg = vs[i];
                    int goal_id = int.Parse(vg);
                    int x = goal_id / xmax;
                    int y = goal_id % xmax;
                    robot_objects[agent_id].GetComponent<KivaRobot>().AddTask(new Vector2Int(y, x));
                    //Debug.Log("add goal to agent" + agent_id + " " + new Vector2Int(y, x));
                }
                agent_id++;
            }

        }
    }


    //Assume that the states are feasible
    List<ACTION> transform_from_states(List<Vector3Int> states)
    {

        List<ACTION> actions = new List<ACTION>();
        for (int t = 1; t < states.Count; t++)
        {
            if (Mathf.Abs(states[t].x - states[t - 1].x) + Mathf.Abs(states[t].y - states[t - 1].y) == 1)
            {
                actions.Add(ACTION.FORWARD);
            }
            else
            {
                int d_angle = states[t].z - states[t - 1].z;
                if (d_angle == 1 || d_angle == -3) actions.Add(ACTION.CLOCKWISE);
                else if (d_angle == -1 || d_angle == 3) actions.Add(ACTION.COUNTERCLOCK);
                else if (d_angle == 0) actions.Add(ACTION.WAIT);
            }
        }
 
        return actions;
    }


    public void puase_simulation(bool pause)
    {
        for(int i = 0; i < robot_objects.Count; i++)
        {
            robot_objects[i].GetComponent<KivaRobot>().pause_agent(pause);
        }
    }
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
