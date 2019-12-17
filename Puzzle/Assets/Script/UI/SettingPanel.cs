using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//游戏设置的面板
public class SettingPanel : MonoBehaviour
{
    //行数的输入域
    public InputField iF_Row;
    //列数的输入域
    public InputField iF_Col;
    //行数或列数的最小值
    public const int MIN_ROWORCOL = 2;
    //行数或列数的最大值
    public const int MAX_ROWORCOL = 20;
    //音乐控制
    public CheckBox cb_MusicSwitch;
    //音乐开关的默认值
    private bool dv_musicSwitch = true;
    //确认按钮
    public Button btn_OK;
    //取消按钮
    public Button btn_Cancel;

    void Start()
    {
        //添加按钮的点击事件
        btn_OK.onClick.AddListener(() =>
        {
            //设置拼图的行数和列数
            int row = Convert.ToInt32(iF_Row.text);
            PlayerPrefs.SetInt("puzzleRow", row);
            int col = Convert.ToInt32(iF_Col.text);
            PlayerPrefs.SetInt("puzzleCol", col);
            if (!GameMain.instance.IsFinished)
                GameMain.instance.IsListenForInput = true;
            //音乐开关
            bool isChecked = cb_MusicSwitch.isChecked;
            if (isChecked)
            {
                if (!AudioPlayer.instance.IsPlaying)
                    AudioPlayer.instance.Play();
            }
            else
                AudioPlayer.instance.Pause();
            //恢复被禁用的按钮
            MainPanel.instance.EnableButton();
            gameObject.SetActive(false);
        });
        btn_Cancel.onClick.AddListener(() => 
        {
            if (!GameMain.instance.IsFinished)
                GameMain.instance.IsListenForInput = true;
            //恢复被禁用的按钮
            MainPanel.instance.EnableButton();
            gameObject.SetActive(false);
        });
        //添加输入框的编辑完成事件
        iF_Row.onEndEdit.AddListener((content) =>
        {
            int row = Convert.ToInt32(iF_Row.text);
            if (row < MIN_ROWORCOL)
                iF_Row.text = MIN_ROWORCOL + "";
            else if (row > MAX_ROWORCOL)
                iF_Row.text = MAX_ROWORCOL + "";
        });
        iF_Col.onEndEdit.AddListener((content) =>
        {
            int col = Convert.ToInt32(iF_Col.text);
            if (col < MIN_ROWORCOL)
                iF_Col.text = MIN_ROWORCOL + "";
            else if (col > MAX_ROWORCOL)
                iF_Col.text = MAX_ROWORCOL + "";
        });      
    }
    //保存用户设置
    public void SaveCustomSetting()
    {
        if (cb_MusicSwitch.IsChecked != dv_musicSwitch)
            CustomSetting.SetBool("IsMusicOn", cb_MusicSwitch.IsChecked);
        else
            CustomSetting.DeleteProperty("IsMusicOn");
    }
    //导入用户设置
    public void LoadCustomSetting()
    {
        //..
        if (CustomSetting.HasKey("IsMusicOn"))
        {
            cb_MusicSwitch.IsChecked = CustomSetting.GetBool("IsMusicOn");
            AudioPlayer.instance.Pause();
        }
    }
    void Update()
    {
        if (iF_Row.isFocused || iF_Col.isFocused)
            btn_OK.interactable = false;
        else
            btn_OK.interactable = true;
    }
}
