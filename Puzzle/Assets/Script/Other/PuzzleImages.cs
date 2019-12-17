using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//管理拼图中使用的图片资源的类，单例  
public class PuzzleImages : MonoBehaviour
{
    //静态实例
    public static PuzzleImages instance;

    //存放默认图片资源的文件的相对路径
    public string defaultImagePath = "PuzzleImage";
    //存放用户图片资源的文件的相对路径
    public string customImgPath = "MyImage";
    //全部的图片
    private List<Texture2D> allImages;
    //全部图片的名称
    public List<string> allImageNames;
    //当前图片的名称
    public string CurrentImgName
    {
        get => allImageNames[count];
    }
    //使用的最新的图片序号
    public int maxCount = -1;
    //当前使用的图片序号
    public int count = -1;
    //是否还有图片
    public bool HaveImg
    {
        get => (count + 1) < allImages.Count;
    }
    //是否有用户图片
    public bool HaveCustomImg
    {
        get
        {            
            string path = Path.Combine(Application.dataPath, customImgPath);
            string[] paths = Directory.GetFiles(path);
            foreach (var p in paths)
            {
                if (p.EndsWith(".jpg") || p.EndsWith(".png"))
                    return true;
            }
            return false;
        }
    }
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    //获取全部拼图图片
    public void GetAllImage(bool useCustomImg = false)
    {
        allImages = new List<Texture2D>();
        allImageNames = new List<string>();
        //使用默认图片
        if (!useCustomImg)
        {
            allImages.AddRange(Resources.LoadAll<Texture2D>(defaultImagePath));
            //保存图片的名称
            foreach (var image in allImages)
                allImageNames.Add(image.name);
        }
        //使用用户图片
        else
        {
            //获取用户图片
            string path = Path.Combine(Application.dataPath, customImgPath);
            string[] paths = Directory.GetFiles(path);
            FileStream fs;
            foreach (var p in paths)
            {
                if (p.EndsWith(".jpg") || p.EndsWith(".png"))
                {
                    fs = File.OpenRead(p);
                    byte[] bs = new byte[fs.Length];
                    fs.Read(bs, 0, (int)fs.Length);
                    Texture2D t2d = new Texture2D(0, 0);
                    t2d.LoadImage(bs);
                    allImages.Add(t2d);
                    //保存图片的名称
                    allImageNames.Add(Path.GetFileNameWithoutExtension(fs.Name));
                }
            }
        }

    }
    //尝试获取下一个拼图图片
    public bool TryGetNextImage(out Texture2D image)
    {
        image = null;
        if(!HaveImg)
            return false;
        image = allImages[++count];
        if (count > maxCount)
            maxCount = count;
        return true;
    }
    //尝试获取一个拼图图片
    public bool TryGetImage(int index, out Texture2D image)
    {
        image = null;
        if (index < 0 || index >= allImages.Count)
            return false;
        image = allImages[index];
        count = index;
        if (count > maxCount)
            maxCount = count;
        return true;
    }
    //从存档读取数据
    public void LoadDataFromArchive(Archive archive)
    {
        count = archive.currentLevel;
        maxCount = archive.maxLevel;
    }
}
