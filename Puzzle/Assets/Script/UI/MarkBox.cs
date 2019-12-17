using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//标记框，鼠标进入拼图方格时显示，单例
public class MarkBox : MonoBehaviour
{
    //静态实例
    public static MarkBox instance;
    //画布组
    private CanvasGroup canvasGroup;
    //标记框处的拼图方格
    public PuzzleGrid selectedPuzzleGrid;
    //标记框是否显示
    public bool IsShow
    {
        get => canvasGroup.alpha != 0;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    //在指定位置显示标记框
    public void Show(Vector3 pos)
    {
        transform.position = pos;
        //调整标记框的层级
        int index = GameMain.instance.puzzleRow * GameMain.instance.puzzleCol - 1;
        transform.SetSiblingIndex(index);

        canvasGroup.alpha = 1;
    }
    //隐藏标记框
    public void Hide()
    {
        selectedPuzzleGrid = null;
        //..
        transform.SetAsFirstSibling();
        canvasGroup.alpha = 0;
    }
}
