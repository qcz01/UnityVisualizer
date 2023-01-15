using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolonomicAgent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Coroutine_MoveTo());
    }

    float speed=5.0f;
    private int counter=0;
    private int MAX_NUM_CMDS = 500;
    List<Vector2> path=new List<Vector2>();
    List<ACTION> actions=new List<ACTION>();

    public void AddCmd(ACTION action)
    {
        actions.Add(action);
    }

    public int getNumberOfCmds()
    {
        return actions.Count;
    }


    private int label = 0;

    public void setLabel(int label)
    {
        this.label = label;
    }

    public void enableLabel()
    {
        transform.GetComponent<TextMesh>().text = label.ToString();
    }

    public void disableLabel()
    {
        transform.GetComponent<TextMesh>().text = "";
    }

    public void simPaths()
    {
        counter = 0;
        StartCoroutine(Coroutine_MoveTo());
    }

    public void simCmds()
    {
        counter = 0;
        Debug.Log("coroutine started  !");
        StartCoroutine(Coroutine_ExecuteCmd());
    }

    public void AddWayPoint(Vector2 point){
        path.Add(point);
    }

    // change the color of the robot
    public void setColor(Color colorx){
        // Debug.Log("color set!!");
        GameObject circle2=transform.Find("Circle2").gameObject;
        var m_SpriteRenderer=circle2.GetComponent<SpriteRenderer>();
        m_SpriteRenderer.color=colorx;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        counter = 0;
        Vector3 start = new Vector3(path[0].x + 0.5f, path[0].y + 0.5f,transform.position.z);
        this.transform.position = start;
    }


    //move to goal position
    public IEnumerator Coroutine_MoveTo()
    {
        // yield return new WaitForSeconds(10);
        while (true)
        {
            //  rotate_propellers();
            while (counter<path.Count)
            {
                Vector2 dg=path[counter];
                dg=new Vector2(dg.x+0.5f,dg.y+0.5f);
               
                yield return StartCoroutine(Coroutine_MoveToPoint(dg,speed));
                //  Debug.Log(dg +"  dequeued"+" Queue size= "+mWayPoints.Count+" time="+Time.time);
            }
            yield return null;
        }
    }


    public IEnumerator Coroutine_ExecuteCmd()
    {
        // yield return new WaitForSeconds(10);
        
        while (true)
        {
            //  rotate_propellers();
            //Debug.Log(counter+" ???? "+actions.Count);
            while (counter < actions.Count)
            {
                //Vector2 dg = path[counter];
                ACTION action = actions[counter];
                var dg = step(action);
                //dg = new Vector2(dg.x + 0.5f, dg.y + 0.5f);

                yield return StartCoroutine(Coroutine_MoveToPoint(dg, speed));
                //  Debug.Log(dg +"  dequeued"+" Queue size= "+mWayPoints.Count+" time="+Time.time);
            }
            yield return null;
        }
    }


    Vector2 step(ACTION action)
    {
        Vector2 result = new Vector2(transform.position.x, transform.position.y);
        switch (action)
        {
            case ACTION.WAIT:
                break;
            case ACTION.UP:
                result.y++;
                break;
            case ACTION.DOWN:
                result.y--;
                break;
            case ACTION.LEFT:
                result.x--;
                break;
            case ACTION.RIGHT:
                result.x++;
                break;
            default:
                break;
        }
        return result;
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
