using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerSlider : MonoBehaviour
{

    [SerializeField] Agents agents;
    [SerializeField] Slider m_slider;
    [SerializeField] PlayAndStop m_player;
    [SerializeField] ConsoleList m_console;
    [SerializeField] Map m_map;

    private int last_counter;
    private float makespan = 9999999f;
    private HashSet<Vector2Int> conflict_area = new HashSet<Vector2Int>();
    // Start is called before the first frame update
    void Start()
    {
        last_counter = agents.get_current_timestep();
  
    }


    public void set_makespan(int makespan)
    {
        this.makespan =Mathf.Max(0, makespan-1);
    }

    // Update is called once per frame
    void Update()
    {
        int current_T = agents.get_current_timestep();
   
        if (last_counter != current_T)
        {
            m_slider.value = current_T / makespan;
            last_counter = current_T;
            var conflicts = agents.get_conflicts_at_time(current_T);
            m_console.DisplayConflictsInfo(ref conflicts);

            //clear last highlight area
            foreach(var v in conflict_area)
            {
                m_map.highlight_conflict(v.x, v.y, false);
            }
            conflict_area.Clear();
            foreach(var cf in conflicts){
                if(cf.type==0)
                {
                    conflict_area.Add(cf.u);
                    m_map.highlight_conflict(cf.u.x, cf.u.y, true);
                }
                else if (cf.type == 1)
                {
                    conflict_area.Add(cf.u);
                    conflict_area.Add(cf.v);
                    //Debug.Log(cf.u + " to " + cf.v+"  "+(m_map==null));
                    m_map.highlight_conflict(cf.u.x, cf.u.y, true);
                    m_map.highlight_conflict(cf.v.x, cf.v.y, true);

                }
            }

        }
    }


    public void SliderBeingDrag()
    {
        //agents.puase_simulation(false); //pause 
        //int timestep =(int) (m_slider.value* makespan);
        //agents.reset_agents(timestep);
        //last_counter = timestep;


    }


    public void SliderAfterDrag()
    {
        //agents.puase_simulation(true);
    }
}
