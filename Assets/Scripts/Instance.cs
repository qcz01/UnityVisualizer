using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instance 
{
    // Start is called before the first frame update
    Instance(List<Vector2> starts,List<Vector2> goals){
        this.starts=starts;
        this.goals=goals;
    }
    List<Vector2> starts,goals;
}
