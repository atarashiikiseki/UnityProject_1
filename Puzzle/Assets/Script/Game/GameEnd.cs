using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏结束时进行的操作
public class GameEnd : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        //保存用户设置
        CustomSetting.Save();
    }
}
