using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    static LevelController m_levelController;

    public static LevelController levelController
    {
        get => m_levelController;
    }

    private void Awake()
    {
        //Check if instance already exists
        if (m_levelController == null)

            //if not, set instance to this
            m_levelController = this;

        //If instance already exists and it's not this:
        else if (m_levelController != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField]
    List<GameObject> m_CubeList;

    public List<GameObject> cubeList
    {
        get => m_CubeList;
        set => m_CubeList = value;
    }

    [SerializeField]
    int m_PrepTimer;

    public int prepTimer
    {
        get => m_PrepTimer;
        set => m_PrepTimer = value;
    }

    [SerializeField]
    int m_GameTimer;

    public int gameTimer
    {
        get => m_GameTimer;
        set => m_GameTimer = value;
    }

    [SerializeField]
    Text m_Timer;

    public Text timer
    {
        get => m_Timer;
        set => m_Timer = value;
    }

    [HideInInspector]
    public bool gameStart = false;
    [HideInInspector]
    public bool gameEnd = false;
    [HideInInspector]
    public bool gameWin = false;

    private void Start()
    {
        gameEnd = false;
        gameStart = false;
        gameWin = false;
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        int counter = m_PrepTimer;
        while (counter > 0)
        {

            if (counter > 2)
            {
                m_Timer.text = (counter - 2).ToString();
            }
            else if (counter == 2)
            {
                m_Timer.text = "Ready";
            }
            else if (counter == 1)
            {
                m_Timer.text = "GO";
            }

            yield return new WaitForSeconds(1);
            counter--;
        }
        gameStart = true;

        counter = m_GameTimer;
        while (counter > 0)
        {
            if (gameEnd)
            {
                yield return null;
                break;
            }
            m_Timer.text = counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }

        gameEnd = true;
        gameObject.GetComponent<AudioSource>().Stop();

        if (!gameWin)
            SceneManager.LoadScene("Failure");
    }
}
