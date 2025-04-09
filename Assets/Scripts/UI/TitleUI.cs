using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public void Play()
    {
        SceneManagerScript.Instance.loadSceneByIndex(1);
    }

    public void Quit()
    {
        SceneManagerScript.Instance.Quit();
    }
}
