using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Failure : MonoBehaviour
{
    public GameObject restartButton;

    VideoPlayer videoPlayer;

    private bool animationOver = false;

    private float skipTime = 1f;
    private bool canSkip = false;


    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        restartButton.SetActive(false);

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
            restartButton.SetActive(true);
            //SceneManager.LoadScene("LevelScene");
        }

    }
}
