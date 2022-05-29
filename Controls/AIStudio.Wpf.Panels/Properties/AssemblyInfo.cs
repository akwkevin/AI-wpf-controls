using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("https://gitee.com/akwkevin/AI-wpf-controls/tree/master/AIStudio.Wpf.Panels", "AIStudio.Wpf.Panels")]
[assembly: XmlnsDefinition("https://gitee.com/akwkevin/AI-wpf-controls/tree/master/AIStudio.Wpf.Panels", "AIStudio.Wpf.Panels.Converters")]
[assembly: XmlnsDefinition("https://gitee.com/akwkevin/AI-wpf-controls/tree/master/AIStudio.Wpf.Panels", "AIStudio.Wpf.Panels.Controls")]
[assembly: XmlnsDefinition("https://gitee.com/akwkevin/AI-wpf-controls/tree/master/AIStudio.Wpf.Panels", "AIStudio.Wpf.Panels.Helpers")]

[assembly: XmlnsPrefix("https://gitee.com/akwkevin/AI-wpf-controls/tree/master/AIStudio.Wpf.Panels", "ap")]

