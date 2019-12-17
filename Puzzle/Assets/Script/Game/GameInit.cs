using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

//游戏开始前进行初始化的类
public class GameInit : MonoBehaviour
{
    //标志变量，初始化只进行一次
    private static bool isFirstStartGame = true;
    void Awake()
    {
        if(isFirstStartGame)
        {
            //获取用户设置
            if (CustomSetting.GetCustomSettingFromFile() == null)
                CustomSetting.CreateDefaultCustomSetting();
            //获取主画布的缩放值，并保存
            GameObject startMenu = GameObject.Find("StartMenu");
            float scaleX = startMenu.transform.localScale.x;
            float scaleY = startMenu.transform.localScale.y;
            PlayerPrefs.SetFloat("scaleX", scaleX);
            PlayerPrefs.SetFloat("scaleY", scaleY);
            isFirstStartGame = false;
        }
    }
}
