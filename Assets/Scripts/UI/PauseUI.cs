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
        Time.timeScale = 0.0f;
        pausePanel.SetActive(true);
    }

    public void Unpause()
    {
        Time.timeScale = 2.0f;
        pausePanel.SetActive(false);
    }
}
