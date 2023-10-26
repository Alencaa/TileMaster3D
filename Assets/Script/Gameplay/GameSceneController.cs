using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSceneController : MonoBehaviour
{
    public static GameSceneController instance;
    private GameObject levelGame;
    public int level = 0;
    [SerializeField] private GameObject winGameUI = null;
    [SerializeField] private GameObject loseGameUI = null;
    [SerializeField] private TextMeshProUGUI txtScore = null;
    [SerializeField] private Button btnContinue = null;
    [SerializeField] private Button btnRetry = null;
    //[SerializeField] private TextMeshProUGUI  = null;
    private void Start()
    {
        instance = this;
        Config.GetCurrLevel();
        level = Config.currLevel;
        if (level == 0) level = 1;
        if (level > 4)
        {
            Config.SetCurrLevel(1);
            Debug.Log(Config.currLevel);
            level = 1;
        }
        LoadLevelGame();
        SetUpButton();
    }
    public void LoadLevelGame()
    {
        if (levelGame != null)
        {
            Destroy(levelGame);
            levelGame = null;
        }
        if (levelGame == null)
        {
            levelGame = Instantiate(Resources.Load("Level/Level" + Config.currLevel)) as GameObject;
        }
        winGameUI.SetActive(false);
        loseGameUI.SetActive(false);
    }
    private void SetUpButton()
    {
        btnContinue.onClick.AddListener(LoadLevelGame);
        btnRetry.onClick.AddListener(LoadLevelGame);
    }
    public void ShowLoseGame()
    {
        loseGameUI.SetActive(true);
    }
    public void ShowWinGame(int score)
    {
        winGameUI.SetActive(true);
        txtScore.text = "+ " + score;
    }
}
