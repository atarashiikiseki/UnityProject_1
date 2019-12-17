using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//游戏的主界面，单例
public class MainPanel : MonoBehaviour
{
    //静态实例
    public static MainPanel instance;

    //显示当前关卡的文本控件
    public Text txt_Level;
    //拼图完成时的消息框
    public GameObject mB_Finished;
    //所有拼图都完成时的消息框
    public GameObject mB_AllFinished;
    //打开菜单的按钮
    public Button btn_Menu;
    //下一关的按钮
    public Button btn_NextLevel;
    //返回开始菜单的按钮
    public Button btn_ReturnStartMenu;
    //菜单
    public GameMenu menu;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        //初始化
        Init();
    }
    //初始化
    void Init()
    {
        //为按钮添加点击事件
        btn_NextLevel.onClick.AddListener(() =>
        {
            GameMain.instance.Clear();
            GameMain.instance.NewPuzzle();
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
            if (!useCustomImg)
                Archive.Save();
            //保存用户设置
            menu.panel_Setting.SaveCustomSetting();
            SceneManager.LoadScene("StartMenu");
        });
        btn_Menu.onClick.AddListener(() =>
        {
            GameMain.instance.IsListenForInput = false;
            DisabelButton();
            menu.gameObject.SetActive(true);
        });
        mB_Finished.transform.GetComponentInChildren<Button>().onClick.AddListener(
        () =>
        {
            EnableButton();
            mB_Finished.gameObject.SetActive(false);
        });
        mB_AllFinished.transform.GetComponentInChildren<Button>().onClick.AddListener(
        () =>
        {
            btn_NextLevel.gameObject.SetActive(false);
            btn_ReturnStartMenu.gameObject.SetActive(true);
            EnableButton();
            mB_AllFinished.gameObject.SetActive(false);
        });

        //读取用户设置
        if (!CustomSetting.IsEmpty)
            menu.panel_Setting.LoadCustomSetting();
    }
    //开始新游戏时，进行相关设置
    public void OnNewGame()
    {
        //开启菜单按钮
        btn_Menu.interactable = true;
        //关闭下一关的按钮
        btn_NextLevel.gameObject.SetActive(true);
        btn_NextLevel.interactable = false;
        //隐藏返回主菜单的按钮
        btn_ReturnStartMenu.gameObject.SetActive(false);
        //更新关卡显示
        txt_Level.text = "Level " + (PuzzleImages.instance.count + 1) + " : " +
            (PuzzleImages.instance.CurrentImgName);

    }
    //一轮游戏结束时，进行相关设置
    public void OnGameFinished()
    {
        DisabelButton();
        //还有资源图片
        if (PuzzleImages.instance.HaveImg)
            mB_Finished.SetActive(true);
        //游戏结束
        else
            mB_AllFinished.SetActive(true);
    }
    //关闭所有按钮
    public void DisabelButton()
    {
        btn_Menu.interactable = false;
        btn_NextLevel.interactable = false;
        btn_ReturnStartMenu.interactable = false;
    }
    //开启所有按钮
    public void EnableButton()
    {
        btn_Menu.interactable = true;
        if (GameMain.instance.IsFinished)
            btn_NextLevel.interactable = true;
        btn_ReturnStartMenu.interactable = true;
    }
}
