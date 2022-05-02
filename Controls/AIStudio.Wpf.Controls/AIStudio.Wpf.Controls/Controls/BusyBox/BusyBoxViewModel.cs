using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace AIStudio.Wpf.Controls
{
    public class BusyBoxViewModel : INotifyPropertyChanged, IBusyBox
    {

        private string waitInfo = "系统加载中，请耐心等待";

        public string WaitInfo
        {
            get
            {
                return waitInfo;
            }
            set
            {
                waitInfo = value;
                OnPropertyChanged("WaitInfo");
            }
        }

        private double percent;

        public double Percent
        {
            get
            {
                return percent;
            }
            set
            {
                percent = value;
                OnPropertyChanged("Percent");
            }
        }

        public Action CancelAction
        {
            get; set;
        }

        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = (propertyExpression.Body as MemberExpression).Member.Name;
            this.OnPropertyChanged(propertyName);
        }

        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
