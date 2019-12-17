using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//拼图方格
public class PuzzleGrid : MonoBehaviour
{
    //网格坐标
    public Vector2 location;
    //拼图方格内的拼图碎片
    public ImagePart imagePart;
    //拼图方格内放置的是否是正确拼图碎片
    public bool IsCorrectIPIn
    {
        get
        {
            if (imagePart != null && location.Equals(imagePart.correctLoc))
                return true;

            return false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Equals("CursorTracer") ||
           GameMain.instance.selectedImagePart == null)
            return;
        //鼠标进入拼图方格
        Vector2 pos = transform.position;
        MarkBox.instance.selectedPuzzleGrid = this;
        MarkBox.instance.Show(pos);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Equals("CursorTracer") ||
           GameMain.instance.selectedImagePart == null)
            return;
        //鼠标离开拼图方格
        MarkBox.instance.Hide();
    }
}
