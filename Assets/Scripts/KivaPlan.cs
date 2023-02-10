using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class KivaPlan 
{
    List<List<Vector3Int>> plans=new List<List<Vector3Int>>();  //x,y,orientation




    public KivaPlan(string plan_name)
    {
        
        string[] lines = File.ReadAllLines(plan_name);
        foreach (string line in lines)
        {
            string[] size_strings = line.Split('=');
            string[] path_strings = line.Split(':');
            if (path_strings.Length < 2) continue;
            List<Vector3Int> pi = new List<Vector3Int>();
            path_strings[1] = path_strings[1].Substring(1);
            string[] vertex_strings = path_strings[1].Split(new string[] { "->" }, System.StringSplitOptions.None);
            foreach (string vstring in vertex_strings)
            {
                
                if (vstring.Length < 1) continue;
                //Debug.Log("vstring="+vstring);
                //string vn=vstring.Remove(0);
                //vn=vn.Remove(vn.Length - 1);
                // Debug.Log(vstring);
                string vn = vstring.Substring(1,vstring.Length-2);
                 //Debug.Log("vn="+vn);
                string[] vs = vn.Split(',');
                //for(int i = 0; i < vs.Length; i++)
                //{
                //    Debug.Log(vs[i]+"debugggg");
                //}
                //Debug.Log(vs[0]+","+vs[1]+","+vs[2]);
                pi.Add(new Vector3Int(int.Parse(vs[1]), int.Parse(vs[0]),int.Parse(vs[2])));
            }
            plans.Add(pi);
        }
        //dims = new Vector2Int(xmax, ymax);
        //// Debug.Log("Read");
        //numAgents = paths.Count;
        //makespan = soc = 0;
        //for (int i = 0; i < paths.Count; i++)
        //{
        //    makespan = System.Math.Max(makespan, paths[i].Count);
        //    soc += paths[i].Count;
        //}
    }


    public int size()
    {
        return plans.Count;
    }

    public int getMakespan()
    {
        return plans[0].Count;
    }

    public List<Vector3Int> getPlan(int agent)
    {
        return plans[agent];
    }

    public Vector3Int getState(int agent,int t)
    {
        int T = plans[agent].Count;
        if (t < T) return plans[agent][t];
        else return plans[agent][T - 1];
    }


    public List<Vector3Int> getConfig(int t)
    {
        List<Vector3Int> config = new List<Vector3Int>();
        for(int i = 0; i < plans.Count; i++)
        {
            config.Add(getState(i, t));   
        }
        return config;
    }

    public Vector3Int getGoal(int i)
    {
        return plans[i][plans[i].Count - 1];
    }

   
}
