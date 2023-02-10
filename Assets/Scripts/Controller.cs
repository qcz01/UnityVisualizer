using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Newtonsoft.Json;

using NetMQ;
using NetMQ.Sockets;

public enum ACTION
{
    WAIT = 0,
    UP = 1,
    LEFT = 2,
    DOWN = 3,
    RIGHT = 4,
    FORWARD = 5,
    CLOCKWISE = 6,
    COUNTERCLOCK = 7
}

public class Controller : MonoBehaviour
{

    [SerializeField] private Agents agents;
    [SerializeField] private Map map;

    //public string listenting_port = "";
    bool has_rotations = false;
    private bool requesterIsStarted = false;
    private string inMsg = "";
    private List<Queue<ACTION>> cmds;
    private SubscriberSocket sub=null;
    bool canReplay = false;
    bool enable_listener = false;
    
    private Queue<List<ACTION>> buffer=new Queue<List<ACTION>>();
    GameObject mapObject = null;

    bool simulating_start = false;
    bool use_labels = false;
    private float speed=1.0f;

    private string map_file="./Assets/Resources/Maps/den312d.map";
    private string paths_file="./Assets/Resources/Paths/den312_demo.txt";
    private string instance_file= "./Assets/Resources/Instances/den312d/den312d_agents20_0.scen";
    private string action_file="";
    private string json_config="";
    private string task_file = "";
    private string plan_file = ""; //used for start kit plan 


    private int rotation_cost=0;

    

    void subscriber()
    {
        AsyncIO.ForceDotNet.Force();
        //client = new RequestSocket("tcp://localhost:5556");
        sub = new SubscriberSocket();
        //sub.Bind("tcp://localhost:5556");
        sub.Connect("tcp://localhost:5556");
        sub.Subscribe(string.Empty);

        //var agents = agentsObject.GetComponent<Agents>();
        //agentsObject = GameObject.Find("Agents");
        //var agents = agentsObject.GetComponent<Agents>();
        Debug.Log("gt listening to tcp://localhost:5556");
        try
        {
            while (requesterIsStarted)
            {
                string msg;
                
                if (sub.TryReceiveFrameString(TimeSpan.FromSeconds(3), out msg))
                {
                    inMsg = msg;
                    
                    Debug.Log(inMsg+"  inMSG");

                    if (!string.Equals(inMsg, ""))
                    {
                        //Debug.Log("process string:" + inMsg);
                        var action_map = JsonConvert.DeserializeObject<Dictionary<string, int>>(inMsg);
                        List<ACTION> tmp = new List<ACTION>();
                        foreach (KeyValuePair<string, int> entry in action_map)
                        {
                            int id = int.Parse(entry.Key);
                            if (entry.Value > 7) continue;
                            ACTION action = (ACTION)(entry.Value);
                            tmp.Add(action);
                            

                            //Debug.Log(action);
                        // do something with entry.Value or entry.Key
                        }
                        buffer.Enqueue(tmp);
                    }
                    inMsg = "";
                }
            }
        }
        finally
        {
            if (sub != null)
            {
                ((IDisposable)sub).Dispose();
            }
        }
    }

    private void OnApplicationQuit()
    {
        //Debug.Log("close client");
        if(sub!=null)sub.Close();
        
        NetMQConfig.Cleanup();
        //server.Close();
    }

    void startSubscribing()
    {
        requesterIsStarted = true;
        Task task = new Task(async () => subscriber());
        task.Start();
        //Debug.Log("listening");
    }
    // Start is called before the first frame update
    void Start()
    {
        //demo();
        startKit_exe();
    

    }

