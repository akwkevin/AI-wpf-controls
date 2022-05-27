using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// OptionList.xaml 的交互逻辑
    /// </summary>
    public partial class OptionList : Control
    {
        static OptionList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OptionList),
                new FrameworkPropertyMetadata(typeof(OptionList)));
        }

        /// <summary>
        /// 存储数据源的字典
        /// </summary>
        Dictionary<string, bool> itemsDic = new Dictionary<string, bool>();

        //点击保存的事假
        public event EventHandler<EventArgs> SaveEvent;

        ///<summary>
        ///是否显示已成交
        ///</summary>
        public OptionOrdt SelectOption
        {
            get { return (OptionOrdt)GetValue(SelectOptionProperty); }
            set { SetValue(SelectOptionProperty, value); }
        }

        public static readonly DependencyProperty SelectOptionProperty = DependencyProperty.Register("SelectOption", typeof(OptionOrdt), typeof(OptionList), new FrameworkPropertyMetadata(null));

        ///<summary>
        ///全选
        ///</summary>
        public bool? IsCheck
        {
            get { return (bool?)GetValue(IsCheckProperty); }
            set { SetValue(IsCheckProperty, value); }
        }

        public static readonly DependencyProperty IsCheckProperty = DependencyProperty.Register("IsCheck", typeof(bool?), typeof(OptionList), new FrameworkPropertyMetadata(true, IsCheckChanged));

        private static void IsCheckChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OptionList option)
            {
                option.OptionList_IsCheckChanged();
            }
        }

        ///<summary>
        ///要显示的列名
        ///</summary>
        public string HearName
        {
            get { return (string)GetValue(HearNameProperty); }
            set { SetValue(HearNameProperty, value); }
        }

        public static readonly DependencyProperty HearNameProperty = DependencyProperty.Register("HearName", typeof(string), typeof(OptionList), new FrameworkPropertyMetadata(""));

        /// <summary>
        /// 选择框生改变时
        /// </summary>
        private void UpdateSelectAllState()
        {

            if (OptionOrdtS.All(x => x.Host))
                IsCheck = true;
            else if (OptionOrdtS.All(x => !x.Host))
                IsCheck = false;
            else
                IsCheck = null;
        }

        public void SetValuesToCheck(IEnumerable<OptionOrdt> values)
        {
            if (OptionOrdtS != null)
                OptionOrdtS.Clear();
            else
                OptionOrdtS = new ObservableCollection<OptionOrdt>();

            foreach (var item in values)
            {
                bool IsHost = true;
                if (item.Name == null)
                    item.Name = "";
                if (!itemsDic.TryGetValue(item.Name, out IsHost))
                    IsHost = true;
                OptionOrdtS.Add(new OptionOrdt() { Host = IsHost, Name = item.Name });
            }
            UpdateSelectAllState();
            RefreshViewSource();
        }
        /// <summary>
        /// 获取数据源的方法
        /// </summary>
        /// <returns></returns>
        private void SaveitemsDic(IEnumerable<OptionOrdt> values)
        {
            itemsDic.Clear();
            foreach (var item in values)
            {
                itemsDic[item.Name] = item.Host;
            }

        }

        public void ResetOptionList()
        {
            for (int i = 0; i < OptionOrdtS.Count; i++)
            {
                if (IsCheck.HasValue)
                    OptionOrdtS[i].Host = true;
            }
        }

        private void OptionList_IsCheckChanged()
        {
            for (int i = 0; i < OptionOrdtS.Count; i++)
            {
                if (IsCheck.HasValue)
                    OptionOrdtS[i].Host = IsCheck.Value;
            }
            RefreshViewSource();
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public ObservableCollection<OptionOrdt> OptionOrdtS
        {
            get { return (ObservableCollection<OptionOrdt>)GetValue(OptionOrdtSProperty); }
            set { SetValue(OptionOrdtSProperty, value); }
        }
        public static readonly DependencyProperty OptionOrdtSProperty = DependencyProperty.Register("OptionOrdtS", typeof(ObservableCollection<OptionOrdt>), typeof(OptionList), new FrameworkPropertyMetadata(null));



        /// <summary>
        /// 确认
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    try
                    {
                        SaveAction();
                        if (SaveEvent != null)
                            SaveEvent(this, EventArgs.Empty);

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });
            }
        }

        public void SaveAction()
        {
            if (OptionOrdtS != null)
            {
                SaveitemsDic(OptionOrdtS);
            }
        }

        public ICommand IsListCheckCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    try
                    {
                        UpdateSelectAllState();

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });
            }
        }

        public IEnumerable<string> GetCheckedValues()
        {

            if (OptionOrdtS != null)
            {
                foreach (var item in OptionOrdtS)
                {

                    if (item != null && item.Host)
                        yield return item.Name;
                }
            }
        }

        private void RefreshViewSource()
        {
            var view = CollectionViewSource.GetDefaultView(OptionOrdtS);
            if (view != null)
                view.Refresh();
        }
    }
}
