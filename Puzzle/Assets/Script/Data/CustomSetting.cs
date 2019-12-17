using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//保存用户设置的类，单例
[Serializable]
public class CustomSetting
{
    //静态实例
    public static CustomSetting instance;

    //用户设置文件的相对路径
    public static string path = "CustomSetting.sav";
    //bool 类变量
    public Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
    //..

    //存档数据是否为空
    public static bool IsEmpty
    {
        get
        {
            return (instance == null || instance.boolValues.Count == 0);
        }
    }

    private CustomSetting() { }

    //从本地文件读取用户设置
    public static CustomSetting GetCustomSettingFromFile()
    {
        string path = Path.Combine(Application.dataPath, CustomSetting.path);
        try
        {
            string jsonData = File.ReadAllText(path);
            instance = JsonConvert.DeserializeObject<CustomSetting>(jsonData);
            return instance;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }
    //创建空的用户设置
    public static void CreateDefaultCustomSetting()
    {
        instance = new CustomSetting();
    }
    //将用户设置保存到本地
    public static void Save()
    {
        if (instance == null)
            return;
        string jsonText = JsonConvert.SerializeObject(instance);
        string path = Path.Combine(Application.dataPath, CustomSetting.path);
        FileStream fs = File.Open(path, FileMode.Create);
        fs.Close();
        File.WriteAllText(path, jsonText);
    }
    //是否有相应的键
    public static bool HasKey(string key)
    {
        foreach (var k in instance.boolValues.Keys)
            if (k.Equals(key))
                return true;

        return false;
    }
    //设置一个 bool 值
    public static void SetBool(string key, bool value)
    {
        if (HasKey(key))
            instance.boolValues[key] = value;
        else
            instance.boolValues.Add(key, value);
    }
    //根据属性名获取一个 bool 值
    public static bool GetBool(string key)
    {
        if (instance.boolValues.TryGetValue(key, out bool value))
            return value;

        return false;
    }
    //根据属性名删除一个属性
    public static bool DeleteProperty(string key)
    {
        return (instance.boolValues.Remove(key));
    }
}