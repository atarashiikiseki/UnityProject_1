using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//拼图碎片的区域，单例
public class ImagePartArea : MonoBehaviour
{
    //静态实例
    public static ImagePartArea instance;
    //拼图碎片的长和宽
    private float ipWidth;
    private float ipHeight;
    public float IpWidth { get => ipWidth; }
    public float IpHeight { get => ipHeight;}
    //边距
    public float margin;

    public GameObject imagePart;
    //全部拼图碎片
    public ImagePart[,] imageParts;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    //生成拼图碎片
    public void GenerateImageParts(Texture2D image,int row, int col)
    {
        //调整拼图碎片的尺寸

        float imgWidth = image.width;
        float imgHeight = image.height;
        //保存调整后的图片尺寸
        float imgWidth2 = imgWidth;
        float imgHeight2 = imgHeight;
        //拼图碎片区域是固定大小的，拼图网格区域大小不确定，但是有限制，根据此来确定如何修改
        //图片碎片尺寸
        float maxLength = PuzzleGridArea.instance.maxImageSize;
        float ratio = Mathf.Min((imgWidth / imgHeight),(imgHeight / imgWidth));
        if(imgWidth >= imgHeight)
        {
            if(imgWidth > maxLength)
            {
                imgWidth2 = maxLength;
                imgHeight2 = imgWidth2 * ratio;
            }
        }
        else
        {
            if(imgHeight > maxLength)
            {
                imgHeight2 = maxLength;
                imgWidth2 = imgHeight2 * ratio;
            }
        }
        //计算拼图碎片的尺寸
        ipWidth = imgWidth2 / col;
        ipHeight = imgHeight2 / row;
        imageParts = new ImagePart[col, row];
        float dImgWidth = image.width / col;
        float dImgHeight = image.height / row;
        //生成拼图碎片
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                GameObject ip = Instantiate(imagePart);
                ip.transform.SetParent(transform,false);
                ImagePart script = ip.GetComponent<ImagePart>();
                //分割图片
                float x0 = x * dImgWidth;
                float y0 = y * dImgHeight;
                Rect rect = new Rect(x0, y0, dImgWidth, dImgHeight);
                Sprite sprite = Sprite.Create(image, rect, Vector2.zero);
                ip.GetComponent<Image>().sprite = sprite;
                ip.GetComponent<RectTransform>().sizeDelta = new Vector2(ipWidth, ipHeight);
                //调整碰撞盒的尺寸
                ip.GetComponent<BoxCollider2D>().size = new Vector2(ipWidth, ipHeight);
                script.correctLoc = new Vector2(x, y);
                imageParts[x, y] = script;
            }
        }
    }
    //利用存档数据恢复拼图碎片的位置和层级
    public void Restore(Archive archive)
    {
        KeyValuePair<float,float>[,] pos = archive.imagePartPos;
        int[,] floors = archive.floors;
        foreach(var ip in imageParts)
        {
            int x = (int)ip.correctLoc.x;
            int y = (int)ip.correctLoc.y;
            ip.transform.position = new Vector3(pos[x, y].Key, pos[x, y].Value, 0);
            ip.gameObject.transform.SetSiblingIndex(floors[x,y]);
        }
    }
    //删除所有拼图碎片
    public void Clear()
    {
        foreach(var ip in imageParts)
            Destroy(ip.gameObject);
        System.Array.Clear(imageParts, 0, imageParts.Length);
    }
    //交换两个拼图碎片的位置
    public void SwapImagePart(Vector2 index1,Vector2 index2)
    {
        Vector2 tempPos = imageParts[(int)index1.x,(int)index1.y].GetComponent<RectTransform>().anchoredPosition;
        imageParts[(int)index1.x, (int)index1.y].GetComponent<RectTransform>().anchoredPosition =
            imageParts[(int)index2.x, (int)index2.y].GetComponent<RectTransform>().anchoredPosition;
        imageParts[(int)index2.x, (int)index2.y].GetComponent<RectTransform>().anchoredPosition = tempPos;
    }
    //乱序排列拼图碎片
    //参数为分散的程度
    public void RandomPlace(float dispersion)
    {
        int amount = imageParts.Length;
        /*
        
        口 口 口 口 口    口 口 口 
        口 口 口 口 口 -> 口 口 口 口
        口 口 口 口 口    口 口 口 口
                         口 口 口 口

        */
        int puzzleRow = GameMain.instance.puzzleRow;
        int puzzleCol = GameMain.instance.puzzleCol; 
        float WID = GetComponent<RectTransform>().sizeDelta.x;
        float HEI = GetComponent<RectTransform>().sizeDelta.y;
        int Len = Mathf.CeilToInt(Mathf.Sqrt(amount));
        float dx = (WID - 2 * margin) / Len;
        float dy = (HEI - 2 * margin) / Len;
        //第一次乱序，不改变拼图碎片的相对位置，仅仅使拼图碎片稍微偏移
        for (int y = 0; y < Len; y++)
        {
            for (int x = 0; x < Len; x++)
            {
                int count = y * Len + x;
                if (count >= amount)
                    break;

                Vector2 pos = new Vector2(margin + x * dx, margin + y * dy);
                pos += new Vector2(ipWidth / 2, ipHeight / 2);
                float offsetX = Random.Range(-1 * dispersion, dispersion);
                float offsetY = Random.Range(-1 * dispersion, dispersion);
                pos += new Vector2(offsetX, offsetY);

                int x1 = count % puzzleCol;
                int y1 = count / puzzleCol;
                imageParts[x1, y1].GetComponent<RectTransform>().anchoredPosition = pos;
            }
        }
        for (int y1 = 0; y1 < puzzleRow; y1++)
        {
            for (int x1 = 0; x1 < puzzleCol; x1++)
            {
                Vector2 index1 = new Vector2(x1, y1);
                int x2 = Random.Range(0, puzzleCol);
                int y2 = Random.Range(0, puzzleRow);
                Vector2 index2 = new Vector2(x2,y2);
                SwapImagePart(index1, index2);
            }
        }
    }
    //判断拼图碎片是否在拼图碎片区域内
    public bool IsImagePartInTheIPArea(ImagePart imagePart)
    {
        Vector2 aPos = imagePart.gameObject.GetComponent<RectTransform>().anchoredPosition;
        //获取拼图碎片的尺寸
        float WID = GetComponent<RectTransform>().sizeDelta.x;
        if (aPos.x > ipWidth / 2 && aPos.x < WID - ipWidth / 2
            && aPos.y > ipHeight / 2 && aPos.y < WID - ipHeight / 2)
            return true;

        return false;
    }
}
