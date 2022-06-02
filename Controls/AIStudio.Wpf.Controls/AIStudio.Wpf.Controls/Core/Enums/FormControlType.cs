using System.ComponentModel;

namespace AIStudio.Wpf.Controls
{
    public enum FormControlType
    {
        [Description("无")]
        None,
        [Description("文本框")]
        TextBox,
        [Description("下拉框")]
        ComboBox,
        [Description("密码框")]
        PasswordBox,
        [Description("日期框")]
        DatePicker,
        [Description("树选框")]
        TreeSelect,
        [Description("多选框")]
        MultiComboBox,
        [Description("树多选框")]
        MultiTreeSelect,
        [Description("勾选框")]
        CheckBox,
        [Description("开关框")]
        ToggleButton,
        [Description("富文本框")]
        RichTextBox, 
        [Description("文件上传")]
        UploadFile,
        [Description("图片上传")]
        UploadImage,
        [Description("表格")]
        DataGrid,

        [Description("Int数值")]
        IntegerUpDown = 100,
        [Description("Long数值")]
        LongUpDown,
        [Description("Double数值")]
        DoubleUpDown,
        [Description("Decimal数值")]
        DecimalUpDown,
        [Description("DateTime数值")]
        DateTimeUpDown,

        [Description("查询按钮")]
        Query = 200,
        [Description("提交按钮")]
        Submit,
        [Description("新增按钮")]
        Add,
        [Description("删除按钮")]
        Delete,
    }
}
