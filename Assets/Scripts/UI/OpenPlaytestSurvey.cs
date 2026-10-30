using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPlaytestSurvey : MonoBehaviour
{
    const string formLink = "https://forms.gle/dEmFnCZaYCu1eies8";
    
    public void OpenSurvey()
    {
        Application.OpenURL(formLink);
    }
}
