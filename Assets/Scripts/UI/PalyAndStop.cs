using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PalyAndStop : MonoBehaviour
{
    // Start is called before the first frame update

    private bool isPaused = false;

    public Sprite playImg;
    public Sprite pauseImage;
    void Start()
    {
        
    }

    void continueToPlay()
    {

    }


    void pauseTheGame()
    {

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
