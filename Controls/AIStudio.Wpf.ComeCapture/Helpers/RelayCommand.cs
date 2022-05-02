/* 
 * author      : singba singba@163.com 
 * version     : 20161221
 * source      : AF.Wpf
 * license     : free use or modify
 * description : WPF的按钮命令的基类，本程序根据微软WPF例子中的RelayCommmand做了些扩展封装
 */
using System;
using System.Windows.Input;

namespace AIStudio.Wpf.ComeCapture.Helpers
{
    /// <summary>
    /// 界面命令类
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        private Func<object, bool> _canExecute;
        private Action<object> _execute;
        private bool _IsExecuting = false;
        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            ResetActions(execute, canExecute);
        }

        #endregion // Constructors

        //特殊情况下方法需要重设

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : ((!_IsExecuting) && _canExecute(parameter));
        }

        public event EventHandler CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            try
            {
                if (_IsExecuting)
                    return;
                _IsExecuting = true;
                RaiseCanExecuteChanged();
                _execute(parameter);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _IsExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        #endregion

        public void ResetActions(Action<object> execute, Func<object, bool> canExecute)
        {
            if (execute != null)
            {
                _execute = execute;
            }
            if (canExecute != null)
            {
                _canExecute = canExecute;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}