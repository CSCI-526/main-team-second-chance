using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public void Play()
    {
        // we move to the level select now instead
        SceneManagerScript.Instance.loadSceneByIndex(2);
    }

    public void Quit()
    {
        SceneManagerScript.Instance.Quit();
    }
}
