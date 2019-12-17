using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//选择关卡的面板
public class SelectLevelPanel : MonoBehaviour
{
    //可选择的关卡的下拉框
    public Dropdown dp_SelectableLevels;
    //确认按钮
    public Button btn_OK;
    //取消按钮
    public Button btn_Cancel;

    //可选择的最大关卡
    public int maxLevel = -1;

    void Start()
    {
        //添加按钮的点击事件
        btn_OK.onClick.AddListener(() =>
        {
            //跳转到指定关卡
            int value = dp_SelectableLevels.value;
            PuzzleImages.instance.count = value - 1;
            GameMain.instance.Clear();
            GameMain.instance.NewPuzzle();
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
    }
    private void OnEnable()
    {
        //更新可选择的最大关卡
        int rmMaxCount = PuzzleImages.instance.maxCount;
        if (rmMaxCount > maxLevel)
        {
            List<string> names = PuzzleImages.instance.allImageNames;
            for(int i = maxLevel + 1; i <= rmMaxCount; i++)
            {
                string text = "Level " + (i + 1) + " : " + names[i];
                dp_SelectableLevels.options.Add(new Dropdown.OptionData(text));
            }
            maxLevel = rmMaxCount;
        }
    }
}
