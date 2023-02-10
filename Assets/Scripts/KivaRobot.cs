using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class KivaRobot : MonoBehaviour
{

    GameObject movingDirection;
    float speed = 0.50f;
    float Heading = 0;
    int rotation_cost = 1;

    private int counter = 0;
    private int goal_counter = 0;

    List<Vector2Int> goal_positions = new List<Vector2Int>();
    // List<Vector2> path=new List<Vector2>();
    //List<string> actions=new List<string>();
    List<ACTION> actions = new List<ACTION>();
    List<Vector3> path = new List<Vector3>();   
    bool if_pause = false;
    void Start()
    {
       
        //StartCoroutine(StartExecution());
    }

    public void AddState(Vector3 state)
    {
        // state = x, y, euler_angle
        path.Add(state);
    }


    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public void pause_agent(bool pause )
    {
        if_pause = pause;
    }

    private int label = 0;

    private bool show_goal = false;

    public void setLabel(int label)
    {
        this.label = label;
    }


    public void showGoal(bool show)
    {
        this.show_goal = show;
        if (show) initDirection();
        else Destroy(movingDirection);
    }

    void initDirection()
    {
        
        movingDirection = new GameObject();
        movingDirection.transform.position = transform.position;
        movingDirection.AddComponent<LineRenderer>();
        movingDirection.transform.parent = transform;
        LineRenderer lr = movingDirection.GetComponent<LineRenderer>();
        lr.SetWidth(0.5f, 0.5f);
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position);

    }


    void updateLine(Vector2Int goalPos)
    {
        //Debug.Log("current goal=" + goalPos);
        movingDirection.transform.position = transform.position;
        LineRenderer lr = movingDirection.GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, new Vector3(goalPos.x+0.5f,goalPos.y+0.5f,0));
    }

    public void setCounter(int counter)
    {
        this.counter = counter;
    }

    public int getCounter()
    {
        return counter; 
    }
    public void enableLabel()
    {
        transform.GetComponent<TextMesh>().text = label.ToString();
    }

    public void disableLabel()
    {
        transform.GetComponent<TextMesh>().text = "";
    }


    public void resetState(int t)
    {
        transform.position = new Vector3(path[t].x, path[t].y, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, path[t].z);
        counter = Mathf.Max(t - 1, 0);
    }

    public void simCmds()
    {
        counter = 0;
        goal_counter = 0;
        if(show_goal)initDirection();
        StartCoroutine(StartExecution());
    }


    public void setHeading(float heading)
    {
        this.Heading = heading;
    }

  

    // public void AddWayPoint(Vector2 point){
    //     path.Add(point);
    // }

    public void AddCmd(ACTION action){
        actions.Add(action);
    }


    public void AddTask(Vector2Int goal)
    {
        goal_positions.Add(goal);
    }

    // change the color of the robot
    public void setColor(Color colorx){
        // Debug.Log("color set!!");
        GameObject circle2=transform.Find("InerCircle").gameObject;
        var m_SpriteRenderer=circle2.GetComponent<SpriteRenderer>();
        m_SpriteRenderer.color=colorx;
    }

    // Update is called once per frame
    private float NormAngle(float angle){
        if(angle>=180) angle=angle-360;
        if(angle<-180) angle=angle+360;
        return angle;
    }


    public IEnumerator Corountine_RotateToAngle(float angle,float omega){
        Vector3 currentAngle=transform.eulerAngles;
        

        float duration =Mathf.Abs(currentAngle.z - angle) / omega;
        // Debug.Log("rotation duration="+duration+" omega="+omega+" currentAngle="+currentAngle.z+" angle="+angle);
        // Debug.Assert(duration<2);
        yield return StartCoroutine(Coroutine_RotateOverSeconds(transform.gameObject,angle,duration,omega));
        // yield return null;
    }

    public IEnumerator Coroutine_RotateOverSeconds(GameObject objectToMove, float angle,float seconds,float omega){
        float elapsedTime = 0;
        // if(seconds==0) {
        //     seconds=1.0f/speed;
        // }
        Vector3 startingAngle = objectToMove.transform.eulerAngles;
        Vector3 endAngle=new Vector3(startingAngle.x,startingAngle.y,angle);
        
        while (elapsedTime < seconds)
        {
            while (if_pause) yield return null;
            objectToMove.transform.eulerAngles =
            // linear interpolates between startingPos and End
              Vector3.Lerp(startingAngle, endAngle, (elapsedTime / seconds));      
             
            elapsedTime += Time.deltaTime;
            // Debug.Log(Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.eulerAngles= endAngle;
        counter++;
    }

    private Vector2 getNextForwardPoint(){
        Vector2 currentPosition=transform.position;
        Vector2 nextPoint=currentPosition;
        switch(Heading){
            case 0: nextPoint=currentPosition+new Vector2(1,0); break;
            case 90:nextPoint=currentPosition+new Vector2(0,1); break;
            case 180: nextPoint=currentPosition+ new Vector2(-1,0); break;
            case 270:nextPoint=currentPosition+ new Vector2(0,-1);break;
        }
        return nextPoint;
    }

    private float getNextAngle(string action){
        float angle=0;
        // int rotation_cost=3;
        switch(action){
            case "CW":angle=Heading-90.0f/rotation_cost;break;
            case "CCW":angle=Heading+90.0f/rotation_cost;break;
        }
        Heading=angle;
        if(Heading<0) Heading=Heading+360;
        if(Heading>=360) Heading=Heading-360;
        
        return angle;
    }


    //move to goal position
    public IEnumerator StartExecution()
    {
        // int rotation_cost=3;
        // yield return new WaitForSeconds(10);
        while (true)
        {
         
            while (counter<actions.Count)
            {
                ACTION action=actions[counter];
        
                switch(action){
                    case ACTION.FORWARD: var dg=getNextForwardPoint();yield return StartCoroutine(Coroutine_MoveToPoint(dg,speed));break;
                    case ACTION.COUNTERCLOCK: var angle=getNextAngle("CCW"); float omega=90.0f*speed/rotation_cost; yield return StartCoroutine(Corountine_RotateToAngle(angle,omega));break;
                    case ACTION.CLOCKWISE: angle=getNextAngle("CW");omega=90.0f*speed/rotation_cost; yield return StartCoroutine(Corountine_RotateToAngle(angle,omega));break;
                    case ACTION.WAIT: dg=transform.position;yield return StartCoroutine(Coroutine_MoveToPoint(dg,speed));break;
                    default: counter++; yield return null;break;
                }

                var dist = transform.position - new Vector3(goal_positions[goal_counter].x+0.5f, goal_positions[goal_counter].y+0.5f, transform.position.z);
                //Debug.Log("dist=" + transform.position + "  " + goal_positions[goal_counter]+"  "+dist.magnitude);
                if (dist.magnitude < 1e-1)
                {
                    goal_counter += 1;
                    if (goal_counter >= goal_positions.Count) goal_counter -= 1;
                }
                // dg=new Vector2(dg.x+0.5f,dg.y+0.5f);
               
                // yield return StartCoroutine(Coroutine_MoveToPoint(dg,speed));
                //  Debug.Log(dg +"  dequeued"+" Queue size= "+mWayPoints.Count+" time="+Time.time);
            }
            yield return null;
        }
    }



    IEnumerator Coroutine_MoveToPoint(Vector2 endP, float speed)
    {
      
        Vector2 currentPosition=transform.position;
        float duration = (currentPosition - endP).magnitude / speed;
       
        yield return StartCoroutine(Coroutine_MoveOverSeconds(transform.gameObject,endP,duration));
    }

    private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector2 end,float seconds)
    {
        float elapsedTime = 0;
        if(seconds==0) {
            seconds=1.0f/speed;
        }
        Vector3 startingPos = objectToMove.transform.position;
        Vector3 endPos=new Vector3(end.x,end.y,startingPos.z);
        
        while (elapsedTime < seconds)
        {
            while (if_pause) yield return null;
            objectToMove.transform.position =
            // linear interpolates between startingPos and End
              Vector3.Lerp(startingPos, endPos, (elapsedTime / seconds));      
             
            elapsedTime += Time.deltaTime;
            // Debug.Log(Time.deltaTime);
            if (show_goal)
            {
                if (goal_counter < goal_positions.Count)
                    updateLine(goal_positions[goal_counter]);
            }
            
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = endPos;
        counter++;
    }
}
