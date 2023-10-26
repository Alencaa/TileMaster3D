using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtLevel = null;
    [SerializeField] private Button btnPlay = null;
    private void Start()
    {
        Config.GetCurrLevel();
        int level = Config.currLevel;
        txtLevel.text = "Level " + level;
        btnPlay.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
            SoundHelper.PlayEffect("play");
        });
        SoundHelper.PlayMusic("music");
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Config.ClearPlayerPref();
            int level = Config.currLevel;
            txtLevel.text = "Level " + level;
        }
    }
}
