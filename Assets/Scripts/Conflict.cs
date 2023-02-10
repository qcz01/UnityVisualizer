using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conflict 
{
    // Start is called before the first frame update
    public int i, j, t;
    public Vector2Int u, v;
    public int type = 0;//0 is for vertex conflict, 1 is for edge conflict, 3 is for colliding with obstacles
    public Conflict(int i,int j,Vector2Int u, int t)
    {
        this.i = i;
        this.j = j;
        this.u = u;
        this.t = t;
        this.type = 0;//vertex conflict
    }

    public Conflict(int i,int j, Vector2Int u, Vector2Int v,int t)
    {
        this.i = i;
        this.j = j;
        this.u = u;
        this.v = v;
        this.t = t;
        this.type = 1;//edge conflict
    }

    public Conflict(int i,Vector2Int u, int t)
    {
        this.i = i;
        this.u = u;
        this.type = 3;// obstacle 
    }
}
