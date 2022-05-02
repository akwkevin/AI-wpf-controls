/* 
 * author      : singba singba@163.com 
 * version     : 20161221
 * source      : AF.Core
 * license     : free use or modify
 * description : 通用类型基类
 */
using System;
using System.Linq.Expressions;

namespace AIStudio.Wpf.ComeCapture.Models
{
    /// <summary>
    /// singba:20120807
    /// 这个基类只作为INotifyPropertyChanged的实现，不再是Entity的基类
    /// </summary>
    public class EntityBase : System.ComponentModel.INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }

        protected virtual void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;
            RaisePropertyChanged(memberExpression.Member.Name);
        }
        #endregion
    }
}