    void demo()
    {
        //string exe_plan = "./Assets/kits/exp/test_actual.txt";
        //string task_file = "./Assets/kits/maps/tasks_room.txt";
        //map_file = "./Assets/kits/maps/room-32-32-4.map";

        plan_file = "./Assets/kits/exp/6x6demo.txt";
        task_file = "./Assets/kits/maps/6x6task.txt";
        map_file = "./Assets/Resources/Maps/6x6.map";
        //mapObject = GameObject.Find("Tilemap");
        //var map = mapObject.GetComponent<Map>();
        map.build_map(map_file);
        //var agentsObject = GameObject.Find("Agents");
        //agents = agentsObject.GetComponent<Agents>();

        agents.initialize_from_startKits(plan_file,task_file);

        agents.begin_simulate();

    }


    void startKit_exe()
    {
        //default
        plan_file = "./Assets/kits/exp/6x6demo.txt";
        task_file = "./Assets/kits/maps/6x6task.txt";
        map_file = "./Assets/Resources/Maps/6x6.map";
        runWithExternalCommand();
        map.build_map(map_file);
        agents.initialize_from_startKits(plan_file, task_file,speed);
        agents.begin_simulate();
    }

    void simulator_exe()
    {
        runWithExternalCommand();
        ini_environment();
        if (enable_listener)
        {
            startSubscribing();
        }
        //System.Threading.Thread.Sleep(10000);
        //var agents = agentsObject.GetComponent<Agents>();
        agents.begin_simulate();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("updating !    " +agents.get_num_agents());
        while (buffer.Count > 0)
        {
            var data = buffer.Dequeue();
            for(int i = 0; i < agents.get_num_agents(); i++)
            {
                agents.send_cmd(i, data[i]);
                //Debug.Log("sent cmd");
            }
        }
    }

    void ini_environment()
    {
        //mapObject = GameObject.Find("Tilemap");
        //var map = mapObject.GetComponent<Map>();
        map.build_map(map_file);

        if (instance_file.Length != 0)
        {
            //var agentsObject = GameObject.Find("Agents");
            //agents = agentsObject.GetComponent<Agents>();

            agents.build_from_file(instance_file,map.getXmax(),map.getYmax(),rotation_cost:rotation_cost);
        }
    }


    public void Quit()
    {
        #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                         Application.Quit();
        #endif
    }



    void runWithExternalCommand()
    {

        string[] args = System.Environment.GetCommandLineArgs();
        bool map_name = false, instance_name = false, paths_name = false, actions_name=false;
        for (int i = 0; i < args.Length; i++)
        {
            if (i + 1 < args.Length)
            {
                switch (args[i])
                {
                    case "--map":
                        map_file = args[i + 1];
                        //PlayerPrefs.SetString("selected_map", args[i + 1]);
                        map_name = true;
                        break;
                    case "--instance":
                        instance_file = args[i + 1];
                        //PlayerPrefs.SetString("selected_instance", args[i + 1]);
                        instance_name = true;
                        break;
                    case "--paths":
                        instance_file = args[i + 1];
                        //PlayerPrefs.SetString("selected_paths", args[i + 1]);
                        paths_name = true;
                        break;
                    case "--actions":
                        action_file = args[i + 1];
                        actions_name = true;
                        break;
                    case "--rotation_cost":
                        rotation_cost = int.Parse(args[i + 1]);
                        break;
                    case "--enable_listener":
                        enable_listener = false;
                        break;
                    case "--speed":
                        speed = float.Parse(args[i + 1]);
                        break;
                    case "--labels":
                        use_labels =bool.Parse(args[i + 1]);
                        break;
                    case "--plan":
                        plan_file = args[i + 1];
                        break;
                    case "--task":
                        task_file = args[i + 1];
                        break;
                    default:
                        break;
                }
            }
        }
        if (map_name && paths_name)
        {
            //PlayerPrefs.SetInt("visual", 1);
            //loadScene("Simulator");
            //Quit();
        }
 
        if (map_name == false)
        {
            System.Console.WriteLine("no map file!  Using the default map");
            //Quit();
        }
        if (instance_name == false)
        {
            System.Console.WriteLine("no instance file!");
            //Quit();
        }
        if (paths_name == false)
        {
            System.Console.WriteLine("no paths file!");
            //Quit();
        }


    }

}
