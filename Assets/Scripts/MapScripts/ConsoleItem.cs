using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConsoleItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Text itemName;

    public void SetText(string name)
    {
        itemName.text = name;
    }

    public void isSelected()
    {
        Debug.Log(itemName.text + " is selected!");

    }
}
