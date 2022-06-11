using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls.Behaviors
{
    public class CommandBehaviorBase<T>
      where T : FrameworkElement
    {
        private ICommand command;
        private readonly WeakReference targetObject;
        private object[] objs;

        private List<object> Parameters = new List<object>();

        /// <summary>
        /// Constructor specifying the target object.
        /// </summary>
        /// <param name="targetObject">The target object the behavior is attached to.</param>
        public CommandBehaviorBase(T targetObject)
        {
            this.targetObject = new WeakReference(targetObject);
            for (int i = 0; i < 4; i++)
            {
                Parameters.Add(null);
            }
        }

        /// <summary>
        /// Corresponding command to be execute and monitored for <see cref="ICommand.CanExecuteChanged"/>
        /// </summary>
        public ICommand Command
        {
            get
            {
                return command;
            }
            set
            {
                if (this.command != null)
                {
                    this.command.CanExecuteChanged -= this.CommandCanExecuteChanged;
                }

                this.command = value;

                if (this.command != null)
                {
                    this.command.CanExecuteChanged += this.CommandCanExecuteChanged;
                    UpdateEnabledState();
                }
            }
        }

        #region 参数设置

        /// <summary>
        /// The parameter to supply the command during execution
        /// </summary>
        public object CommandParameter
        {
            get
            {
                if (this.Parameters != null && this.Parameters.Count > 0)
                {
                    return this.Parameters[0];
                }
                return null;
            }
            set
            {
                if (this.Parameters != null && this.Parameters.Count > 0 && this.Parameters[0] != value)
                {
                    this.Parameters[0] = value;
                    this.UpdateEnabledState();
                }
            }
        }

        /// <summary>
        /// The parameter to supply the command during execution
        /// </summary>
        public object CommandParameter1
        {
            get
            {
                if (this.Parameters != null && this.Parameters.Count > 1)
                {
                    return this.Parameters[1];
                }
                return null;
            }
            set
            {
                if (this.Parameters != null && this.Parameters.Count > 1 && this.Parameters[1] != value)
                {
                    this.Parameters[1] = value;
                    this.UpdateEnabledState();
                }
            }
        }


        /// <summary>
        /// The parameter to supply the command during execution
        /// </summary>
        public object CommandParameter2
        {
            get
            {
                if (this.Parameters != null && this.Parameters.Count > 2)
                {
                    return this.Parameters[2];
                }
                return null;
            }
            set
            {
                if (this.Parameters != null && this.Parameters.Count > 2 && this.Parameters[2] != value)
                {
                    this.Parameters[2] = value;
                    this.UpdateEnabledState();
                }
            }
        }


        /// <summary>
        /// The parameter to supply the command during execution
        /// </summary>
        public object CommandParameter3
        {
            get
            {
                if (this.Parameters != null && this.Parameters.Count > 3)
                {
                    return this.Parameters[3];
                }
                return null;
            }
            set
            {
                if (this.Parameters != null && this.Parameters.Count > 3 && this.Parameters[3] != value)
                {
                    this.Parameters[3] = value;
                    this.UpdateEnabledState();
                }
            }
        }

        #endregion


        /// <summary>
        /// Object to which this behavior is attached.
        /// </summary>
        protected T TargetObject
        {
            get
            {
                return targetObject.Target as T;
            }
        }


        /// <summary>
        /// Updates the target object's IsEnabled property based on the commands ability to execute.
        /// </summary>
        protected virtual void UpdateEnabledState()
        {
            if (TargetObject == null)
            {
                this.Command = null;
                if (this.Parameters != null)
                {
                    for (int i = 0; i < this.Parameters.Count; i++)
                    {
                        this.Parameters[i] = null;
                    }
                }
            }
            else if (this.Command != null)
            {
                if (this.Parameters != null)
                {
                    bool isEnable = this.Command.CanExecute(this.Parameters.ToArray());
                    if (isEnable == false)
                    {
                        List<object> lists = new List<object>() { null, null };
                        lists.AddRange(this.Parameters);
                        isEnable = this.Command.CanExecute(lists.ToArray());
                    }
                    TargetObject.IsEnabled = isEnable;
                }
            }
        }

        /// <summary>
        /// 需要添加的默认参数
        /// </summary>
        /// <param name="defaultparameters"></param>
        protected void SetDefaultParameter(object[] defaultparameters)
        {
            this.objs = defaultparameters;
        }

        private void AddDefaultParameter()
        {
            if (this.objs != null)
            {
                if (this.Parameters == null) this.Parameters = new List<object>();
                for (int i = 0; i < objs.Length; i++)
                {
                    this.Parameters.Insert(i, objs[i]);
                }
            }
        }

        private void RemoveDefaultParameter()
        {
            if (this.objs != null)
            {
                if (this.Parameters == null) this.Parameters = new List<object>();
                for (int i = 0; i < objs.Length; i++)
                {
                    if (this.Parameters.Count > 0)
                        this.Parameters.RemoveAt(0);
                }
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }

        /// <summary>
        /// Executes the command, if it's set, providing the <see cref="CommandParameter"/>
        /// </summary>
        protected virtual void ExecuteCommand()
        {
            if (this.Command != null)
            {
                AddDefaultParameter(); //加上默认参数再试一下
                if (this.Command.CanExecute(this.Parameters.ToArray()) == true)
                {
                    this.Command.Execute(this.Parameters.ToArray());
                    RemoveDefaultParameter();
                }
                else
                {
                    RemoveDefaultParameter();
                    if (this.Command.CanExecute(this.Parameters.ToArray()) == true)
                    {
                        this.Command.Execute(this.Parameters.ToArray());
                    }
                }
            }
        }
    }
}
