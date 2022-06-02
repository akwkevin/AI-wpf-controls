using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Demo.Validation;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    public partial class FormViewModel : BindableBase
    {
        private Base_User _base_User = new Base_User();
        public Base_User Base_User
        {
            get
            {
                return _base_User;
            }
            set
            {
                SetProperty(ref _base_User, value);
            }
        }

        private Base_User _base_User2 = new Base_User();
        public Base_User Base_User2
        {
            get
            {
                return _base_User2;
            }
            set
            {
                SetProperty(ref _base_User2, value);
            }
        }

        private FormSetting _formSetting = new FormSetting();
        public FormSetting FormSetting
        {
            get
            {
                return _formSetting;
            }
            set
            {
                SetProperty(ref _formSetting, value);
            }
        }

        private List<SelectOption> _sexList;
        public List<SelectOption> SexList
        {
            get
            {
                return _sexList;
            }
            set
            {
                SetProperty(ref _sexList, value);
            }
        }

        private List<SelectOption> _rolesList;
        public List<SelectOption> RolesList
        {
            get
            {
                return _rolesList;
            }
            set
            {
                SetProperty(ref _rolesList, value);
            }
        }

        private List<SelectOption> _duties;
        public List<SelectOption> Duties
        {
            get
            {
                return _duties;
            }
            set
            {
                SetProperty(ref _duties, value);
            }
        }

        private List<TreeModel> _departments;
        public List<TreeModel> Departments
        {
            get
            {
                return _departments;
            }
            set
            {
                SetProperty(ref _departments, value);
            }
        }

        private ICommand _submitCommand;
        public ICommand SubmitCommand
        {
            get
            {
                return this._submitCommand ?? (this._submitCommand = new DelegateCommand<object>(para => this.Submit(para)));
            }
        }

        public void Submit(object para)
        {
            string message = string.Empty;
            Type type = para.GetType();
            if (type.GetProperty("Error") != null)
            {
                message = type.GetProperty("Error").GetValue(para)?.ToString();
            }
            
            if (string.IsNullOrEmpty(message))
            {
                foreach (var prop in type.GetProperties())
                {
                    if (prop.CanRead && prop.CanWrite)
                    {              
                        var ignoreAttr = prop.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), true);
                        if (ignoreAttr.Length > 0)
                        {
                            continue;
                        }

                        var displayAttr = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                        string name = string.Empty;
                        if (displayAttr.Length > 0)
                        {
                            name = (displayAttr[0] as DisplayNameAttribute).DisplayName;
                        }
                        else
                        {
                            name = prop.Name;
                        }

                        var value = prop.GetValue(para);
                        if (value is IEnumerable<object> list)
                        {
                            value = string.Join(",", list.Select(p => p?.ToString()));
                        }

                        if (value == null || value.ToString() == "")
                        {
                            continue;
                        }
                        message += $"{name}:{value?.ToString()},";
                    }
                }
            }

            MessageBox.Show(Application.Current.MainWindow,  message);
        }

        public FormViewModel()
        {
            SexList = new List<SelectOption>()
            {
                new SelectOption(){Value = "0",Text = "女" },
                new SelectOption(){Value ="1",Text = "男" },
            };
            RolesList = new List<SelectOption>()
            {
                new SelectOption(){Value = "Manager",Text = "管理员" },
                new SelectOption(){Value ="Operator",Text = "操作员" },
                new SelectOption(){Value ="Engineer",Text = "工程师" },
            };
            Duties = new List<SelectOption>()
            {
                new SelectOption(){Value ="Develop",Text = "开发" },
                new SelectOption(){Value ="Operator",Text = "经理" },
                new SelectOption(){Value ="Engineer",Text = "总监" },
            };

            Departments = new List<TreeModel>()
            {
                new TreeModel()
                {
                    Value = "Depart1",
                    Text = "部门1",
                    Children = new List<TreeModel>()
                    {
                        new TreeModel()
                        {
                              Value = "Depart1_1",
                              Text = "部门1_1",
                        },
                        new TreeModel()
                        {
                              Value = "Depart1_2",
                              Text = "部门1_2",
                        },
                    },
                },
                new TreeModel()
                {
                    Value = "Depart2",
                    Text = "部门2",
                    Children = new List<TreeModel>()
                    {
                        new TreeModel()
                        {
                              Value = "Depart2_1",
                              Text = "部门2_1",
                        },
                        new TreeModel()
                        {
                              Value = "Depart2_2",
                              Text = "部门2_2",
                        },
                    },
                },
                new TreeModel()
                {
                    Value = "Depart3",
                    Text = "部门3",
                    Children = new List<TreeModel>()
                    {
                        new TreeModel()
                        {
                              Value = "Depart3_1",
                              Text = "部门3_1",
                        },
                        new TreeModel()
                        {
                              Value = "Depart3_2",
                              Text = "部门3_2",
                        },
                    },
                },
            };
        }
    }

    public partial class Base_User : BindableBase
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                SetProperty(ref _isChecked, value);
            }
        }

        public string Id
        {
            get; set;
        }

        public bool Deleted
        {
            get; set;
        }

        private string _userName;
        [Required(ErrorMessage = "用户名不能为空")]
        [DisplayName("姓名")]
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                SetProperty(ref _userName, value);
            }
        }

        private string _realName;
        [DisplayName("真实姓名")]
        public string RealName
        {
            get
            {
                return _realName;
            }
            set
            {
                SetProperty(ref _realName, value);
            }
        }

        private string _password;
        [Required(ErrorMessage = "密码不能为空")]
        [DisplayName("密码")]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        private int _sex;
        [Required(ErrorMessage = "请选择性别")]
        [DisplayName("性别")]
        public int Sex
        {
            get
            {
                return _sex;
            }
            set
            {
                SetProperty(ref _sex, value);
            }
        }

        private DateTime? _birthday;
        [Required(ErrorMessage = "请选择出生日期")]
        [DisplayName("生日")]
        public DateTime? Birthday
        {
            get
            {
                return _birthday;
            }
            set
            {
                SetProperty(ref _birthday, value);
            }
        }

        private ObservableCollection<string> _roleIdList = new ObservableCollection<string>();
        [NullOrEmptyValidation(ErrorMessage = "请选择角色")]
        [DisplayName("角色")]
        public ObservableCollection<string> RoleIdList
        {
            get
            {
                return _roleIdList;
            }
            set
            {
                SetProperty(ref _roleIdList, value);
            }
        }

        private string _departmentId;
        [NullOrEmptyValidation(ErrorMessage = "请选择部门")]
        [DisplayName("部门")]
        public string DepartmentId
        {
            get
            {
                return _departmentId;
            }
            set
            {
                SetProperty(ref _departmentId, value);
            }
        }

        private string _selectedDuty;
        [Required(ErrorMessage = "请选择职能")]
        [DisplayName("职能")]
        public string SelectedDuty
        {
            get
            {
                return _selectedDuty;
            }
            set
            {
                SetProperty(ref _selectedDuty, value);
            }
        }

        private string _email;
        [EmailValidation(ErrorMessage = "请输入正确的邮箱格式,例：zhangsan@126.com")]
        [DisplayName("邮箱")]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                SetProperty(ref _email, value);
            }
        }

        private string _phoneNumber;
        [PhoneValidation]
        [DisplayName("手机号码")]
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                SetProperty(ref _phoneNumber, value);
            }
        }

        [DisplayName("创建时间")]
        public DateTime CreateTime
        {
            get; set;
        }

        [DisplayName("修改时间")]
        public DateTime? ModifyTime
        {
            get; set;
        }

        public string CreatorId
        {
            get; set;
        }

        [DisplayName("创建者")]
        public string CreatorName
        {
            get; set;
        }

        public string ModifyId
        {
            get; set;
        }

        [DisplayName("修改者")]
        public string ModifyName
        {
            get; set;
        }

        public Base_User()
        {
            RoleIdList.CollectionChanged += RoleIdList_CollectionChanged;
        }

        private void RoleIdList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("RoleIdList");
        }
    }

    public partial class Base_User : IDataErrorInfo
    {
        public string this[string columnName]
        {
            get
            {
                List<ValidationResult> validationResults = new List<ValidationResult>();

                bool result = Validator.TryValidateProperty(
                    GetType().GetProperty(columnName).GetValue(this),
                    new ValidationContext(this)
                    {
                        MemberName = columnName
                    },
                    validationResults);

                if (result)
                    return null;

                return validationResults.First().ErrorMessage;
            }
        }

        public string Error
        {
            get
            {
                ICollection<ValidationResult> results;
                this.Validate(out results);

                return results.FirstOrDefault()?.ErrorMessage;
            }
        }
    }

    public class FormSetting : BindableBase
    {
        private System.Windows.Controls.Orientation _orientation;
        public System.Windows.Controls.Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                SetProperty(ref _orientation, value);
            }
        }

        private FormPanelType _panelType;
        public FormPanelType PanelType
        {
            get
            {
                return _panelType;
            }
            set
            {
                SetProperty(ref _panelType, value);
            }
        }

        private string _headerWidth = "50";
        public string HeaderWidth
        {
            get
            {
                return _headerWidth;
            }
            set
            {
                SetProperty(ref _headerWidth, value);
            }
        }

        private string _bodyWidth = "*";
        public string BodyWidth
        {
            get
            {
                return _bodyWidth;
            }
            set
            {
                SetProperty(ref _bodyWidth, value);
            }
        }

        private double _horizontalMargin;
        public double HorizontalMargin
        {
            get
            {
                return _horizontalMargin;
            }
            set
            {
                if (SetProperty(ref _horizontalMargin, value))
                {
                    ItemMargin = new System.Windows.Thickness(HorizontalMargin, VerticalMargin, HorizontalMargin, VerticalMargin);
                }
            }
        }

        private double _verticalMargin = 8.0;
        public double VerticalMargin
        {
            get
            {
                return _verticalMargin;
            }
            set
            {
                if (SetProperty(ref _verticalMargin, value))
                {
                    ItemMargin = new System.Windows.Thickness(HorizontalMargin, VerticalMargin, HorizontalMargin, VerticalMargin);
                }
            }
        }

        private System.Windows.Thickness _itemMargin;
        public System.Windows.Thickness ItemMargin
        {
            get
            {
                return _itemMargin;
            }
            set
            {
                SetProperty(ref _itemMargin, value);
            }
        }

        private bool _allowDrop;
        public bool AllowDrop
        {
            get
            {
                return _allowDrop;
            }
            set
            {
                SetProperty(ref _allowDrop, value);
            }
        }
        

        public FormSetting()
        {
            ItemMargin = new System.Windows.Thickness(HorizontalMargin, VerticalMargin, HorizontalMargin, VerticalMargin);
        }
    }

    public class SelectOption
    {
        public string Value
        {
            get; set;
        }
        public string Text
        {
            get; set;
        }

        public override string ToString()
        {
            return $"{Value}-{Text}";
        }
    }

    public class TreeModel : BindableBase
    {
        /// <summary>
        /// 唯一标识Id
        /// </summary>
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// 数据值
        /// </summary>
        public string Value
        {
            get; set;
        }

        /// <summary>
        /// 父Id
        /// </summary>
        public string ParentId
        {
            get; set;
        }

        /// <summary>
        /// 节点深度
        /// </summary>
        public int? Level { get; set; } = 1;

        /// <summary>
        /// 显示的内容
        /// </summary>
        public string Text
        {
            get; set;
        }

        /// <summary>
        /// 孩子节点
        /// </summary>
        public List<TreeModel> Children
        {
            get; set;
        }

        /// <summary>
        /// 孩子节点
        /// </summary>
        public object children
        {
            get; set;
        }


        private bool isExpanded = true;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                SetProperty(ref isExpanded, value);
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                SetProperty(ref isSelected, value);
            }
        }

        public override string ToString()
        {
            return $"{Value}-{Text}";
        }
    }
}