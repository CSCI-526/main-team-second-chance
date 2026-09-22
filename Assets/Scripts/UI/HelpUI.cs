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
        GameManager.SetGamePaused(true);
        helpPanel.SetActive(true);
        AudioManager.TriggerSound(AudioManager.Instance.ClickSound,Vector3.zero);
    }

    public void CloseHelpPanel()
    {
        GameManager.SetGamePaused(false);
        helpPanel.SetActive(false);
        AudioManager.TriggerSound(AudioManager.Instance.ClickSound,Vector3.zero);
    }
}
