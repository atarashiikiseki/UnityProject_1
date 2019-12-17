using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//游戏内的菜单
public class GameMenu : MonoBehaviour
{
    //游戏设置的按钮
    public Button btn_Setting;
    //重新开始的按钮
    public Button btn_Restart;
    //关卡选择的按钮
    public Button btn_SelectLevel;
    //返回开始菜单的按钮
    public Button btn_ReturnStartMenu;
    //返回游戏的按钮
    public Button btn_ReturnGame;
    //游戏设置的面板
    public SettingPanel panel_Setting;
    //关卡选择的面板
    public SelectLevelPanel panel_selectLevel;
    void Start()
    {
        //添加按钮事件
        btn_Setting.onClick.AddListener(() => 
        {
            panel_Setting.gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
        btn_Restart.onClick.AddListener(() =>
        {
            GameMain.instance.Clear();
            PuzzleImages.instance.count--;
            GameMain.instance.NewPuzzle();
            gameObject.SetActive(false);
        });
        btn_SelectLevel.onClick.AddListener(() => 
        {
            panel_selectLevel.gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
        //使用自定义图片时，不会保存进度，更新相应的显示
        bool useCustomImg = PlayerPrefs.GetInt("useCustomImg") == 1 ? true : false;
        if (useCustomImg)
            btn_ReturnStartMenu.transform.GetChild(0).GetComponent<Text>().text =
                "返回主菜单";
        else
            btn_ReturnStartMenu.transform.GetChild(0).GetComponent<Text>().text =
                "保存并返回主菜单";
        btn_ReturnStartMenu.onClick.AddListener(() => 
        {
            //如果使用的是默认图片，返回主菜单前保存游戏
            if (!useCustomImg) 
                Archive.Save();
            //保存用户设置
            panel_Setting.SaveCustomSetting();
            SceneManager.LoadScene("StartMenu");
        });
        btn_ReturnGame.onClick.AddListener(() => 
        {
            if(!GameMain.instance.IsFinished)
                GameMain.instance.IsListenForInput = true;
            MainPanel.instance.EnableButton();
            gameObject.SetActive(false);
        });
    }
}
