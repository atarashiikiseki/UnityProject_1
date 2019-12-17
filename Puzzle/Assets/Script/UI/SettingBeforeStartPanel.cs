using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//开始游戏之前的设置
public class SettingBeforeStartPanel: MonoBehaviour
{
    //行数的输入域
    public InputField iF_Row;
    //列数的输入域
    public InputField iF_Col;
    //行数或列数的最小值
    public const int MIN_ROWORCOL = 2;
    //行数或列数的最大值
    public const int MAX_ROWORCOL = 20;
    //选择图片来源的下拉框
    public Dropdown dp_ImgSource;
    //选择下拉框前一个选中的值
    private int preValue = 0;
    //包含一些显示信息的文本
    public GameObject info;
    //显示信息的文本
    private Text[] infoTexts;
    //确认按钮
    public Button btn_OK;

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
            //获取图片资源
            bool useCustomImg = dp_ImgSource.value == 1 ? true : false;
            PlayerPrefs.SetInt("useCustomImg",dp_ImgSource.value);
            PuzzleImages.instance.GetAllImage(useCustomImg);
            PuzzleImages.instance.count = -1;
            PuzzleImages.instance.maxCount = -1;
            //切换到拼图游戏场景
            SceneManager.LoadScene("Puzzle");
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
        //为下拉选择框添加事件
        infoTexts = info.transform.GetComponentsInChildren<Text>();
        dp_ImgSource.onValueChanged.AddListener((value) =>
        {
            if(value == 1)
            {
                if (!PuzzleImages.instance.HaveCustomImg)
                {
                    infoTexts[value].text = "使用MyImage文件下的图片（jpg,png）进行游戏 " +
                         "(错误-未找到图片资源)";
                }
                else
                    infoTexts[value].text = "使用MyImage文件下的图片（jpg,png）进行游戏 ";
            }
            infoTexts[preValue].GetComponent<CanvasGroup>().alpha = 0;
            infoTexts[value].GetComponent<CanvasGroup>().alpha = 1;
            preValue = value;
        });
    }
    void Update()
    {
        if (iF_Row.isFocused || iF_Col.isFocused)
            btn_OK.interactable = false;
        else if (dp_ImgSource.value == 1 && !PuzzleImages.instance.HaveCustomImg)
            btn_OK.interactable = false;
        else
            btn_OK.interactable = true;
    }
}
