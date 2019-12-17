using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//动态多选标记框，用于选中多个拼图碎片
public class DynamicMarkBox : MonoBehaviour,IPointerClickHandler
{
    //静态实例
    public static DynamicMarkBox instance;

    private CanvasGroup canvasGroup;
    public Color normalColor;
    public Color abnormalColor;
    //右键鼠标选中的点和松开鼠标时的点
    private Vector2 firstPoint;
    private Vector2 secondPoint;
    //用户选中的轴心
    private Vector2 customPivot;
    //第一个点和第二个点所在处的拼图网格
    private PuzzleGrid firstPG;
    private PuzzleGrid secondPG;
    //鼠标位置相对于被选中的位置的方向
    public Vector2 direction;
    //是否可以形成合法的形状
    private bool canFromShape
    {
        get => (firstPG != null && secondPG != null);
    }
    //是否显示
    public bool IsShow
    {
        get => canvasGroup.alpha == 1;
    }
    //形状是否已经确定
    private bool hadShaped = false;
    //是否被选中
    private bool isSelected = false;
    //被选中时的位置
    private Vector2 originPos;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    //监听鼠标
    void ListenForInput()
    {
        //单击右键
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            //选中第一个点
            if (!IsShow && !hadShaped)
                GetFirstPoint();
            //放置选中区域
            if(isSelected)
            {
                //放置到指定位置
                if (IsValidPosition() && !IsImagePartOverlap())
                    PlaceImageParts();
                //放置回原位置
                else
                    RestoreImagePartPos();
            }
        }
        //随着鼠标的移动，改变标记框的形状
        if (IsShow && !hadShaped)
            GenerateShape();
        //松开右键
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            //选中第二个点
            if (IsShow && !hadShaped)
                GetSecondPoint();
        }

        //随着鼠标的移动，移动标记框
        if (isSelected)
            MoveWithCursor();
    }
    //获取第一个点和第一个被选中的拼图网格
    public void GetFirstPoint()
    {
        firstPoint = Input.mousePosition;
        transform.position = firstPoint;
        firstPG = TryGetPuzzleGrid(firstPoint);
        if (firstPG != null)
            canvasGroup.alpha = 1;
    }
    //获取第二个点和第二个被选中的拼图网格
    public void GetSecondPoint()
    {
        secondPoint = Input.mousePosition;
        secondPG = TryGetPuzzleGrid(secondPoint);
        if (canFromShape)
            HandleSelectedArea();
        else
        {
            canvasGroup.alpha = 0;
            firstPG = null;
        }
    }
    //尝试获取指定位置处的拼图网格
    public PuzzleGrid TryGetPuzzleGrid(Vector2 pos)
    {
        //射线检测，获取指定位置
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);
        //未获取到任何物体
        if (hits.Length == 0)
            return null;
        foreach (var hit in hits)
        {            
            //检测到拼图方格
            if (hit.collider.tag.Equals("PuzzleGrid"))
                 return hit.collider.gameObject.GetComponent<PuzzleGrid>();
        }
        return null;
    }
    //生成形状
    public void GenerateShape()
    {
        Vector2 pos = Input.mousePosition;
        Vector2 pivot = new Vector2();
        pivot.x = firstPoint.x > pos.x ? 1 : 0;
        pivot.y = firstPoint.y > pos.y ? 1 : 0;
        //设置轴心
        transform.GetComponent<RectTransform>().pivot = pivot;
        //firstPoint 与 pos 均为世界坐标系的坐标
        float wid = Mathf.Abs(firstPoint.x - pos.x) / PlayerPrefs.GetFloat("scaleX");
        float hei = Mathf.Abs(firstPoint.y - pos.y) / PlayerPrefs.GetFloat("scaleY");
        //调整大小
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(wid, hei);
    }
    //处理被选中的区域，包括调整多选框的形状，以及获取被选中的拼图碎片
    public void HandleSelectedArea()
    {
        //获取被选中区域左下角与右上角的坐标
        int x0 = (int)Mathf.Min(firstPG.location.x, secondPG.location.x);
        int y0 = (int)Mathf.Min(firstPG.location.y, secondPG.location.y);
        int x1 = (int)Mathf.Max(firstPG.location.x, secondPG.location.x);
        int y1 = (int)Mathf.Max(firstPG.location.y, secondPG.location.y);
        //获取拼图碎片，同时修剪选中区域

        GameMain.instance.selectedImageParts = new List<ImagePart>();
        PuzzleGrid[,] puzzleGrids = PuzzleGridArea.instance.puzzleGrids;
        bool hadImagePart = false;
        int minX = 10000;
        int minY = 10000;
        int maxX = -1;
        int maxY = -1;
        for (int y = y0; y <= y1; y++)
        {
            bool hadImagePart_Row = false;
            for (int x = x0; x <= x1; x++)
            {
                ImagePart imagePart = puzzleGrids[x, y].imagePart;
                if (imagePart != null)
                {
                    GameMain.instance.selectedImageParts.Add(imagePart);
                    if (x < minX)
                        minX = x;
                    if (x > maxX)
                        maxX = x;
                    hadImagePart = true;
                    hadImagePart_Row = true;
                }
            }
            if(hadImagePart_Row)
            {
                if (y < minY)
                    minY = y;
                if (y > maxY)
                    maxY = y;
            }
        }
        if (hadImagePart)
        {
            //修剪选中区域

            firstPG = puzzleGrids[minX, minY];
            secondPG = puzzleGrids[maxX, maxY];
            //获取拼图碎片的宽与高
            float ipWidth = ImagePartArea.instance.IpWidth;
            float ipHeight = ImagePartArea.instance.IpHeight;
            //修改选中区域的位置与大小
            float newWidth = ipWidth * (maxX - minX + 1);
            float newHeight = ipHeight * (maxY - minY + 1);
            float scaleX = PlayerPrefs.GetFloat("scaleX");
            float scaleY = PlayerPrefs.GetFloat("scaleY");
            //被选中时的位置
            originPos = ((Vector2)firstPG.transform.position -
                new Vector2(ipWidth / 2 * scaleX, ipHeight / 2 * scaleY));

            transform.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            transform.position = originPos;
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, newHeight);
            hadShaped = true;
        }
        else
        {
            firstPG = null;
            secondPG = null;
            hadShaped = false;
            isSelected = false;
            canvasGroup.alpha = 0;
        }
    }
    //随着光标的移动，移动标记框
    public void MoveWithCursor()
    {
        float ipWidth = ImagePartArea.instance.IpWidth;
        float ipHeight = ImagePartArea.instance.IpHeight;
        Vector2 pos = Input.mousePosition;

        float scaleX = PlayerPrefs.GetFloat("scaleX");
        float scaleY = PlayerPrefs.GetFloat("scaleY");

        int x = (int)((pos.x - customPivot.x) /
            (ipWidth * scaleX));
        int y = (int)((pos.y - customPivot.y) /
            (ipHeight * scaleY));
        direction = new Vector2(x, y);

        //检查目标位置是否越界
        if (IsValidPosition())
        {
            canvasGroup.alpha = 1;
            transform.position = originPos +
                 new Vector2(x * ipWidth * scaleX, y * ipHeight * scaleY);
            GameMain.instance.MoveImageParts(direction);
            //检查选中位置是否已经含有其他拼图碎片
            if (!IsImagePartOverlap())
                GetComponent<Image>().color = normalColor;
            else
                GetComponent<Image>().color = abnormalColor;
        }
        else
            canvasGroup.alpha = 0;
    }
    //当前选中位置是否越界
    public bool IsValidPosition()
    { 
        int puzzleRow = GameMain.instance.puzzleRow;
        int puzzleCol = GameMain.instance.puzzleCol;
        Vector2 loc1 = firstPG.location;
        Vector2 loc2 = secondPG.location;
        loc1 += direction;
        loc2 += direction;
        if (loc1.x < 0 || loc1.y < 0 || loc2.x >= puzzleCol || loc2.y >= puzzleRow)
            return false;
        return true;
    }
    //选中位置是否有其他的拼图碎片
    public bool IsImagePartOverlap()
    {
        Vector2 loc1 = firstPG.location;
        Vector2 loc2 = secondPG.location;
        loc1 += direction;
        loc2 += direction;
        int minX = (int)firstPG.location.x;
        int maxX = (int)secondPG.location.x;
        int minY = (int)firstPG.location.y;
        int maxY = (int)secondPG.location.y;
        //所有拼图碎片
        PuzzleGrid[,] puzzleGrids = PuzzleGridArea.instance.puzzleGrids;
        for(int y = (int)loc1.y; y <= (int)loc2.y; y++)
        {
            for(int x = (int)loc1.x; x <= (int)loc2.x; x++)
            {
                if(x > maxX || x < minX || y > maxY || y < minY)
                {
                    if (puzzleGrids[x, y].imagePart != null)
                        return true;
                }
            }
        }
        return false;
    }
    //放置被选中的拼图碎片
    public void PlaceImageParts()
    {
        GameMain.instance.MoveImageParts(direction,true);
        firstPG = null;
        secondPG = null;
        hadShaped = false;
        isSelected = false;
        direction = Vector2.zero;
        canvasGroup.alpha = 0;
    }
    //将被选中的拼图碎片放回其原来的位置
    public void RestoreImagePartPos()
    {
        GameMain.instance.MoveImageParts(Vector2.zero);
        firstPG = null;
        secondPG = null;
        hadShaped = false;
        isSelected = false;
        direction = Vector2.zero;
        canvasGroup.alpha = 0;
        GetComponent<Image>().color = normalColor;
    }
    //右键点击选中标记框
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button.Equals(PointerEventData.InputButton.Right) && hadShaped && !isSelected)
        {
            customPivot = Input.mousePosition;
            isSelected = true;
        }
    }
    void Update()
    {
        if (GameMain.instance.IsListenForInput)
            ListenForInput();
    }
}
