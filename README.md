<section id="nice" data-tool="mdnice编辑器" data-website="https://www.mdnice.com" style="font-size: 16px; color: black; padding: 0 10px; line-height: 1.6; word-spacing: 0px; letter-spacing: 0px; word-break: break-word; word-wrap: break-word; text-align: left; font-family: Optima-Regular, Optima, PingFangSC-light, PingFangTC-light, 'PingFang SC', Cambria, Cochin, Georgia, Times, 'Times New Roman', serif;"><p data-tool="mdnice编辑器" style="font-size: 16px; padding-top: 8px; padding-bottom: 8px; margin: 0; line-height: 26px; color: black;">一个Wpf控件库</p>
<p data-tool="mdnice编辑器" style="font-size: 16px; padding-top: 8px; padding-bottom: 8px; margin: 0; line-height: 26px; color: black;">本控件库，结合了MahApps.Metro，Material-Design，HandyControl，PanuonUI，Xceed等控件库，做了一个集成，并有部分自定义的控件，供大家参考使用，以下为控件库截图，本控件库会保持定期维护定期更新，欢迎大家光临。(如果我使用了您的控件，没有在本文提出，请联系我，我给您加上)</p>

![image](https://user-images.githubusercontent.com/27945492/174432563-a997977f-e4f0-4749-920b-2a5aa4d08a29.png)

**快速使用方法** 

1.新建WPF应用程序：TestAIControls

2.使用nuget安装AIStudio.Wpf.Controls控件库

![image](https://user-images.githubusercontent.com/27945492/174433289-8e9b2891-b8f6-49c9-a632-3e402bac58a7.png)

3.App.xaml 引用资源


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

4.MainWindow.xaml界面使用控件


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
            <Rectangle Width="24" Height="24" Fill="{DynamicResource MahApps.Brushes.Gray8}"></Rectangle>
        </ac:Badged>
        <Button Content="Default" Margin="2" Style="{DynamicResource AIStudio.Styles.Button}"/>
        <Button Content="Outlined" Margin="2" Style="{DynamicResource AIStudio.Styles.Button.Outlined}"/>
        <Button Content="Flat" Margin="2" Style="{DynamicResource AIStudio.Styles.Button.Flat}"/>
        <Button Content="Paper" Margin="2" Style="{DynamicResource AIStudio.Styles.Button.Paper}"/>
        <Button Content="Progress" Margin="2" ac:ButtonAttach.Value="50" Style="{DynamicResource AIStudio.Styles.Button.Progress}"/>
        <CheckBox Margin="2" Content="CheckBox" IsChecked="True" Style="{DynamicResource AIStudio.Styles.CheckBox}"/>
        <CheckBox Margin="2" Content="CheckBoxNull" IsChecked="{x:Null}" Style="{DynamicResource AIStudio.Styles.CheckBox}"/>
        <CheckBox Margin="2" Content="RightCheckBox" IsChecked="True" ac:IconAttach.Dock="Right" Style="{DynamicResource AIStudio.Styles.CheckBox}"/>
        <CheckBox Margin="2" Content="Plain" IsChecked="True" Style="{DynamicResource AIStudio.Styles.CheckBox.Plain}"/>
        <ac:ColorPicker Width="180" Margin="2"/>
        <ComboBox Width="180" Margin="2" ac:ControlAttach.Watermark="请选择" Style="{DynamicResource AIStudio.Styles.ComboBox}">
            <ComboBoxItem Content="子项1"/>
            <ComboBoxItem Content="子项2"/>
        </ComboBox>
        <DatePicker Margin="2" Style="{DynamicResource AIStudio.Styles.DatePicker}" MinWidth="120" ac:ControlAttach.Watermark="请选择日期"/>
        <ac:DropDown Margin="2" Content="Down">
            <ac:DropDown.Child>
                <TextBlock Margin="10" Text="下拉列表" VerticalAlignment="Center"/>
            </ac:DropDown.Child>
        </ac:DropDown>
        <ac:LinkTextBlock Margin="2" Content="Link" ViewInBrower="True" Url="https://gitee.com/akwkevin" />
        <ac:Loading Margin="2" IsRunning="True"></ac:Loading>
        <ProgressBar Margin="10" Width="100" Value="50" Style="{DynamicResource AIStudio.Styles.ProgressBar}"/>
        <ProgressBar Margin="10" Width="100" Value="70" IsIndeterminate="True" Style="{DynamicResource AIStudio.Styles.ProgressBar}"/>
        <ProgressBar Margin="10" Width="100" Height="22" Value="90" ac:ControlAttach.CornerRadius="10" Style="{DynamicResource AIStudio.Styles.ProgressBar.Percent}"/>
        <ProgressBar Margin="10" Width="100" Value="50" Style="{DynamicResource AIStudio.Styles.ProgressBar}"/>
        <ProgressBar Margin="10" Width="100" Value="70" IsIndeterminate="True" Style="{DynamicResource AIStudio.Styles.ProgressBar}"/>
        <ProgressBar Margin="10" Width="100" Height="22" Value="90" ac:ControlAttach.CornerRadius="10" Style="{DynamicResource AIStudio.Styles.ProgressBar.Percent}"/>
        <TextBox MinWidth="120" Margin="2" ac:ControlAttach.Watermark="请输入内容" Style="{DynamicResource AIStudio.Styles.TextBox}" ></TextBox>
        <TextBox MinWidth="120" Margin="2" ac:ControlAttach.Watermark="请输入内容" ac:ShadowAttach.DropShadowEffect="{x:Null}" Style="{DynamicResource AIStudio.Styles.TextBox}" ></TextBox>
        <PasswordBox Margin="2" MinWidth="120" ac:ControlAttach.Watermark="请输入密码" Style="{DynamicResource AIStudio.Styles.PasswordBox}"/>
        <PasswordBox Margin="2" MinWidth="120" ac:ControlAttach.Watermark="请输入密码" Style="{DynamicResource AIStudio.Styles.PasswordBox.Plus}"/>
        <TextBox MinWidth="120" Margin="2" ac:ControlAttach.Watermark="请输入内容" Style="{DynamicResource AIStudio.Styles.TextBox}" ></TextBox>
        <TextBox MinWidth="120" Margin="2" ac:ControlAttach.Watermark="请输入内容" ac:ShadowAttach.DropShadowEffect="{x:Null}" Style="{DynamicResource AIStudio.Styles.TextBox}" ></TextBox>
        <PasswordBox Margin="2" MinWidth="120" ac:ControlAttach.Watermark="请输入密码" Style="{DynamicResource AIStudio.Styles.PasswordBox}"/>
        <PasswordBox Margin="2" MinWidth="120" ac:ControlAttach.Watermark="请输入密码" Style="{DynamicResource AIStudio.Styles.PasswordBox.Plus}"/>
    </WrapPanel>
</Window>
```

5：如果使用MaterialDesign风格：App.xaml的引用资源替换成如下：

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

6：如果与MahApps.Metro一起使用(需要安装MahApps.Metro),App.xaml的引用资源替换成如下：
           
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

7：如果与MaterialDesign一起使用(需要安装MaterialDesign),App.xaml的引用资源替换成如下：
```
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>             
            <materialDesign:MahAppsBundledTheme BaseTheme="Inherit" PrimaryColor="DeepPurple" SecondaryColor="LightBlue"/>               
             <!--Material Design--> 
            <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml" />
            <ResourceDictionary Source="pack://application:,,,/AIStudio.Wpf.Controls;component/Themes/MaterialDesign.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```
    
**友情提示**
很多朋友老问为什么编译不过去，您需要安装对应的net版本，或者修改工程的net版本，如下图。
![image](https://user-images.githubusercontent.com/27945492/174464893-948521f0-36f6-4a15-84de-759043751d64.png)
    
    
技术交流
个人QQ:80267720 QQ技术交流群:51286643 个人博客:https://www.cnblogs.com/akwkevin/

更多
界面截图请到博客介绍：https://www.cnblogs.com/akwkevin/p/16297568.html
