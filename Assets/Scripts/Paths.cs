using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class Paths 
{
    
    List<List<Vector2>>  paths;  
    public Paths(string file_name){
        int xmax=0,ymax=0;
        int scale=1;
        paths=new List<List<Vector2>>();
        string[] lines = File.ReadAllLines(file_name);
        foreach (string line in lines) {
            string[] size_strings=line.Split('=');
            if(size_strings.Length==2){
                if(size_strings[0]=="xmax"){ xmax=int.Parse(size_strings[1])*scale; continue;}
                if(size_strings[0]=="ymax"){ ymax=int.Parse(size_strings[1])*scale; continue;}
            }
            string[] path_strings=line.Split(':');
            if(path_strings.Length<2) continue;
            List<Vector2> pi=new List<Vector2>();
            string[] vertex_strings=path_strings[1].Split(new string[]{"),"},System.StringSplitOptions.None);
       
            foreach(string vstring in vertex_strings){
                
            
                if(vstring.Length<1) continue;
                // Debug.Log(vstring);
                string vn=vstring.Substring(1);  
                // Debug.Log(vn);
                string []vs=vn.Split(',');
                // Debug.Log(vs[0]+","+vs[1]+","+vs[2]);
                pi.Add(new Vector2(int.Parse(vs[0])*scale,int.Parse(vs[1])*scale));
            }
            paths.Add(pi);
        }
        dims=new Vector2Int(xmax,ymax);
        // Debug.Log("Read");
        numAgents=paths.Count; 
        makespan=soc=0;
        for(int i=0;i<paths.Count;i++){
            makespan=System.Math.Max(makespan,paths[i].Count);
            soc+=paths[i].Count;
        }
        // checkValid();
    }


    private float distance(Vector2 v1,Vector2 v2){
        return  Math.Abs(v1[0]-v2[0])+Math.Abs(v1[1]-v2[1]);
    }


    public  bool checkValid(){
        //format
        for(int i=0;i< numAgents;i++){
            if(paths[i].Count==makespan) continue;
            while(paths[i].Count<makespan){
                int l=paths[i].Count;
                paths[i].Add(paths[i][l-1]);
            }
        }
    
        for(int t=1;t<makespan;t++){
            for(int j=0;j<numAgents;j++){
                for(int k=j+1;k<numAgents;k++){
                    //vertex
                    if(paths[j][t]==paths[k][t]) throw  new Exception();
                    if(paths[j][t-1]==paths[k][t] && paths[k][t-1]==paths[j][t]) throw new Exception();
                }
            }
        }
   
        for(int k=0;k<numAgents;k++){
            for(int ti=0;ti<paths[k].Count-1;ti++){
                if(distance(paths[k][ti],paths[k][ti+1])>1.001f) {
                    Debug.Log(k+" "+ti+" "+paths[k][ti]+"-->"+paths[k][ti+1]);
                    throw new Exception();
                }

            }
        }
        return true;
    }

    public int getMakespan(){
        return makespan;
    }

    public int getSOC(){
        return soc;
    }

    public int getNumAgents(){
        return numAgents;
    }

    public Vector2 getVertex(int agent_id,int t){
        return paths[agent_id][t];
    }

    public List<Vector2> getPath(int agent_id){
        return paths[agent_id];
    }

    public Vector2Int getDim(){
        return dims;
    }
    int makespan;
    int numAgents=0;
    Vector2Int dims;
    int soc;
}
