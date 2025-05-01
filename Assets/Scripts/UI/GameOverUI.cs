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
    private TextMeshProUGUI PlayAgainText;
    //[SerializeField]
    //private Button LevelSelectButton;
    [SerializeField]
    private Color playerColor;
    [SerializeField]
    private Color enemyColor;

    public string titleScene;
    public string gameScene;
    private bool bDidPlayerLose = false;
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
        if (bShouldRestart || bDidPlayerLose)
        {
            SceneManagerScript.Instance.loadSceneByIndex(0);
        }
        else
        {
            SceneManagerScript.Instance.loadSceneByIndex(2);
        }
    }

    //public void OnClickLevelSelect()
    //{
    //    SceneManagerScript.Instance.loadSceneByIndex(2);
    //}

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
            GameOverText.text = "Game over. You lose...".ToUpper();
            GameOverText.color = enemyColor;
        }
        ShowPanel();
    }

    private void HidePanel()
    {
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);
        GameOverText.gameObject.SetActive(false);
        PlayAgainButton.gameObject.SetActive(false);
        //LevelSelectButton.gameObject.SetActive(false);
    }

    private void ShowPanel()
    {
        gameObject.GetComponent<CanvasRenderer>().SetAlpha(1);
        GameOverText.gameObject.SetActive(true);
        PlayAgainButton.gameObject.SetActive(true);
        //LevelSelectButton.gameObject.SetActive(true);

        if (!PlayAgainText)
        {
            Debug.LogError("PlayAgainText SerializedField is null. Please Set it to the text under PlayAgainButton");
        }
        if (GameManager.Instance.GetPlayerScore() == GameManager.Instance.GetEnemyScore() || GameManager.Instance.GetPlayerScore() > GameManager.Instance.GetEnemyScore())
        {
            PlayAgainText.text = "CONTINUE";
        }
        else if (GameManager.Instance.GetPlayerScore() < GameManager.Instance.GetEnemyScore())
        {
            PlayAgainText.text = "TRY AGAIN";
            bDidPlayerLose = true;
        }
    }
}
