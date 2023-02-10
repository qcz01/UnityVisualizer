using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class console : MonoBehaviour
{
    [SerializeField]
    private Text currentStep;
    [SerializeField]
    private Agents agents;
    
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        if (counter == 10)
        {
            counter = 0;
            
            currentStep.text = "current timestep=" + agents.get_current_timestep();
        }
        else
        {
            counter++;
        }
    }
}
