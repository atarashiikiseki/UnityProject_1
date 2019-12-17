using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

//存档类，本游戏只有一个存档
[System.Serializable]
public class Archive
{
    //存档文件的相对路径
    public static string path = "Save.sav";
    //进行到的最大关卡
    public int maxLevel;
    //当前正在进行的关卡
    public int currentLevel;
    //拼图的列数与行数
    public int puzzleRow;
    public int puzzleCol;
    //拼图碎片的位置（correctPos = (x,y)的拼图碎片实际的位置）
    //使用 Unity.Vector2 序列化时会产生不必要的数据，故使用KeyValuePair
    public KeyValuePair<float, float>[,] imagePartPos;
    //拼图网格内的拼图碎片的数据(对于a[] location = (a[0],a[1])的拼图网格处是 correctLos = (a[2],a[3])的拼图碎片)
    public List<int[]> puzzleGridInfos = new List<int[]>();
    //拼图碎片的层级
    public int[,] floors;
    //未正确放置的拼图碎片的数量
    public int errorPosIPCount;

    private Archive() { }
    //从本地文件中获取存档
    public static Archive GetArchive()
    {
        string path = Path.Combine(Application.dataPath, Archive.path);
        try
        {
            string jsonData = File.ReadAllText(path);
            Archive archive = JsonConvert.DeserializeObject<Archive>(jsonData);
            return archive;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }
    //生成存档，并保存到本地
    public static void Save()
    {
        Archive archive = new Archive();
        //保存当前关卡和进行到的最大关卡

        archive.currentLevel = PuzzleImages.instance.count;
        //获取之前的存档
        Archive preArchive = GetArchive();
        if (preArchive != null)
        {
            int preMax = preArchive.maxLevel;
            int crMax = PuzzleImages.instance.maxCount;
            archive.maxLevel = Mathf.Max(preMax, crMax);
        }
        else
            archive.maxLevel = PuzzleImages.instance.maxCount;
        //保存拼图的行数和列数
        int puzzleRow = GameMain.instance.puzzleRow;
        archive.puzzleRow = puzzleRow;
        int puzzleCol = GameMain.instance.puzzleCol;
        archive.puzzleCol = puzzleCol;
        //保存位置错误的拼图碎片的个数
        archive.errorPosIPCount = GameMain.instance.errorPosIPCount;
        //记录拼图碎片的位置和层级
        ImagePart[,] imageParts = ImagePartArea.instance.imageParts;
        archive.imagePartPos = new KeyValuePair<float, float>[archive.puzzleCol, archive.puzzleRow];
        archive.floors = new int[archive.puzzleCol, archive.puzzleRow];
        foreach (var ip in imageParts)
        {
            int x = (int)ip.correctLoc.x;
            int y = (int)ip.correctLoc.y;
            Vector2 pos = ip.transform.position;
            archive.imagePartPos[x, y] = new KeyValuePair<float, float>(pos.x, pos.y);
            archive.floors[x, y] = ip.gameObject.transform.GetSiblingIndex();
        }
        //记录拼图网格
        PuzzleGrid[,] puzzleGrids = PuzzleGridArea.instance.puzzleGrids;
        for (int y1 = 0; y1 < puzzleRow; y1++)
        {
            for (int x1 = 0; x1 < puzzleCol; x1++)
            {
                if (puzzleGrids[x1, y1].imagePart != null)
                {
                    ImagePart ip = puzzleGrids[x1, y1].imagePart;
                    int x2 = (int)ip.correctLoc.x;
                    int y2 = (int)ip.correctLoc.y;
                    archive.puzzleGridInfos.Add(new int[4] { x1, y1, x2, y2 });
                }
            }
        }
        //加密数据..

        //生成文件，写入数据
        string jsonText = JsonConvert.SerializeObject(archive);
        string path = Path.Combine(Application.dataPath,Archive.path);
        FileStream fs = File.Open(path, FileMode.Create);
        fs.Close();
        File.WriteAllText(path,jsonText);
    }
}
