using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGoal : MonoBehaviour
{
    [SerializeField]
    Toggle m_Toggle;

    [SerializeField]
    Agents agents;
    //public Text m_Text;

    void Start()
    {
        //Fetch the Toggle GameObject
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
        agents.show_goals(false);

    }


    void enableLabels()
    {
        agents.show_goals(true);
    }

}
