using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI GameOverText;
    [SerializeField]
    private Button PlayAgainButton;
    [SerializeField]
    private Color playerColor;
    [SerializeField]
    private Color enemyColor;

    private void OnEnable()
    {
        TurnStateEvents.OnGameOver += UpdateGameOverPanel;
    }
    private void OnDisable()
    {
        TurnStateEvents.OnGameOver -= UpdateGameOverPanel;
    }

    void Start () {
        HidePanel();
        PlayAgainButton.onClick.AddListener(OnClickPlayAgain);
    }

    public void OnClickPlayAgain() {
        GameManager.Instance.RestartGame();
        HidePanel();
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

    private void HidePanel() {
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);
        GameOverText.gameObject.SetActive(false);
        PlayAgainButton.gameObject.SetActive(false);
    }

    private void ShowPanel() {
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(1);
        GameOverText.gameObject.SetActive(true);
        PlayAgainButton.gameObject.SetActive(true);
    }
}
