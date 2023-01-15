using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseLabel : MonoBehaviour
{
    Toggle m_Toggle;
    public Text m_Text;

    void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponent<Toggle>();
        //Add listener for when the state of the Toggle changes, to take action
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(m_Toggle);
        });

        //Initialise the Text to say the first state of the Toggle
        //m_Text.text = "First Value : " + m_Toggle.isOn;
    }

    //Output the new state of the Toggle into Text
    void ToggleValueChanged(Toggle change)
    {
        //m_Text.text = "New Value : " + m_Toggle.isOn;
        if (m_Toggle.isOn) enableLabels();
        else disableLabels();
    }


    void disableLabels()
    {
        var agentsObject = GameObject.Find("Agents");
        var agents = agentsObject.GetComponent<Agents>();
        agents.disable_labels();
        
    }


    void enableLabels()
    {
        var agentsObject = GameObject.Find("Agents");
        var agents = agentsObject.GetComponent<Agents>();
        agents.enable_labels();
    }
}
