using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayAndStop : MonoBehaviour
{
    // Start is called before the first frame update

    private bool isPaused = false;

    [SerializeField] Sprite playImg;
    [SerializeField] Sprite pauseImage;
    [SerializeField] Agents agents;
    void Start()
    {
        
    }

    void continueToPlay()
    {
        agents.puase_simulation(true);
    }


    void pauseTheGame()
    {
        agents.puase_simulation(false);
    }

    public void clickButton()
    {
        if (isPaused==true)
        {
            continueToPlay();
            isPaused = false;
            this.GetComponent<Image>().sprite = playImg;
            
        }
        else
        {
            pauseTheGame();
            isPaused = true;
            this.GetComponent<Image>().sprite = pauseImage;
        }
    }
}
