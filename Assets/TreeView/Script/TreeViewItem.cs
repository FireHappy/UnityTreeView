using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TreeView.Script
{
    /// <summary>
    /// 树形菜单元素
    /// </summary>
    public class TreeViewItem : MonoBehaviour
    {
        /// <summary>
        /// 树形菜单控制器
        /// </summary>
        public TreeViewControl Controller;
        /// <summary>
        /// 当前元素的子元素是否展开（展开时可见）
        /// </summary>
        public bool IsExpanding = false;
        /// <summary>
        /// 数据的引用
        /// </summary>
        public TreeViewData Data { get; set; }

        //当前元素指向的父元素
        private TreeViewItem _parent;
        //当前元素的所有子元素
        private List<TreeViewItem> _children;
        //正在进行刷新
        private bool _isRefreshing = false;

        public void Init(TreeViewData data)
        {
            this.Data = data;
        }

        void Awake()
        {
            //上下文按钮点击回调
            transform.FindChild("ContextButton").GetComponent<Button>().onClick.AddListener(ContextButtonClick);
            transform.FindChild("TreeViewButton").GetComponent<Button>().onClick.AddListener(delegate()
            {
                Controller.ClickItem(gameObject);
            });
        }
        /// <summary>
        /// 点击上下文菜单按钮，元素的子元素改变显示状态
        /// </summary>
        public void ContextButtonClick()
        {
            //上一轮刷新还未结束
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;

            if (IsExpanding)
            {
                transform.FindChild("ContextButton").GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
                IsExpanding = false;
                ChangeChildren(this, false);
            }
            else
            {
                transform.FindChild("ContextButton").GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
                IsExpanding = true;
                ChangeChildren(this, true);
            }

            //刷新树形菜单
            Controller.RefreshTreeView();
            _isRefreshing = false;
        }

        /// <summary>
        /// 改变某一元素所有子元素的显示状态
        /// </summary>
        void ChangeChildren(TreeViewItem tvi, bool value)
        {
            for (int i = 0; i < tvi.GetChildrenNumber(); i++)
            {
                tvi.GetChildrenByIndex(i).gameObject.SetActive(value);
                if (tvi.GetChildrenByIndex(i).IsExpanding)
                {
                    ChangeChildren(tvi.GetChildrenByIndex(i), value);
                }
            }
        }

        /// <summary>
        /// 点击上下文菜单按钮，元素的子元素改变显示状态
        /// </summary>
        public void SetExpanding (bool isExpanding)
        {
            //上一轮刷新还未结束
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;

            transform.FindChild("ContextButton").GetComponent<RectTransform>().localRotation = isExpanding ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 90);
            IsExpanding = isExpanding;
            ChangeChildren(this, isExpanding);

            //刷新树形菜单
            Controller.RefreshTreeView();
            _isRefreshing = false;
        }

        #region 属性访问

        public TreeViewItem GetParent()
        {
            return _parent;
        }
        public void SetParent(TreeViewItem parent)
        {
            _parent = parent;
        }
        public void AddChildren(TreeViewItem children)
        {
            if (_children == null)
            {
                _children = new List<TreeViewItem>();
            }
            _children.Add(children);
        }
        public void RemoveChildren(TreeViewItem children)
        {
            if (_children == null)
            {
                return;
            }
            _children.Remove(children);
        }
        public void RemoveChildren(int index)
        {
            if (_children == null || index < 0 || index >= _children.Count)
            {
                return;
            }
            _children.RemoveAt(index);
        }
        public int GetChildrenNumber()
        {
            if (_children == null)
            {
                return 0;
            }
            return _children.Count;
        }
        public TreeViewItem GetChildrenByIndex(int index)
        {
            if (index >= _children.Count)
            {
                return null;
            }
            return _children[index];
        }
        #endregion
    }
}
