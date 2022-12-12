using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapItem: MonoBehaviour{
    // Start is called before the first frame update
    public Text itemName;
  
    public void SetText(string name){
        itemName.text=name;
    }

    public void isSelected(){
        Debug.Log(itemName.text+" is selected!");
        PlayerPrefs.SetString("selected_map",itemName.text);
    }
}
