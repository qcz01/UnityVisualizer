using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class UIMenu 
{

    private static bool use_labels = false;
    private const string LabelItem = "Simulation Settings/Show Labels";
    private const string FileOpenInstance = "File/Open Instance";
    private const string FileOpenMap = "File/Open Map";
    [MenuItem(LabelItem, priority = 1)]
    private static void ToggleAction()
    {

        /// Toggling action
        PerformAction(!use_labels);
    }

    private static void PerformAction(bool enabled)
    {

        /// Set checkmark on menu item
        Menu.SetChecked(LabelItem, enabled);
        /// Saving editor state
        //EditorPrefs.SetBool(CheckmarkMenuItem.MENU_NAME, enabled);

        use_labels = enabled;
        if (use_labels) enableAllAgentsLabels();
        else disableAllAgentsLabels();

        /// Perform your logic here...
    }

    private static void enableAllAgentsLabels()
    {
        var agentsObject = GameObject.Find("Agents");
        var agents = agentsObject.GetComponent<Agents>();
        agents.enable_labels();
    }

    private static void disableAllAgentsLabels()
    {
        var agentsObject = GameObject.Find("Agents");
        var agents = agentsObject.GetComponent<Agents>();
        agents.disable_labels();
    }


}
