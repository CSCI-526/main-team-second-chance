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
        SceneManager.LoadScene(gameScene);
    }

    public void OnClickTitle()
    {
        SceneManager.LoadScene(titleScene);
    }

    private void UpdateGameOverPanel()
    {
        if (GameManager.Instance.GetPlayerScore() == GameManager.Instance.GetEnemyScore())
        {
            GameOverText.text = "Draw!";
            GameOverText.color = Color.white;
        }
        else if (GameManager.Instance.GetPlayerScore() > GameManager.Instance.GetEnemyScore())
        {
            GameOverText.text = "You win!";
            GameOverText.color = playerColor;
        }
        else if (GameManager.Instance.GetPlayerScore() < GameManager.Instance.GetEnemyScore())
        {
            GameOverText.text = "You lose...";
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
        PlayAgainButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
    }
}
