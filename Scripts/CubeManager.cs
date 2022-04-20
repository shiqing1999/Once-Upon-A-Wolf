using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubeManager : MonoBehaviour
{
    public enum MovementType
    {
        Stable,
        GhostShell,
        OnlyRotatable,
        UpAndDown
    }

    [SerializeField]
    [HideInInspector]
    MovementType m_MovementType = MovementType.Stable;

    public MovementType movementType
    {
        get => m_MovementType;
        set => m_MovementType = value;
    }

    public enum PathDirection
    {
        Straight,
        LeftTurn,
        RightTurn
    }

    [SerializeField]
    [HideInInspector]
    PathDirection m_PathDirection = PathDirection.Straight;

    public PathDirection pathDirection
    {
        get => m_PathDirection;
        set => m_PathDirection = value;
    }

    public enum HeightLevel
    {
        Same,
        Down
    }

    [SerializeField]
    [HideInInspector]
    [Tooltip("Height level compared to the previous one")]
    HeightLevel m_HeightLevel = HeightLevel.Same;

    public HeightLevel heightLevel
    {
        get => m_HeightLevel;
        set => m_HeightLevel = value;
    }

    public enum Placement
    {
        InPlace,
        NotInPlace
    }

    [SerializeField]
    [HideInInspector]
    Placement m_Placement = Placement.InPlace;

    public Placement placement
    {
        get => m_Placement;
        set => m_Placement = value;
    }

    public enum CubeFace
    {
        Up,
        Left,
        Right,
        Down
    }

    [SerializeField]
    [HideInInspector]
    CubeFace m_CorrectFace = CubeFace.Up;

    public CubeFace correctFace
    {
        get => m_CorrectFace;
        set => m_CorrectFace = value;
    }

    [SerializeField]
    bool m_LastCube = false;

    public bool lastCube
    {
        get => m_LastCube;
        set => m_LastCube = value;
    }

    [SerializeField]
    //[HideInInspector]
    List<GameObject> m_ShellList;

    public List<GameObject> shellList
    {
        get => m_ShellList;
        set => m_ShellList = value;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (lastCube && collision.collider.tag == "Player")
        {
            LevelController.levelController.gameEnd = true;
            LevelController.levelController.gameWin = true;
            //Debug.Log("win");
        }
    }

    CubeFace currentFace = CubeFace.Up;
    Quaternion currentRotation;
    bool justRotated;
    Vector3 currentPosition;
    bool justMoved;
    AudioSource audioSource;
    bool rotationLocked;

    private void Start()
    {
        currentRotation = gameObject.transform.rotation;
        currentPosition = gameObject.transform.position;
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void AdjustCube()
    {
        Quaternion rotationAmount = Quaternion.Euler(0, 0, 0);
        if (gameObject.transform.rotation.z > currentRotation.z)
        {
            rotationAmount = Quaternion.Euler(0, 0, -90);
        }
        else if (gameObject.transform.rotation.z < currentRotation.z)
        {
            rotationAmount = Quaternion.Euler(0, 0, 90);
        }

        if (rotationLocked)
        {
            rotationAmount = Quaternion.Euler(0, 0, 0);
            gameObject.transform.rotation = currentRotation;
        }

        bool up = false;
        bool down = false;
        if (gameObject.transform.position.y > currentPosition.y)
        {
            up = true;
            down = false;
        }
        else if (gameObject.transform.position.y < currentPosition.y)
        {
            up = false;
            down = true;
        }

        if (!justRotated && !justMoved)
        {
            StartCoroutine(AdjustCubeSlowly(rotationAmount, up, down));
        }
    }

    IEnumerator AdjustCubeSlowly(Quaternion rotationAmount, bool up, bool down)
    {
        Debug.Log("rotation" + rotationAmount);
        Debug.Log("up: " + up + ", down: " + down);
        if (rotationAmount.z != 0 && !rotationLocked)
        {
            currentRotation *= rotationAmount;
            gameObject.transform.rotation = currentRotation;
            if (rotationAmount.z > 0)
            {
                justRotated = true;
                audioSource.Play();
                UpdateFace(true);
            }
            else
            {
                justRotated = true;
                audioSource.Play();
                UpdateFace(false);
            }

            if (currentFace == correctFace)
                rotationLocked = true;
        }

        if (up)
        {
            justMoved = true;
            gameObject.transform.position = m_ShellList[0].transform.position;
            currentPosition = gameObject.transform.position;
            audioSource.Play();
        }
        else if (down)
        {
            justMoved = true;
            gameObject.transform.position = m_ShellList[1].transform.position;
            currentPosition = gameObject.transform.position;
            audioSource.Play();
        }
        CheckInPlace();

        yield return new WaitForSeconds(1);
        justRotated = false;
        justMoved = false;

    }

    public void RotateCube()
    {
        Quaternion rotationAmount = Quaternion.Euler(0, 0, 0);
        if (gameObject.transform.rotation.z > currentRotation.z)
        {
            rotationAmount = Quaternion.Euler(0, 0, -90);
        }
        else if (gameObject.transform.rotation.z < currentRotation.z)
        {
            rotationAmount = Quaternion.Euler(0, 0, 90);
        }

        if (rotationLocked)
        {
            rotationAmount = Quaternion.Euler(0, 0, 0);
            gameObject.transform.rotation = currentRotation;
        }

        if (!justRotated && !rotationLocked)
        {
            StartCoroutine(RotateCubeSlowly(rotationAmount));
        }
    }
    IEnumerator RotateCubeSlowly(Quaternion rotationAmount)
    {
        Debug.Log("only rotation" + rotationAmount);

        currentRotation = currentRotation * rotationAmount;
        gameObject.transform.rotation = currentRotation;
        if (rotationAmount.z > 0)
        {
            justRotated = true;
            UpdateFace(true);
            audioSource.Play();
        }
        else if (rotationAmount.z < 0)
        {
            justRotated = true;
            UpdateFace(false);
            audioSource.Play();
        }

        if (correctFace == currentFace)
        {
            rotationLocked = true;
            m_Placement = Placement.InPlace;
        }
        yield return new WaitForSeconds(1);
        justRotated = false;
    }

    void UpdateFace(bool clockwise)
    {
        if (clockwise)
        {
            switch (currentFace)
            {
                case CubeFace.Up:
                    currentFace = CubeFace.Right;
                    break;

                case CubeFace.Right:
                    currentFace = CubeFace.Down;
                    break;

                case CubeFace.Down:
                    currentFace = CubeFace.Left;
                    break;

                case CubeFace.Left:
                    currentFace = CubeFace.Up;
                    break;
            }
        }
        else
        {
            switch (currentFace)
            {
                case CubeFace.Up:
                    currentFace = CubeFace.Left;
                    break;

                case CubeFace.Left:
                    currentFace = CubeFace.Down;
                    break;

                case CubeFace.Down:
                    currentFace = CubeFace.Right;
                    break;

                case CubeFace.Right:
                    currentFace = CubeFace.Up;
                    break;
            }
        }
        Debug.Log("CurrentFace: " + currentFace);
    }

    void CheckInPlace()
    {
        if (gameObject.transform.position == m_ShellList[0].transform.position && currentFace == m_ShellList[0].GetComponent<CubeManager>().correctFace)
        {
            m_ShellList[0].GetComponent<CubeManager>().placement = Placement.InPlace;
            m_ShellList[1].GetComponent<CubeManager>().placement = Placement.NotInPlace;
        }
        else if (gameObject.transform.position == m_ShellList[1].transform.position && currentFace == m_ShellList[1].GetComponent<CubeManager>().correctFace)
        {
            m_ShellList[1].GetComponent<CubeManager>().placement = Placement.InPlace;
            m_ShellList[0].GetComponent<CubeManager>().placement = Placement.NotInPlace;
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(CubeManager))]
class CubeCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CubeManager cubeManager = (CubeManager)target;
        cubeManager.movementType = (CubeManager.MovementType)EditorGUILayout.EnumPopup("Movement Type", cubeManager.movementType);
        cubeManager.pathDirection = (CubeManager.PathDirection)EditorGUILayout.EnumPopup("Path Direction", cubeManager.pathDirection);
        cubeManager.heightLevel = (CubeManager.HeightLevel)EditorGUILayout.EnumPopup("Height Level", cubeManager.heightLevel);

        if (cubeManager.movementType != CubeManager.MovementType.Stable)
        {
            cubeManager.placement = (CubeManager.Placement)EditorGUILayout.EnumPopup("Placement", cubeManager.placement);
            cubeManager.correctFace = (CubeManager.CubeFace)EditorGUILayout.EnumPopup("Correct Face", cubeManager.correctFace);
        }
        //if (cubeManager.movementType == CubeManager.MovementType.UpAndDown)
        //{
        //    //TODO: show list
        //}
    }
}
#endif
