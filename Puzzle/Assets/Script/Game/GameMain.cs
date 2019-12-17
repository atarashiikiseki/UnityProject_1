using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//处理游戏主要逻辑的类，单例
public class GameMain : MonoBehaviour
{
    //静态实例
    public static GameMain instance;
    //拼图行数
    public int puzzleRow = 8;
    //拼图列数
    public int puzzleCol = 12;
    //未正确放置的拼图碎片的数量
    public int errorPosIPCount;
    //是否已经完成拼图
    public bool IsFinished
    {
        get => errorPosIPCount == 0;
    }
    //被选中的拼图碎片
    public ImagePart selectedImagePart;
    //被选中的多个拼图碎片
    public List<ImagePart> selectedImageParts;

    //是否监听鼠标输入
    public bool IsListenForInput = false;    

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        //开始新的游戏，还是从存档开始游戏
        if (PlayerPrefs.GetInt("isNewGame") == 1)
            NewPuzzle();
        else
            ContinueGame(Archive.GetArchive());
    }   
    //初始化一个新的拼图
    public void NewPuzzle()
    {
        //获取拼图的行数和列数
        puzzleRow = PlayerPrefs.GetInt("puzzleRow");
        puzzleCol = PlayerPrefs.GetInt("puzzleCol");
        //初始化未正确放置的拼图单元的数量
        errorPosIPCount = puzzleRow * puzzleCol;

        //获取资源图片
        PuzzleImages.instance.TryGetNextImage(out Texture2D resourcesImg);

        //更新UI界面，必须在获取资源图片之后
        MainPanel.instance.OnNewGame();

        //生成拼图碎片
        ImagePartArea.instance.GenerateImageParts(resourcesImg,puzzleRow, puzzleCol);
        //乱序排列拼图碎片
        ImagePartArea.instance.RandomPlace(10);

        //生成拼图网格
        float ipWidth = ImagePartArea.instance.IpWidth;
        float ipHeith = ImagePartArea.instance.IpHeight;
        PuzzleGridArea.instance.GenerateGrid(ipWidth, ipHeith, puzzleRow, puzzleCol);

        IsListenForInput = true;
    }
    //从存档开始游戏
    public void ContinueGame(Archive archive)
    {
        //获取拼图的行数和列数
        puzzleRow = archive.puzzleRow;
        puzzleCol = archive.puzzleCol;
        //更新拼图的行数和列数信息
        PlayerPrefs.SetInt("puzzleRow",puzzleRow);
        PlayerPrefs.SetInt("puzzleCol",puzzleCol);
        //获取资源图片
        PuzzleImages.instance.GetAllImage();
        PuzzleImages.instance.LoadDataFromArchive(archive);
        PuzzleImages.instance.TryGetImage(archive.currentLevel,out Texture2D resourcesImg);

        //更新UI界面
        MainPanel.instance.OnNewGame();

        //未正确放置的拼图单元的数量
        errorPosIPCount = archive.errorPosIPCount;

        //生成拼图碎片
        ImagePartArea.instance.GenerateImageParts(resourcesImg, puzzleRow, puzzleCol);
        //根据存档数据恢复现场
        ImagePartArea.instance.Restore(archive);

        //生成拼图网格
        float ipWidth = ImagePartArea.instance.IpWidth;
        float ipHeith = ImagePartArea.instance.IpHeight;
        PuzzleGridArea.instance.GenerateGrid(ipWidth, ipHeith, puzzleRow, puzzleCol);
        //根据存档数据恢复现场
        PuzzleGridArea.instance.Restore(archive);

        //存档时已经完成拼图
        if (IsFinished)
            FinishedPuzzle();
        else
            IsListenForInput = true;
    }
    //清理拼图拼图碎片和拼图网格
    public void Clear()
    {
        PuzzleGridArea.instance.Clear();
        ImagePartArea.instance.Clear();
    }
    //获取指定位置的拼图碎片
    public void TryGetImagePart(Vector2 pos)
    {
        //射线检测，获取指定位置
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);
        //未获取到任何物体
        if (hits.Length == 0)
            return;
        //记录获取的拼图碎片的深度
        int maxIndex = -1;
        //记录鼠标处的拼图方格
        PuzzleGrid puzzleGrid = null;
        GameObject target = null;
        foreach (var hit in hits)
        {
            //检测到拼图碎片
            if (hit.collider.tag.Equals("ImagePart"))
            {
                //获取最靠近相机的拼图碎片
                int index = hit.collider.gameObject.transform.GetSiblingIndex();
                if (index > maxIndex)
                {
                    target = hit.collider.gameObject;
                    maxIndex = index;
                }
            }
            //检测到拼图方格
            else if (hit.collider.tag.Equals("PuzzleGrid"))
                puzzleGrid = hit.collider.gameObject.GetComponent<PuzzleGrid>();
        }

        if (target == null)
            return;

        //获取的是拼图方格中的拼图碎片
        if (puzzleGrid != null)
        {
            //方格当前的拼图碎片是正确的
            if (puzzleGrid.IsCorrectIPIn)
                errorPosIPCount++;
            puzzleGrid.imagePart = null;
        }
        //更新当前选中的拼图碎片
        selectedImagePart = target.GetComponent<ImagePart>();
        selectedImagePart.IsSelected = true;
        selectedImagePart.transform.SetAsLastSibling();
    }
    //交换选中的拼图碎片与当前被选中的拼图方格中的拼图碎片
    public void SwapImagePart(PuzzleGrid pg)
    {
        //获取方格中的拼图的碎片
        ImagePart preIP = pg.imagePart;
        //之前放置的拼图碎片是正确的
        if (pg.IsCorrectIPIn)
            errorPosIPCount++;

        //将被选中的拼图碎片放到指定位置
        PlaceImagePart(pg);
        //获取拼图网格中原有的拼图碎片
        selectedImagePart = preIP;
        selectedImagePart.IsSelected = true;
        selectedImagePart.transform.SetAsLastSibling();
    }
    //将拼图碎片放置到指定的拼图方格
    public void PlaceImagePart(PuzzleGrid pg)
    {
        selectedImagePart.transform.position = pg.gameObject.transform.position;

        pg.imagePart = selectedImagePart;
        //记录拼图碎片当前的位置
        selectedImagePart.currentLoc = pg.location;
        //将拼图碎片放置到了正确的位置
        if (pg.IsCorrectIPIn)
        {
            errorPosIPCount--;
            if (errorPosIPCount == 0)
                FinishedPuzzle();
        }
        //丢弃被选中的拼图碎片
        DiscardImagePart();
        //隐藏标记框
        MarkBox.instance.Hide();
    }
    //将多个拼图碎片进行移动
    public void MoveImageParts(Vector2 direction,bool needPlace = false)
    {
        if (needPlace)
        {
            foreach (var ip in selectedImageParts)
            {
                Vector2 prePGPos = ip.currentLoc;
                PuzzleGrid prePG =
                    PuzzleGridArea.instance.puzzleGrids[(int)prePGPos.x, (int)prePGPos.y];
                if (prePG.IsCorrectIPIn)
                    errorPosIPCount++;
                prePG.imagePart = null;
            }
            foreach (var ip in selectedImageParts)
            {
                Vector2 targetPGPos = ip.currentLoc + direction;
                PuzzleGrid targetPG =
                    PuzzleGridArea.instance.puzzleGrids[(int)targetPGPos.x, (int)targetPGPos.y];
                selectedImagePart = ip;
                PlaceImagePart(targetPG);
            }
            selectedImagePart = null;
        }
        else
        {
            foreach (var ip in selectedImageParts)
            {
                Vector2 targetPGPos = ip.currentLoc + direction;
                PuzzleGrid targetPG =
                    PuzzleGridArea.instance.puzzleGrids[(int)targetPGPos.x, (int)targetPGPos.y];
                ip.transform.position = targetPG.transform.position;
            }
        }
    }
    //丢弃被选中的拼图碎片
    public void DiscardImagePart()
    {
        selectedImagePart.IsSelected = false;
        selectedImagePart = null;
    }      
    //监听鼠标输入
    void ListenForInput()
    {        
        //点击鼠标左键
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (DynamicMarkBox.instance.IsShow)
                return;
            //没有拼图碎片被选中时，尝试寻找拼图碎片
            if (selectedImagePart == null)
            {
                //获取鼠标位置
                Vector2 mousePos = Input.mousePosition;
                TryGetImagePart(mousePos);
            }
            //有拼图碎片被选中时
            else
            {
                //鼠标在拼图方格内
                if (MarkBox.instance.IsShow)
                {
                    //获取被选中的网格
                    PuzzleGrid selectedPG = MarkBox.instance.selectedPuzzleGrid;
                    //被选中的拼图方格内没有拼图碎片，直接将拼图碎片放入其中
                    if (selectedPG.imagePart == null)
                        PlaceImagePart(selectedPG);
                    //否则与拼图方格内的拼图碎片交换
                    else
                        SwapImagePart(selectedPG);
                }
                //鼠标在拼图碎片区域内，将其放到鼠标位置
                else if (ImagePartArea.instance.IsImagePartInTheIPArea(selectedImagePart))
                    DiscardImagePart();
            }
        }
    }
    //完成拼图
    void FinishedPuzzle()
    {
        IsListenForInput = false;
        MainPanel.instance.OnGameFinished();
    }
    void Update()
    {
        if (IsListenForInput)
            ListenForInput();
    }
}
