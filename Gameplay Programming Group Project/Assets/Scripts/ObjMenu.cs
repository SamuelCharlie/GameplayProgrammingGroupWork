using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjMenu : MonoBehaviour
{
    public GameObject obj_menu;

    private void Update()
    {
        if (PlayerController1.in_obj_menu)
        {
            obj_menu.SetActive(true);
        }
        else if (PlayerController1.in_obj_menu)
        {
            obj_menu.SetActive(false);
        }
    }
}
