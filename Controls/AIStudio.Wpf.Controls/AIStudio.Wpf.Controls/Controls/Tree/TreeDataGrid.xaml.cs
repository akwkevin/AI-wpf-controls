using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class TreeDataGrid : DataGrid
    {
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.NewItems != null)
                {
                    foreach (object o in e.NewItems)
                    {
                        var p = o as IBaseTreeItemViewModel;
                        if (p != null)
                        {
                            p.PropertyChanged += p_PropertyChanged;
                            p_PropertyChanged(p, new PropertyChangedEventArgs("IsExpanded"));
                        }
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (object o in e.OldItems)
                    {
                        var p = o as IBaseTreeItemViewModel;
                        if (p != null)
                        {
                            p.PropertyChanged -= p_PropertyChanged;
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (IBaseTreeItemViewModel p in this.Items.OfType<IBaseTreeItemViewModel>().ToList())
                {
                    p.PropertyChanged -= p_PropertyChanged;
                    p.PropertyChanged += p_PropertyChanged;
                    p_PropertyChanged(p, new PropertyChangedEventArgs("IsExpanded"));
                }
            }
        }

        void p_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var selectedObj = sender as BaseTreeItemViewModel;
            if (selectedObj == null)
                return;
            if (selectedObj.IsExpanded)
            {
                int next = ((IList)this.ItemsSource).IndexOf(selectedObj) + 1;
                for (int i = 0; i < selectedObj.Children.Count; i++)
                {
                    var p = selectedObj.Children[i];
                    if (this.ItemsSource.OfType<IBaseTreeItemViewModel>().FirstOrDefault(q => q == p) != null)
                        return;
                    //p.PropertyChanged += p_PropertyChanged;
                    p.IsExpanded = false;
                    ((IList)this.ItemsSource).Insert(next++, p);
                }
            }
            else if (!selectedObj.IsExpanded)
            {
                for (int i = 0; i < selectedObj.Children.Count; i++)
                {
                    var p = selectedObj.Children[i];
                    RemoveNode(p);
                }
            }
        }

        private void RemoveNode(IBaseTreeItemViewModel person)
        {
            for (int i = 0; i < person.Children.Count; i++)
            {
                var p = person.Children[i];
                RemoveNode(p);
                ((IList)this.ItemsSource).Remove(p);
            }
             ((IList)this.ItemsSource).Remove(person);
        }
    }

    [Serializable]
    public abstract class BaseTreeItemViewModel : INotifyPropertyChanged, IBaseTreeItemViewModel//组织机构树节点
    {
        #region 基本属性 
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        #endregion

        #region 
        private ObservableCollection<BaseTreeItemViewModel> _children;
        public ObservableCollection<BaseTreeItemViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new ObservableCollection<BaseTreeItemViewModel>();
                    _children.CollectionChanged += new NotifyCollectionChangedEventHandler(OnChildrenChanged);
                }

                return _children;
            }
        }

        protected void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Note: This section does not account for multiple items being involved in change operations.
            // Note: This section does not account for the replace operation.
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                BaseTreeItemViewModel child = (BaseTreeItemViewModel)e.NewItems[0];
                child.Parent = this;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                BaseTreeItemViewModel child = (BaseTreeItemViewModel)e.OldItems[0];
                if (child.Parent == this)
                {
                    child.Parent = null;
                }
            }
        }

        public BaseTreeItemViewModel Parent
        {
            get; set;
        }

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        private bool? _isChecked = false;
        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged("IsChecked");

                    SetChildChecked(_isChecked);


                    if (Parent != null)
                    {
                        if (Parent.Children?.Count == Parent.Children?.Count(p => p.IsChecked == true))
                        {
                            Parent.IsChecked = true;
                        }
                        else if (Parent.Children?.Count == Parent.Children?.Count(p => p.IsChecked == false))
                        {
                            Parent.IsChecked = false;
                        }
                        else
                        {
                            Parent.IsChecked = null;
                        }
                    }

                }
            }
        }

        public bool? IsCheckedOnlySelf
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        private void SetChildChecked(bool? isChecked)
        {
            if (Children != null && isChecked != null)
            {
                foreach (var child in Children)
                {
                    child.IsChecked = isChecked;
                    child.SetChildChecked(isChecked);
                }
            }
        }

        public void SetChecked(bool isChecked)
        {
            _isChecked = isChecked;
            OnPropertyChanged("IsChecked");
        }

        public int Level
        {
            get
            {
                if (Parent == null)
                {
                    return 0;
                }
                else
                {
                    return Parent.Level + 1;
                }
            }
        }

        #region TreeDataGrid专用
        public double MarginLeft
        {
            get
            {
                return Level * 20 + 10;
            }
        }
        public Visibility ChildVisible
        {
            get
            {
                if (Children.Count == 0)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public void TreeDataGridOnPropertyChanged()
        {
            OnPropertyChanged("MarginLeft");
            OnPropertyChanged("ChildVisible");
        }
        #endregion

        /// <summary>
        /// 设置所有子项展开状态
        /// </summary>
        /// <param name="isExpanded"></param>
        public void SetChildrenExpanded(bool isExpanded)
        {
            foreach (BaseTreeItemViewModel child in Children)
            {
                child.IsExpanded = isExpanded;
                child.SetChildrenExpanded(isExpanded);
            }
        }


        public void InsertChild(int index, BaseTreeItemViewModel child)
        {
            if (!Children.Contains(child))
            {
                child.Parent = this;
                child.TreeDataGridOnPropertyChanged();
                Children.Insert(index, child);
                TreeDataGridOnPropertyChanged();
            }
        }

        public void AddChild(BaseTreeItemViewModel child)
        {
            if (!Children.Contains(child))
            {
                child.Parent = this;
                child.TreeDataGridOnPropertyChanged();
                Children.Add(child);
                TreeDataGridOnPropertyChanged();
            }
        }

        public void AddChildRange(IEnumerable<BaseTreeItemViewModel> childs)
        {
            foreach (var child in childs)
            {
                if (!Children.Contains(child))
                {
                    child.Parent = this;
                    child.TreeDataGridOnPropertyChanged();
                    Children.Add(child);
                }
            }
            TreeDataGridOnPropertyChanged();
        }

        public void RemoveChild(BaseTreeItemViewModel child)
        {
            if (Children.Contains(child))
            {
                child.Parent = null;
                Children.Remove(child);
                TreeDataGridOnPropertyChanged();
            }
        }

        public void ClearChild()
        {
            if (_children != null)
            {
                _children.Clear();
                TreeDataGridOnPropertyChanged();
            }
        }
        #endregion       

        public IEnumerable<BaseTreeItemViewModel> GetHierarchy()
        {
            return GetAscendingHierarchy().Reverse();
        }

        public IEnumerable<BaseTreeItemViewModel> GetChildren()
        {
            return this.Children;
        }

        private IEnumerable<BaseTreeItemViewModel> GetAscendingHierarchy()
        {
            var vm = this;
            yield return vm;
            while (vm.Parent != null)
            {
                yield return vm.Parent;
                vm = vm.Parent;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public interface IBaseTreeItemViewModel
    {
        string Name
        {
            get;
        }
        ObservableCollection<BaseTreeItemViewModel> Children
        {
            get;
        }
        bool IsExpanded
        {
            get; set;
        }
        bool IsSelected
        {
            get; set;
        }
        bool? IsChecked
        {
            get; set;
        }
        IEnumerable<BaseTreeItemViewModel> GetChildren();
        IEnumerable<BaseTreeItemViewModel> GetHierarchy();

        event PropertyChangedEventHandler PropertyChanged;
    }

    public static class BaseTreeItemViewModelHelper
    {
        public static IEnumerable<IBaseTreeItemViewModel> GetChecked(IEnumerable<IBaseTreeItemViewModel> trees)
        {
            if (trees == null)
                return new List<IBaseTreeItemViewModel>();
            List<IBaseTreeItemViewModel> list = new List<IBaseTreeItemViewModel>();

            foreach (var tree in trees)
            {
                if (tree.IsChecked == true || tree.IsChecked == null)
                {
                    list.Add(tree);
                }
                list.AddRange(GetChecked(tree.Children));
            }

            return list;
        }

        public static void SetChecked(IEnumerable<IBaseTreeItemViewModel> trees, bool? isChecked)
        {
            if (trees == null || isChecked == null)
                return;

            foreach (var tree in trees)
            {
                tree.IsChecked = isChecked;

                SetChecked(tree.Children, isChecked);
            }
        }
    }
}
