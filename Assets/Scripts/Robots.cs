using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robots : MonoBehaviour
{
    // Start is called before the first frame update
    private int  number_robots;
    // Start is called before the first frame update
    [SerializeField]
    GameObject robotPrefab;
    GameObject[] robot_objects;

    public string solutionFile;


    void construct_from_file(string file_name){
        if(file_name.Length==0){
            Debug.Log("path file empty");
            return;
        } 
        Actions actions=new Actions(file_name);
        number_robots=actions.getNumAgents();
        robot_objects=new GameObject[number_robots];
        Vector2Int dims=actions.getDim();
        Debug.Log("dims="+dims);
        var starts=actions.get_starts();
        var goals=actions.get_goals();
        for(int k=0;k<number_robots;k++){
            var sk=starts[k];
            Vector3 position=new Vector3(sk.x+0.5f,sk.y+0.5f,-1);
            robot_objects[k]=Instantiate(robotPrefab,position,Quaternion.identity);
            robot_objects[k].transform.SetParent(transform);
            robot_objects[k].name="robot_"+k;
            KivaRobot ri=robot_objects[k].GetComponent<KivaRobot>();
            /////////////////////set color////////////////////////////////////
            List<string> actions_k=actions.getActionOfAgent(k);
            Vector2 goal=goals[k];
            Color ci=new Color(1.0f*goal[0]/dims[0],0.80f*goal[1]/dims[1],0.75f,1.0f);
            // Debug.Log("color="+ci);
            // ri.setColor(ci);
            for(int tk=0;tk<actions_k.Count;tk++){
                string at=actions_k[tk];
                // Debug.Log("debug"+pi);
                // Assert.IsTrue((pi-lastPoint).magnitude<=1.0f);
                ri.AddCmd(at);
            }
        }
    }


    
    void Start()
    {
        
      
        construct_from_file(solutionFile);
    }

}
