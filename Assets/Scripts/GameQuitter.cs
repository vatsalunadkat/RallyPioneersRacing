using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameQuitter : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("Select");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
