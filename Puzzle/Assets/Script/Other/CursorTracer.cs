using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//追踪鼠标光标、含有碰撞体便于对鼠标进行检测的类
public class CursorTracer : MonoBehaviour
{ 
    void FixedUpdate()
    {
        //追踪鼠标光标的位置
        Vector2 cursorPos = Input.mousePosition;
        transform.position = cursorPos;
    }
}
