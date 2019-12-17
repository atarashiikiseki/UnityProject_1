using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//拼图碎片
public class ImagePart : MonoBehaviour
{
    //是否被选中
    public bool IsSelected = false;
    //正确的坐标
    public Vector2 correctLoc;
    //当前坐标
    public Vector2 currentLoc = new Vector2(-1,-1);

    void Update()
    {
        //拼图碎片被选中时会随着鼠标移动
        if (IsSelected)
            transform.position = Input.mousePosition;
    }
}