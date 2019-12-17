using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//自定义复选框控件
public class CheckBox : MonoBehaviour,IPointerDownHandler
{
    //勾选与未勾选时的图像
    public Sprite sp_checked;
    public Sprite sp_unchecked;
    //是否被勾选
    public bool isChecked;
    public bool IsChecked
    {
        get => isChecked;
        set
        {
            if (isChecked == value)
                return;
            isChecked = value;
            if (isChecked)
                GetComponent<Image>().sprite = sp_checked;
            else
                GetComponent<Image>().sprite = sp_unchecked;
        }
    }

    void Start()
    {
        if (isChecked)
            GetComponent<Image>().sprite = sp_checked;
        else
            GetComponent<Image>().sprite = sp_unchecked;
    }
    //鼠标点击事件
    public void OnPointerDown(PointerEventData eventData)
    {
        IsChecked = !IsChecked;
    }
}
