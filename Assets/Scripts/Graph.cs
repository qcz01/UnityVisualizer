using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Graph
{
    public Graph (string file_name){
    
       Debug.Log("graph file "+file_name);
       string[] lines=File.ReadAllLines(file_name);
       int xi=0;
       string[] height_string=lines[1].Split(' ');
       string[] width_string=lines[2].Split(' ');
       ymax=int.Parse(height_string[1]);
       xmax=int.Parse(width_string[1]);
       Debug.Log("xmax,ymax="+xmax+","+ymax);
       grids=new int[xmax,ymax];
       for(int hi=4;hi<lines.Length;hi++){
           string grid_strings=lines[hi];
           for(int wi=0;wi<grid_strings.Length;wi++){
               if(grid_strings[wi]=='.') grids[wi,xi]=0;
               else grids[wi,xi]=1;
           }
           xi++;
       }
       Debug.Log("graph read!");
   }

   public int isBlocked(int x,int y){
       return grids[x,y];
   }

   public int getXmax(){
       return xmax;
   }
   public int getYmax(){
       return ymax;
   }
   int xmax,ymax; 
   
   int [,] grids;
}
