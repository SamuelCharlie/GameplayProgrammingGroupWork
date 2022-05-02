using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public int current_scene;
    public Transform player_transform;

    private void OnTriggerEnter(Collider other)
    {
        if (current_scene == 1)
        {
            SceneManager.LoadSceneAsync("SceneTwo");
            current_scene = 2;
        }
        else if (current_scene == 2)
        {
            SceneManager.LoadSceneAsync("SceneOne");
            current_scene = 1;
        }
    }
}
