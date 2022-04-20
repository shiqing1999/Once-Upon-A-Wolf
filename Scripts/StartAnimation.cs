using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StartAnimation : MonoBehaviour
{
    //public GameObject startButton;

    VideoPlayer videoPlayer;

    private bool animationOver = false;

    private float skipTime = 1f;
    private bool canSkip = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        //startButton.SetActive(false);

        Invoke(methodName: "allowSkip", skipTime);
    }

    private void allowSkip()
    {
        canSkip = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!videoPlayer.isPlaying && !animationOver && canSkip)
        {
            animationOver = true;
            //startButton.SetActive(true);
            SceneManager.LoadScene("LevelScene");
        }
            
    }
}
