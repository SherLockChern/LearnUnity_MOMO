using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class LoopScrollView : MonoBehaviour {

    private List<GameObject> goList;//当前显示的go列表
    private Queue<GameObject> freeGoQueue;//空闲的go队列，存放未显示的go
    private Dictionary<GameObject, int> goIndexDic;//key:所有的go value:真实索引
    private ScrollRect scrollRect;
    private RectTransform contentRectTra;
    private Vector2 scrollRectSize;
    private Vector2 cellSize;
    private int startIndex;//起始索引
    private int maxCount;//创建的最大数量
    private int createCount;//当前显示的数量

    private const int cacheCount = 2;//缓存数目
    private const int invalidStartIndex = -1;//非法的起始索引

    private int dataCount;
    private GameObject prefabGo;
    private Action<GameObject, int> updateCellCB;
    private float cellPadding;

    //初始化SV并刷新
    public void Show(int dataCount, GameObject prefabGo, Action<GameObject, int> updateCellCB, float cellPadding = 0f)
    {
        //数据和组件初始化
        this.dataCount = dataCount;
        this.prefabGo = prefabGo;
        this.updateCellCB = updateCellCB;
        this.cellPadding = cellPadding;

        goList = new List<GameObject>();
        freeGoQueue = new Queue<GameObject>();
        goIndexDic = new Dictionary<GameObject, int>();
        scrollRect = GetComponent<ScrollRect>();
        contentRectTra = scrollRect.content;
        scrollRectSize = scrollRect.GetComponent<RectTransform>().sizeDelta;
        cellSize = prefabGo.GetComponent<RectTransform>().sizeDelta;
        startIndex = 0;
        maxCount = GetMaxCount();
        createCount = 0;

        if (scrollRect.horizontal)
        {
            contentRectTra.anchorMin = new Vector2(0, 0);
            contentRectTra.anchorMax = new Vector2(0, 1);
        }
        else
        {
            contentRectTra.anchorMin = new Vector2(0, 1);
            contentRectTra.anchorMax = new Vector2(1, 1);
        }
        scrollRect.onValueChanged.RemoveAllListeners();
        scrollRect.onValueChanged.AddListener(OnValueChanged);
        ResetSize(dataCount);
    }

    //重置数量
    public void ResetSize(int dataCount)
    {
        this.dataCount = dataCount;
        contentRectTra.sizeDelta = GetContentSize();

        //回收显示的go
        for (int i = goList.Count - 1; i >= 0; i--)
        {
            GameObject go = goList[i];
            RecoverItem(go);
        }

        //创建或显示需要的go
        createCount = Mathf.Min(dataCount, maxCount);
        for (int i = 0; i < createCount; i++)
        {
            CreateItem(i);
        }

        //刷新数据
        startIndex = -1;
        contentRectTra.anchoredPosition = Vector3.zero;
        OnValueChanged(Vector2.zero);
    }

    //更新当前显示的列表
    public void UpdateList()
    {
        for (int i = 0; i < goList.Count; i++)
        {
            GameObject go = goList[i];
            int index = goIndexDic[go];
            updateCellCB(go, index);
        }
    }

    //创建或显示一个item
    private void CreateItem(int index)
    {
        GameObject go;
        if (freeGoQueue.Count > 0)//使用原来的
        {
            go = freeGoQueue.Dequeue();
            goIndexDic[go] = index;
            go.SetActive(true);
        }
        else//创建新的
        {
            go = Instantiate<GameObject>(prefabGo);
            go.name = "item" + index;
            goIndexDic.Add(go, index);
            go.transform.SetParent(contentRectTra.transform, false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
        }
        goList.Add(go);
        go.transform.localPosition = GetPosition(index);
        updateCellCB(go, index);
    }

    //回收一个item
    private void RecoverItem(GameObject go)
    {
        go.SetActive(false);
        goList.Remove(go);
        freeGoQueue.Enqueue(go);
        goIndexDic[go] = invalidStartIndex;
    }

    //滑动回调
    private void OnValueChanged(Vector2 vec)
    {
        int curStartIndex = GetStartIndex();
        // Debug.Log(curStartIndex);

        if ((startIndex != curStartIndex) && (curStartIndex > invalidStartIndex))
        {
            startIndex = curStartIndex;

            //收集被移出去的go
            //索引的范围:[startIndex, startIndex + createCount - 1]
            for (int i = goList.Count - 1; i >= 0; i--)
            {
                GameObject go = goList[i];
                int index = goIndexDic[go];
                if (index < startIndex || index > (startIndex + createCount - 1))
                {
                    RecoverItem(go);
                }
            }

            //对移除出的go进行重新排列
            for (int i = startIndex; i < startIndex + createCount; i++)
            {
                if (i >= dataCount)
                {
                    break;
                }

                bool isExist = false;
                for (int j = 0; j < goList.Count; j++)
                {
                    GameObject go = goList[j];
                    int index = goIndexDic[go];
                    if (index == i)
                    {
                        isExist = true;
                        break;
                    }
                }
                if (isExist)
                {
                    continue;
                }

                CreateItem(i);
            }
        }
    }

    //获取需要创建的最大prefab数目
    private int GetMaxCount()
    {
        if (scrollRect.horizontal)
        {
            return Mathf.CeilToInt(scrollRectSize.x / (cellSize.x + cellPadding)) + cacheCount;
        }
        else
        {
            return Mathf.CeilToInt(scrollRectSize.y / (cellSize.y + cellPadding)) + cacheCount;
        }
    }

    //获取起始索引
    private int GetStartIndex()
    {
        if (scrollRect.horizontal)
        {
            return Mathf.FloorToInt(-contentRectTra.anchoredPosition.x / (cellSize.x + cellPadding));
        }
        else
        {
            var ret = Mathf.FloorToInt(contentRectTra.anchoredPosition.y / (cellSize.y + cellPadding));
            Debug.Log("contentRectTra: " + contentRectTra.anchoredPosition.y + " cellSize.y: " + cellSize.y + " cellPadding" + cellPadding + " ret: " + ret);
            return ret;
        }
    }

    //获取索引所在位置
    private Vector3 GetPosition(int index)
    {
        if (scrollRect.horizontal)
        {
            return new Vector3(index * (cellSize.x + cellPadding), 0, 0);
        }
        else
        {
            return new Vector3(0, index * -(cellSize.y + cellPadding), 0);
        }
    }

    //获取内容长宽
    private Vector2 GetContentSize()
    {
        if (scrollRect.horizontal)
        {
            return new Vector2(cellSize.x * dataCount + cellPadding * (dataCount - 1), contentRectTra.sizeDelta.y);
        }
        else
        {
            return new Vector2(contentRectTra.sizeDelta.x, cellSize.y * dataCount + cellPadding * (dataCount - 1));
        }
    }
}