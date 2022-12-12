using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


/// <summary>
/// Action: WAIT FORWARD, CLOCKWISE, COUNTERCLOCK, LOAD,UNLOAD
/// </summary>
public enum Action
{
    /// <summary>
    /// Wait at its current vertex
    /// </summary>
    WAIT = 0,
    /// <summary>
    /// Move to the next vertex
    /// </summary>
    FORWARD = 1,
    /// <summary>
    /// rotate 90 degrees clockwise
    /// </summary>
    CLOCKWIISE = 2,
    /// <summary>
    /// rotate 90 degress counter clockwise
    /// </summary>
    COUNTERCLOCK = 3,
    /// <summary>
    /// UP
    /// </summary>
    UP = 4,
    /// <summary>
    /// move left
    /// </summary>
    LEFT = 5,
    /// <summary>
    /// move down
    /// </summary>
    DOWN = 6,
    /// <summary>
    /// move right
    /// </summary>
    RIGHT = 7
};


public class Actions 
{
    //F---forward,CW---Clockwise,CCW---CounterClockWise,W---wait
    List<List<string>> actions;
    Vector2Int dims;
    int scale=1;
    int makespan;
    int numAgents;
    List<Vector2> starts;
    List<Vector2> goals;

    void read_config(ref string config_string, ref List<Vector2> config){
        string[] start_vertices=config_string.Split(new string[]{"),"},System.StringSplitOptions.None);
        foreach(string vstring in start_vertices){
            if(vstring.Length<1) continue;
            string vn=vstring.Substring(1);  
            string []vs=vn.Split(',');
            config.Add(new Vector2(int.Parse(vs[0])*scale,int.Parse(vs[1])*scale));
        }
    }
    public Actions(string file_name){
        int xmax=0,ymax=0;
    
        int rotation_cost=1;
        actions=new List<List<string>>();
        starts=new List<Vector2>();
        goals=new List<Vector2>();
        string[] lines = File.ReadAllLines(file_name);
        foreach (string line in lines) {
            string[] size_strings=line.Split('=');
            if(size_strings.Length==2){
                if(size_strings[0]=="xmax"){ xmax=int.Parse(size_strings[1])*scale; continue;}
                if(size_strings[0]=="ymax"){ ymax=int.Parse(size_strings[1])*scale; continue;}
                if(size_strings[0]=="rotation_cost"){rotation_cost=int.Parse(size_strings[1])*scale; continue;}
                if(size_strings[0]=="starts"){
                    string starts_string=size_strings[1];
                    read_config(ref starts_string,ref starts);
                    continue;
                }
                if(size_strings[0]=="goals"){
                    string goals_string=size_strings[1];
                    read_config(ref goals_string,ref goals);
                    continue;
                }
            }
            string[] path_strings=line.Split(':');
            if(path_strings.Length<2) continue;
            List<string> pi=new List<string>();
           
            string[] vertex_strings=path_strings[1].Split(new string[]{","},System.StringSplitOptions.None);
            // Debug.Log("How many commands here?"+vertex_strings.Length);
            foreach(string vstring in vertex_strings){
                // Debug.Log(vstring+" debug vstring");
                if(vstring=="")continue;
                pi.Add(vstring);
            }
            actions.Add(pi);
        }
        dims=new Vector2Int(xmax,ymax);
        // Debug.Log("Read");
        numAgents=actions.Count; 
        makespan=0;
        for(int i=0;i<numAgents;i++){
            makespan=System.Math.Max(makespan,actions[i].Count);
        }
        // Debug.Log("Actions size="+actions[1].Count);
        // checkValid();
    }


    public int getMakespan(){
        return makespan;
    }

    public int getNumAgents(){
        return numAgents;
    }

    public string getVertex(int agent_id,int t){
        return actions[agent_id][t];
    }

    public List<string> getActionOfAgent(int agent_id){
        return actions[agent_id];
    }

    public List<Vector2> get_starts(){
        return starts;
    }

    public List<Vector2> get_goals(){
        return goals;
    }

    public Vector2Int getDim(){
        return dims;
    }
  
}
