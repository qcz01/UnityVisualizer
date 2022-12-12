using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class KivaRobot : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(StartExecution());
    }

    float speed=5.0f;
    float Heading=0;
    int rotation_cost=1;
    
    private int counter=0;
    List<Vector2> path = new List<Vector2>(); // record the history path
    List<int> angles = new List<int>(); //record the history angles
    // List<Vector2> path=new List<Vector2>();
    List<string> actions=new List<string>();

    // public void AddWayPoint(Vector2 point){
    //     path.Add(point);
    // }

    public void AddCmd(string action){
        actions.Add(action);
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
                string action=actions[counter];
                Debug.Log("debug action="+action);
                switch(action){
                    case "F": var dg=getNextForwardPoint();yield return StartCoroutine(Coroutine_MoveToPoint(dg,speed));break;
                    case "CCW":var angle=getNextAngle("CCW");float omega=90.0f*speed/rotation_cost; yield return StartCoroutine(Corountine_RotateToAngle(angle,omega));break;
                    case "CW": angle=getNextAngle("CW");omega=90.0f*speed/rotation_cost; yield return StartCoroutine(Corountine_RotateToAngle(angle,omega));break;
                    case "W": dg=transform.position;yield return StartCoroutine(Coroutine_MoveToPoint(dg,speed));break;
                    default: counter++; yield return null;break;
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
            objectToMove.transform.position =
            // linear interpolates between startingPos and End
              Vector3.Lerp(startingPos, endPos, (elapsedTime / seconds));      
             
            elapsedTime += Time.deltaTime;
            // Debug.Log(Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = endPos;
        counter++;
    }
}
