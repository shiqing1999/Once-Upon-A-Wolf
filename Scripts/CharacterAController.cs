using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterAController : MonoBehaviour
{
    [SerializeField]
    float m_speed;

    public float speed
    {
        get => m_speed;
        set => m_speed = value;
    }

    [SerializeField]
    AudioSource m_rollingSound;

    public AudioSource rollingSound
    {
        get => m_rollingSound;
        set => m_rollingSound = value;
    }

    [SerializeField]
    AudioSource m_pantingSound;

    public AudioSource pantingSound
    {
        get => m_pantingSound;
        set => m_pantingSound = value;
    }

    int currentCubeNumber;

    Animator m_Animator;

    bool redirected = false;

    IEnumerator MoveCharacterA()
    {
        yield return new WaitForSeconds(5);

        //TODO: show tutorial

        while (currentCubeNumber < LevelController.levelController.cubeList.Count)
        {
            //as long as it is not the last cube
            if (currentCubeNumber < LevelController.levelController.cubeList.Count - 1)
            {
                redirected = false;
                GameObject nextGO = LevelController.levelController.cubeList[currentCubeNumber + 1];
                if (nextGO.GetComponent<CubeManager>().placement == CubeManager.Placement.InPlace)
                {
                    //move to the next cube
                    Vector3 nextPos = new Vector3(nextGO.transform.position.x, gameObject.transform.position.y, nextGO.transform.position.z);
                    float yOff = nextGO.transform.position.y - LevelController.levelController.cubeList[currentCubeNumber].transform.position.y;
                    if (nextGO.GetComponent<CubeManager>().heightLevel == CubeManager.HeightLevel.Down)
                    {
                        nextPos += new Vector3(0, yOff, 0);
                    }

                    while (gameObject.transform.position.x != nextPos.x || gameObject.transform.position.z != nextPos.z)
                    {
                        if (!LevelController.levelController.gameEnd)
                        {
                            if (!m_rollingSound.isPlaying)
                                m_rollingSound.Play();

                            //check directions
                            if (nextGO.GetComponent<CubeManager>().pathDirection == CubeManager.PathDirection.Straight)
                            {
                                //m_Animator.SetInteger("directions", 0);
                                gameObject.transform.rotation *= Quaternion.Euler(0, 0, -2);
                            }

                            else if (nextGO.GetComponent<CubeManager>().pathDirection == CubeManager.PathDirection.LeftTurn)
                            {
                                //m_Animator.SetInteger("directions", 1);
                                //gameObject.transform.rotation *= Quaternion.Euler(-1, 0, 0);
                                gameObject.transform.rotation *= Quaternion.Euler(0, 0, -2);
                                if (!redirected)
                                {
                                    gameObject.transform.Rotate(0f, -90f, 0.0f, Space.World);
                                    redirected = true;
                                }
                            }

                            else if (nextGO.GetComponent<CubeManager>().pathDirection == CubeManager.PathDirection.RightTurn)
                            {
                                //m_Animator.SetInteger("directions", 2);
                                //gameObject.transform.rotation *= Quaternion.Euler(1, 0, 0);
                                gameObject.transform.rotation *= Quaternion.Euler(0, 0, -2);
                                if (!redirected)
                                {
                                    gameObject.transform.Rotate(0f, 90f, 0.0f, Space.World);
                                    redirected = true;
                                }
                            }

                            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextPos, speed * Time.deltaTime);
                            yield return new WaitForEndOfFrame();
                        }
                        else
                        {
                            m_rollingSound.Stop();
                            break;
                        }
                    }
                    currentCubeNumber++;
                }
                else
                {
                    //wait here
                    m_rollingSound.Stop();
                    //m_Animator.SetInteger("directions", -1);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                //TODO: game win character animation
                LevelController.levelController.gameEnd = true;
                LevelController.levelController.gameWin = true;
                gameObject.GetComponent<AudioSource>().Stop();
                SceneManager.LoadScene("Ending");
                yield return null;
            }
        }
    }

    private void Start()
    {
        gameObject.transform.position = LevelController.levelController.cubeList[0].transform.position + new Vector3(0, 0.7f, 0);
        currentCubeNumber = 0;
        //m_Animator = gameObject.GetComponent<Animator>();
        //m_Animator.SetInteger("directions", -1);
        StartCoroutine(MoveCharacterA());
    }

    void PlayRollingSound()
    {
        if (!m_rollingSound.isPlaying)
            m_rollingSound.Play();
    }

    void PlayPantingSound()
    {
        if (!m_pantingSound.isPlaying)
            m_pantingSound.Play();
    }

}
