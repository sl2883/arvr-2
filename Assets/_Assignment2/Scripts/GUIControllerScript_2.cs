using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIControllerScript_2 : GUIControllerScript
{
    [SerializeField]
    public SceneController_Part_2 sceneController2;

    public void place()
    {
        sceneController2.place();
    }

    public void updateScroll(float currentVal) 
    {
        sceneController2.updateScroll(currentVal);
    }
}
