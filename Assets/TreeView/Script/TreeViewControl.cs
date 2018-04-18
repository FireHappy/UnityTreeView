using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TreeView.Script
{
    [Serializable]
    public class Padding
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    /// <summary>
    ///     树形菜单控制器
    /// </summary>
    public class TreeViewControl : MonoBehaviour
    {
        /// <summary>
        /// Padding
        /// </summary>
        public Padding Padding;
        /// <summary>
        /// 数据源
        /// </summary>
        [HideInInspector]
        public List<TreeViewData> Data = new List<TreeViewData>();

        /// <summary>
        ///     树形菜单中元素的模板
        /// </summary>
        public GameObject Template;

        /// <summary>
        ///     树形菜单中元素的根物体
        /// </summary>
        public Transform Parent;

        /// <summary>
        ///     树形菜单的纵向排列间距
        /// </summary>
        public int VerticalItemSpace = 2;

        /// <summary>
        ///     树形菜单的横向排列间距
        /// </summary>
        public int HorizontalItemSpace = 25;

        /// <summary>
        ///     树形菜单中元素的宽度
        /// </summary>
        public int ItemWidth = 230;

        /// <summary>
        ///     树形菜单中元素的高度
        /// </summary>
        public int ItemHeight = 35;

        /// <summary>
        ///     所有子元素的鼠标点击回调事件
        /// </summary>
        public delegate void ClickItemdelegate(GameObject item);

        public event ClickItemdelegate ClickItemEvent;

        /// <summary>
        /// 当前树形菜单中的所有元素
        /// </summary>
        private Dictionary<int,TreeViewItem> treeViewItems;

        //树形菜单当前刷新队列的元素位置索引
        private int yIndex;

        /// <summary>
        /// 是否正在刷新
        /// </summary>
        private bool isRefreshing;

        /// <summary>
        /// content高
        /// </summary>
        private float contentHeight=300;

        /// <summary>
        /// content宽
        /// </summary>
        private float contentWidth=300;

        /// <summary>
        ///     鼠标点击子元素事件
        /// </summary>
        public void ClickItem(GameObject item)
        {
            if (ClickItemEvent != null) ClickItemEvent(item);
        }
     
        /// <summary>
        ///     生成TreeView
        /// </summary>
        public void GenerateTreeView()
        {
            treeViewItems=new Dictionary<int, TreeViewItem>();
            for (int i = 0; i < Data.Count; i++)
            {
                GameObject go= Instantiate(Template,Parent);
                go.transform.Find("TreeViewButton/TreeViewText").GetComponent<Text>().text = Data[i].Name;
                go.SetActive(true);
                TreeViewItem item = go.GetComponent<TreeViewItem>();
                item.Init(Data[i]);
                //Debug.Log("数据的id:"+Data[i].Id);
                treeViewItems.Add(Data[i].Id,item);
            }
            //建立子父级关系
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].ParentId==0)
                {
                    continue;
                }
                TreeViewItem parent, child;
                treeViewItems.TryGetValue(Data[i].Id, out child);
                treeViewItems.TryGetValue(Data[i].ParentId, out parent);
                if (parent != null) parent.AddChildren(child);
                if (child != null) child.SetParent(parent);
            }
            //匹配Content           
            SetContentSize();
        }

        /// <summary>
        ///     设置contentSize
        /// </summary>
        private void SetContentSize()
        {
            contentHeight = treeViewItems.Count * (VerticalItemSpace + ItemHeight+Padding.Top);
            transform.GetComponent<ScrollRect>().content.sizeDelta = new Vector2(contentWidth, contentHeight);
        }

        /// <summary>
        ///     刷新树形菜单(修改item的相对位置)
        /// </summary>
        public void RefreshTreeView()
        {
            yIndex =-Padding.Top;
            if (isRefreshing)
            {
                return;
            }
            isRefreshing = true;
            foreach (KeyValuePair<int,TreeViewItem> pair in treeViewItems)
            {
                if (!pair.Value.enabled || pair.Value.GetParent() != null)
                {
                    continue;
                }
                pair.Value.transform.localPosition = new Vector3(Padding.Left, yIndex);
                yIndex += -(VerticalItemSpace + ItemHeight);
                if (pair.Value.IsExpanding && pair.Value.GetChildrenNumber() != 0)
                {
                    RefreshTreeView(pair.Value, Padding.Left);
                } 
            }
            isRefreshing = false;            
        }

        /// <summary>
        ///     设置是否展开
        /// </summary>
        /// <param name="isExpanding"></param>
        public void SetIsExpand(bool isExpanding)
        {
            foreach (KeyValuePair<int, TreeViewItem> pair in treeViewItems)
            {
                pair.Value.SetExpanding(isExpanding);
            }            
        }

        /// <summary>
        ///     刷新树形菜单(修改item的相对位置)
        /// </summary>
        /// <param name="tvi"></param>
        /// <param name="xIndex"></param>
        private void RefreshTreeView(TreeViewItem tvi,int xIndex)
        {
            xIndex += HorizontalItemSpace;
            for (int i = 0; i < tvi.GetChildrenNumber(); i++)
            {
                tvi.GetChildrenByIndex(i).transform.localPosition = new Vector3(xIndex, yIndex);
                yIndex += -(VerticalItemSpace + ItemHeight);
                if (tvi.GetChildrenByIndex(i).IsExpanding && tvi.GetChildrenByIndex(i).GetChildrenNumber() != 0)
                {
                    RefreshTreeView(tvi.GetChildrenByIndex(i), xIndex);
                }
            }
            contentWidth = xIndex + ItemWidth + Padding.Right + Padding.Left > contentWidth ? xIndex + ItemWidth + Padding.Right + Padding.Left : contentWidth;
        }
    }
}
