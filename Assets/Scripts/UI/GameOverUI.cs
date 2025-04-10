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
        SceneManagerScript.Instance.loadSceneByIndex(2);
    }

    public void OnClickTitle()
    {
        SceneManagerScript.Instance.loadSceneByIndex(1);
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
