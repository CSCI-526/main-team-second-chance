using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI GameOverText;
    [SerializeField]
    private Button PlayAgainButton;
    [SerializeField]
    private Button QuitButton;
    [SerializeField]
    private Color playerColor;
    [SerializeField]
    private Color enemyColor;

    public string titleScene;
    public string gameScene;

    private void OnEnable()
    {
        TurnStateEvents.OnGameOver += UpdateGameOverPanel;
    }
    private void OnDisable()
    {
        TurnStateEvents.OnGameOver -= UpdateGameOverPanel;
    }

    void Start()
    {
        HidePanel();
    }
    public void OnClickPlayAgain()
    {
        bool bShouldRestart = NodeManager.Instance.ShouldRestartOrMenu();
        if (bShouldRestart)
        {
            SceneManagerScript.Instance.loadSceneByIndex(0);
        }
        else
        {
            SceneManagerScript.Instance.loadSceneByIndex(2);
        }
    }

    public void OnClickLevelSelect()
    {
        SceneManagerScript.Instance.loadSceneByIndex(2);
    }

    private void UpdateGameOverPanel()
    {
        if (GameManager.Instance.GetPlayerScore() == GameManager.Instance.GetEnemyScore())
        {
            GameOverText.text = "Draw!".ToUpper();
            GameOverText.color = Color.white;
        }
        else if (GameManager.Instance.GetPlayerScore() > GameManager.Instance.GetEnemyScore())
        {
            GameOverText.text = "You win!".ToUpper();
            GameOverText.color = playerColor;
        }
        else if (GameManager.Instance.GetPlayerScore() < GameManager.Instance.GetEnemyScore())
        {
            GameOverText.text = "You lose...".ToUpper();
            GameOverText.color = enemyColor;
        }
        ShowPanel();
    }

    private void HidePanel()
    {
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);
        GameOverText.gameObject.SetActive(false);
        PlayAgainButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
    }

    private void ShowPanel()
    {
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(1);
        GameOverText.gameObject.SetActive(true);

        if (GameManager.Instance.GetPlayerScore() == GameManager.Instance.GetEnemyScore() || GameManager.Instance.GetPlayerScore() > GameManager.Instance.GetEnemyScore())
        {
            PlayAgainButton.gameObject.SetActive(true);
        }
        else if (GameManager.Instance.GetPlayerScore() < GameManager.Instance.GetEnemyScore())
        {
            PlayAgainButton.gameObject.SetActive(false);
        }
        
        QuitButton.gameObject.SetActive(true);
    }
}
