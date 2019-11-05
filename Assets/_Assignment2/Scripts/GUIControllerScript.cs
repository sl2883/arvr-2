using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIControllerScript : MonoBehaviour
{
    [SerializeField]
    public SceneController_Part_1 sceneController;

    public void undo()
    {
        sceneController.undo();
    }

    public void reset()
    {
        sceneController.reset();
    }

}
