using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool paused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) Unpause();
            else Pause();
            paused = !paused;
        }
    }

    public void Pause()
    {
        GameManager.SetGamePaused(true);
        pausePanel.SetActive(true);
        AudioManager.TriggerSound(AudioManager.Instance.ClickSound,Vector3.zero);
    }

    public void Unpause()
    {
        GameManager.SetGamePaused(false);
        pausePanel.SetActive(false);
        AudioManager.TriggerSound(AudioManager.Instance.ClickSound,Vector3.zero);
    }
}
