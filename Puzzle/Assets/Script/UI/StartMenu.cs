using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    //开始游戏的按钮
    public Button btn_NewGame;
    //继续游戏的按钮
    public Button btn_ContinueGame;
    //结束游戏的按钮
    public Button btn_ExitGame;

    //游戏设置的面板
    public SettingBeforeStartPanel gameSetting;

    void Start()
    {
        //为按钮添加点击事件
        btn_NewGame.onClick.AddListener(() =>
        {
            gameSetting.gameObject.SetActive(true);
            PlayerPrefs.SetInt("isNewGame", 1);
            gameObject.SetActive(false);
        });
        //有存档数据时，可以继续游戏
        if(Archive.GetArchive() != null)
        {
            btn_ContinueGame.interactable = true;
            btn_ContinueGame.onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt("isNewGame", 0);
                PlayerPrefs.SetInt("useCustomImg",0);
                SceneManager.LoadScene("Puzzle");
            });
        }
        btn_ExitGame.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
