<section id="nice" data-tool="mdnice编辑器" data-website="https://www.mdnice.com" style="font-size: 16px; color: black; padding: 0 10px; line-height: 1.6; word-spacing: 0px; letter-spacing: 0px; word-break: break-word; word-wrap: break-word; text-align: left; font-family: Optima-Regular, Optima, PingFangSC-light, PingFangTC-light, 'PingFang SC', Cambria, Cochin, Georgia, Times, 'Times New Roman', serif;"><p data-tool="mdnice编辑器" style="font-size: 16px; padding-top: 8px; padding-bottom: 8px; margin: 0; line-height: 26px; color: black;">一个Wpf控件库</p>
<p data-tool="mdnice编辑器" style="font-size: 16px; padding-top: 8px; padding-bottom: 8px; margin: 0; line-height: 26px; color: black;">本控件库，结合了MahApps.Metro，Material-Design，HandyControl，PanuonUI，Xceed等控件库，做了一个集成，并有部分自定义的控件，供大家参考使用，以下为控件库截图，本控件库会保持定期维护定期更新，欢迎大家光临。(如果我使用了您的控件，没有在本文提出，请联系我，我给您加上)</p>

![image](https://github.com/akwkevin/AI-wpf-controls/assets/27945492/5fddb88c-ddd5-4193-b1f7-6126eda276ec)

### AIStudio框架汇总：[https://gitee.com/akwkevin/aistudio.-introduce](https://gitee.com/akwkevin/aistudio.-introduce)
### github 地址：https://github.com/akwkevin/AI-wpf-controls
### gitee 地址：https://gitee.com/akwkevin/AI-wpf-controls
    
**快速使用方法** 

# 1.新建WPF应用程序：TestAIControls

# 2.使用nuget安装AIStudio.Wpf.Controls控件库

![image](https://user-images.githubusercontent.com/27945492/174433289-8e9b2891-b8f6-49c9-a632-3e402bac58a7.png)

# 3.App.xaml 引用资源


```
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!--颜色资源-->
                <ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/Colors.xaml"/>

                <ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MahApps.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
```
    
其中```<ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MahApps.xaml"/>```可以替换成```<ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MahApps.Defaults.xaml"/>```，标准控件的Style也默认使用本控件库的。
    
# 4.MainWindow.xaml界面使用控件,引入命名控件
    
xmlns:ac="https://gitee.com/akwkevin/AI-wpf-controls"  
    
使用样式： Style="{StaticResource AIStudio.Styles.XXX}",其中XXX为控件名；
    
使用内置控件： <ac:YYY></ac:YYY>,其中YYY为自定义控件名；


```
<Window x:Class="TestAIControls.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:TestAIControls"
        xmlns:ac="https://gitee.com/akwkevin/AI-wpf-controls"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <WrapPanel>
        <ac:Avatar Size="Small" Shape="Square" Margin="2">AI</ac:Avatar>
        <ac:Badged Margin="2" Badge="1" IsWaving="True">
            <Rectangle Width="24" Height="24" Fill="{StaticResource MahApps.Brushes.Gray8}"></Rectangle>
        </ac:Badged>
        <Button Content="Default" Margin="2" Style="{StaticResource AIStudio.Styles.Button}"/>
        <Button Content="Outlined" Margin="2" Style="{StaticResource AIStudio.Styles.Button.Outlined}"/>
        <Button Content="Flat" Margin="2" Style="{StaticResource AIStudio.Styles.Button.Flat}"/>
        <Button Content="Paper" Margin="2" Style="{StaticResource AIStudio.Styles.Button.Paper}"/>
        <Button Content="Progress" Margin="2" ac:ButtonAttach.Value="50" Style="{StaticResource AIStudio.Styles.Button.Progress}"/>
        <CheckBox Margin="2" Content="CheckBox" IsChecked="True" Style="{StaticResource AIStudio.Styles.CheckBox}"/>
        <CheckBox Margin="2" Content="CheckBoxNull" IsChecked="{x:Null}" Style="{StaticResource AIStudio.Styles.CheckBox}"/>
        <CheckBox Margin="2" Content="RightCheckBox" IsChecked="True" ac:IconAttach.Dock="Right" Style="{StaticResource AIStudio.Styles.CheckBox}"/>
        <CheckBox Margin="2" Content="Plain" IsChecked="True" Style="{StaticResource AIStudio.Styles.CheckBox.Plain}"/>
        <ac:ColorPicker Width="180" Margin="2"/>
        <ComboBox Width="180" Margin="2" ac:ControlAttach.Watermark="请选择" Style="{StaticResource AIStudio.Styles.ComboBox}">
            <ComboBoxItem Content="子项1"/>
            <ComboBoxItem Content="子项2"/>
        </ComboBox>
        <DatePicker Margin="2" Style="{StaticResource AIStudio.Styles.DatePicker}" MinWidth="120" ac:ControlAttach.Watermark="请选择日期"/>
        <ac:DropDown Margin="2" Content="Down">
            <ac:DropDown.Child>
                <TextBlock Margin="10" Text="下拉列表" VerticalAlignment="Center"/>
            </ac:DropDown.Child>
        </ac:DropDown>
        <ac:LinkTextBlock Margin="2" Content="Link" ViewInBrower="True" Url="https://gitee.com/akwkevin" />
        <ac:Loading Margin="2" IsRunning="True"></ac:Loading>
        <ProgressBar Margin="10" Width="100" Value="50" Style="{StaticResource AIStudio.Styles.ProgressBar}"/>
        <ProgressBar Margin="10" Width="100" Value="70" IsIndeterminate="True" Style="{StaticResource AIStudio.Styles.ProgressBar}"/>
        <ProgressBar Margin="10" Width="100" Height="22" Value="90" ac:ControlAttach.CornerRadius="10" Style="{StaticResource AIStudio.Styles.ProgressBar.Percent}"/>
        <ProgressBar Margin="10" Width="100" Value="50" Style="{StaticResource AIStudio.Styles.ProgressBar}"/>
        <ProgressBar Margin="10" Width="100" Value="70" IsIndeterminate="True" Style="{StaticResource AIStudio.Styles.ProgressBar}"/>
        <ProgressBar Margin="10" Width="100" Height="22" Value="90" ac:ControlAttach.CornerRadius="10" Style="{StaticResource AIStudio.Styles.ProgressBar.Percent}"/>
        <TextBox MinWidth="120" Margin="2" ac:ControlAttach.Watermark="请输入内容" Style="{StaticResource AIStudio.Styles.TextBox}" ></TextBox>
        <TextBox MinWidth="120" Margin="2" ac:ControlAttach.Watermark="请输入内容" ac:ShadowAttach.DropShadowEffect="{x:Null}" Style="{StaticResource AIStudio.Styles.TextBox}" ></TextBox>
        <PasswordBox Margin="2" MinWidth="120" ac:ControlAttach.Watermark="请输入密码" Style="{StaticResource AIStudio.Styles.PasswordBox}"/>
        <PasswordBox Margin="2" MinWidth="120" ac:ControlAttach.Watermark="请输入密码" Style="{StaticResource AIStudio.Styles.PasswordBox.Plus}"/>
        <TextBox MinWidth="120" Margin="2" ac:ControlAttach.Watermark="请输入内容" Style="{StaticResource AIStudio.Styles.TextBox}" ></TextBox>
        <TextBox MinWidth="120" Margin="2" ac:ControlAttach.Watermark="请输入内容" ac:ShadowAttach.DropShadowEffect="{x:Null}" Style="{StaticResource AIStudio.Styles.TextBox}" ></TextBox>
        <PasswordBox Margin="2" MinWidth="120" ac:ControlAttach.Watermark="请输入密码" Style="{StaticResource AIStudio.Styles.PasswordBox}"/>
        <PasswordBox Margin="2" MinWidth="120" ac:ControlAttach.Watermark="请输入密码" Style="{StaticResource AIStudio.Styles.PasswordBox.Plus}"/>
    </WrapPanel>
</Window>
```

以下为控件库的其它样式或者配套用法：
# 5：如果使用MaterialDesign风格：App.xaml的引用资源替换成如下：

```
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!--颜色资源-->
            <ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MaterialDesignColor.xaml"/>

            <ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MaterialDesign.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```
其中```<ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MaterialDesign.xaml"/>```可以替换成```<ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MaterialDesign.Defaults.xaml"/>```，标准控件的Style也默认使用本控件库的。

# 6：如果与MahApps.Metro一起使用(需要安装MahApps.Metro),App.xaml的引用资源替换成如下：
           
```
<Application.Resources>
   <ResourceDictionary>
     <ResourceDictionary.MergedDictionaries>
        <!-- MahApps -->
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml" />
        
        <ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.GridControls;component/Themes/MahApps.xaml"/>
     </ResourceDictionary.MergedDictionaries>
   </ResourceDictionary>
</Application.Resources>
```

# 7：如果与MaterialDesign一起使用(需要安装MaterialDesign),App.xaml的引用资源替换成如下：
```
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>             
            <materialDesign:BundledTheme BaseTheme="Inherit" PrimaryColor="DeepPurple" SecondaryColor="Lime" ColorAdjustment="{materialDesign:ColorAdjustment}" />
            <!--Material Design-->
            <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml" />
            <ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MaterialDesign.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```
# 8 控件截图

![输入图片说明](button.png)

![输入图片说明](textbox.png)

![输入图片说明](dropdown.png)

![输入图片说明](menu.png)

![输入图片说明](pagination.png)

![输入图片说明](stepbar.png)

![输入图片说明](checkbox.png)

![输入图片说明](datetimepicker.png)

![输入图片说明](form.png)

![输入图片说明](combobox.png)

![输入图片说明](radio.png)

![输入图片说明](rate.png)

![输入图片说明](repeat.png)

![输入图片说明](slider.png)

![输入图片说明](toggle.png)

![输入图片说明](updown.png)

![输入图片说明](upload.png)

![输入图片说明](avatar.png)

![输入图片说明](badge.png)

![输入图片说明](calendar.png)

![输入图片说明](card.png)

![输入图片说明](datagrid.png)

![输入图片说明](treedatagrid.png)

![输入图片说明](expander.png)

![输入图片说明](group.png)

![输入图片说明](image2.png)

![输入图片说明](tag.png)

![输入图片说明](tabs.png)

![输入图片说明](timeline.png)

![输入图片说明](tree.png)

![输入图片说明](tree2.png)

![输入图片说明](notice.png)

![输入图片说明](messge.png)

![输入图片说明](diaglog.png)

![输入图片说明](childwindow.png)

![输入图片说明](progress.png)

![输入图片说明](loading.png)

![输入图片说明](anchor.png)

![输入图片说明](divder.png)

![输入图片说明](colorpicker.png)

![输入图片说明](verify.png)

![输入图片说明](icon.png)

![输入图片说明](path.png)

**友情提示**
很多朋友老问为什么编译不过去，您需要安装对应的net版本，或者修改工程的net版本，如下图。
![image](https://user-images.githubusercontent.com/27945492/174464893-948521f0-36f6-4a15-84de-759043751d64.png)
    
    
技术交流
个人QQ:80267720 
QQ技术交流群1:51286643(已满)，QQ技术交流群2:51280907（如果您还喜欢，帮忙点个星，谢谢） 
个人博客:https://www.cnblogs.com/akwkevin/

更多
界面截图请到博客介绍：https://www.cnblogs.com/akwkevin/p/16297568.html
