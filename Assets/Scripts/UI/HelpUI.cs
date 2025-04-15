using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpUI : MonoBehaviour
{
    [SerializeField] private GameObject helpPanel;

    private bool helpPanelOpen = false;

    private void Update()
    {
    }

    public void OpenHelpPanel()
    {
        Time.timeScale = 0.0f;
        helpPanel.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        Time.timeScale = 2.0f;
        helpPanel.SetActive(false);
    }
}
