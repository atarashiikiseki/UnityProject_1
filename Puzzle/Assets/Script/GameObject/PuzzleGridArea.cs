using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//拼图方格区域，单例
public class PuzzleGridArea : MonoBehaviour
{
    //静态实例
    public static PuzzleGridArea instance;

    public GameObject puzzleGrid;
    //拼图方格的容器
    public GameObject grid;
    //全部拼图方格
    public PuzzleGrid[,] puzzleGrids;
    //最大长度
    public float maxImageSize = 360;
    //方格之间的间隔
    public float blank = 2;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    //生成方格
    public void GenerateGrid(float ipWidth, float ipHeight, int row, int col)
    {
        float margin = grid.GetComponent<RectTransform>().sizeDelta.x / -2;
        //调整长宽
        float WID = ipWidth * col + 2 * margin;
        float HEI = ipHeight * row + 2 * margin;
        GetComponent<RectTransform>().sizeDelta = new Vector2(WID, HEI);
        //计算第一个方格的相对于锚点的坐标
        float x0 = ipWidth / 2;
        float y0 = ipHeight / 2;
        //调整标志框的尺寸
        MarkBox.instance.GetComponent<RectTransform>().sizeDelta = new Vector2(ipWidth, ipHeight);

        puzzleGrids = new PuzzleGrid[col, row];        
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                GameObject pg = Instantiate(puzzleGrid);
                RectTransform rt = pg.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(ipWidth - blank,ipHeight - blank);
                rt.anchoredPosition = new Vector2(x0 + x * ipWidth, y0 + y * ipHeight);
                pg.transform.SetParent(grid.transform,false);
                //调整碰撞体的大小
                pg.GetComponent<BoxCollider2D>().size = new Vector2(ipWidth, ipHeight);
                PuzzleGrid script = pg.GetComponent<PuzzleGrid>();
                script.location = new Vector2(x, y);
                puzzleGrids[x, y] = script;
            }
        }
    }
    //使用存档数据恢复现场
    public void Restore(Archive archive)
    {
        var pgInfos = archive.puzzleGridInfos;
        foreach(var pgInfo in pgInfos)
        {
            int x1 = pgInfo[0];
            int y1 = pgInfo[1];
            int x2 = pgInfo[2];
            int y2 = pgInfo[3];
            ImagePart ip = ImagePartArea.instance.imageParts[x2, y2];
            puzzleGrids[x1, y1].imagePart = ip;
        }
    }
    //删除全部拼图网格
    public void Clear()
    {
        foreach (var pg in puzzleGrids)
            Destroy(pg.gameObject);
        System.Array.Clear(puzzleGrids, 0, puzzleGrids.Length);
    }
}
