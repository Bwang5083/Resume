using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace BookApplication
{
    public class TreeViewDataMV : INotifyPropertyChanged
    {
        //two first level tree nodes: My Digital Resources, Online Shopping Mall
        private List<TreeNode> firstLevelNode;
        //each first level tree node has a number of second level tree nodes
        public Dictionary<int, List<TreeNode>> secondLevelNode;
        //tree node collection which will be binded with tree control - My Digital Resources
        private TreeNodeCollection digitalResources;
        //tree node collection which will be binded with tree control - Online Shopping Mall
        private TreeNodeCollection shoppingMall;
        private Collection<TreeNodeCollection> nodeCollection;

        public TreeViewDataMV()
        {
            secondLevelNode = new Dictionary<int, List<TreeNode>>();
        }

        public TreeNodeCollection DigitalResources
        {
            get
            {
                return digitalResources;
            }
            set
            {
                digitalResources = value;
                NotifyPropertyChanged();
            }
        }

        public TreeNodeCollection ShoppingMall
        {
            get
            {
                return shoppingMall;
            }
            set
            {
                shoppingMall = value;
                NotifyPropertyChanged();
            }
        }

        public Collection<TreeNodeCollection> NodeCollection
        {
            get
            {
                return nodeCollection;
            }
            set
            {
                nodeCollection = value;
                NotifyPropertyChanged();
            }
        }

        public List<TreeNode> FirstLevelNode
        {
            get
            {
                return firstLevelNode;
            }
            set
            {
                firstLevelNode = value;
                NotifyPropertyChanged();
            }
        }

        #region
        public void NotifyPropertyChanged([CallerMemberName] string propertyName="")
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public void InitializeTreeViewData()
        {
            //get all second level tree nodes of My Digital Resources
            List<TreeNode> digitalTreeNodes = secondLevelNode[FirstLevelNode[0].NodeID];
            var digitalResources = new TreeNodeCollection("DigitalResources");
            for(int i=0; i<digitalTreeNodes.Count; i++)
            {
                digitalResources.TreeNodes.Add(new TreeNode(digitalTreeNodes[i].NodeID, digitalTreeNodes[i].ParentID, digitalTreeNodes[i].NodeName, digitalTreeNodes[i].TableName, digitalTreeNodes[i].Icon));
            };

            //get all second level tree nodes of Online Shopping Mall
            List<TreeNode> shoppingMallNodes = secondLevelNode[FirstLevelNode[1].NodeID];
            var shoppingmall = new TreeNodeCollection("shoppingmall");
            for (int i = 0; i < shoppingMallNodes.Count; i++)
            {
                shoppingmall.TreeNodes.Add(new TreeNode(shoppingMallNodes[i].NodeID, shoppingMallNodes[i].ParentID, shoppingMallNodes[i].NodeName, shoppingMallNodes[i].TableName, shoppingMallNodes[i].Icon));
            };

            var nodeCollection = new Collection<TreeNodeCollection>() { digitalResources, shoppingmall };

            //My Digital Resources tree nodes collection, bind with TreeView on xaml
            DigitalResources = digitalResources;
            //Online Shopping Mall tree nodes collection, bind with TreeView on xaml
            ShoppingMall = shoppingmall;
            NodeCollection = nodeCollection;
        }
    }

    public class TreeNodeCollection
    {
        public TreeNodeCollection(string name)
        {
            Name = name;
            TreeNodes = new ObservableCollection<TreeNode>();
        }
        public string Name { get; private set; }
        public ObservableCollection<TreeNode> TreeNodes { get; private set; }
    }
}
