using System.Collections.Generic;
using Assets.TreeView.Script;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TreeView
{
    public class TreeViewTest : MonoBehaviour 
    {
        public TreeViewControl TreeView;
    
        void Awake()
        {
            //生成数据
            List<TreeViewData> datas = new List<TreeViewData>();

            TreeViewData data = new TreeViewData();
            data.Name = "第一章";
            data.Id = 1;
            data.ParentId = 0;//parentid 为0,说明没有付级别
            datas.Add(data);

            TreeViewData data0 = new TreeViewData();
            data0.Name = "第二章";
            data0.Id = 3;
            data0.ParentId = 0;//parentid 为0,说明没有付级别
            datas.Add(data0);

            TreeViewData data1 = new TreeViewData();
            data1.Name = "第一节";
            data1.Id = 2;
            data1.ParentId = 1;
            datas.Add(data1);


            TreeViewData data3 = new TreeViewData();
            data3.Name = "第二节";
            data3.Id = 4;
            data3.ParentId = 2;
            datas.Add(data3);

            TreeView.Data = datas;
            //生成treeVive
            TreeView.GenerateTreeView();
            //刷新树形菜单,修改tree的相对位置
            TreeView.RefreshTreeView();
            //注册子元素的鼠标点击事件
            TreeView.ClickItemEvent += CallBack;
            //设置是否展开
            TreeView.SetIsExpand(false);
        }  

        void CallBack(GameObject item)
        {
            Debug.Log("点击了 " + item.transform.FindChild("TreeViewButton/TreeViewText").GetComponent<Text>().text);
        }
    }
}
