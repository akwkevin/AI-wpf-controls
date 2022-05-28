using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    public class PaletteSelectorViewModel : BindableBase
    {
        private static readonly string sizesCulture = "/AIStudio.Wpf.Controls;component/Themes/Sizes.xaml";
        private static readonly string materialDesignCulture = "/AIStudio.Wpf.Controls;component/Themes/MaterialDesign.xaml";
        private static readonly string mahAppsCulture = "/AIStudio.Wpf.Controls;component/Themes/MahApps.xaml";

        public PaletteSelectorViewModel()
        {
            PrimarySwatches = new SwatchesProvider().Swatches;
            AccentSwatches = new SwatchesProvider().Swatches.Where(p => p.IsAccented).ToList();

            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;
            Init();

            SelectedPrimary = PrimarySwatches.FirstOrDefault(p => p.ExemplarHue.Color == theme.PrimaryMid.Color);
            SelectedAccent = AccentSwatches.FirstOrDefault(p => p.AccentExemplarHue.Color == theme.SecondaryMid.Color);
        }

        public void Init()
        {
            ResourceDictionary sizes = GetControlResourceDictionary().GetResourceDictionary(sizesCulture);

            _isMahApps = GetControlResourceDictionary().Source.OriginalString.Contains(mahAppsCulture);

            _defaultControlHeight = sizes["DefaultControlHeight"].ToString();

            var paddingarrary = sizes["DefaultControlPadding"].ToString().Split(',');
            _defaultControlPadding = string.Join(",", paddingarrary.Select(p => p.PadLeft(2, '0')));

            var cornerRadiusarrary = sizes["DefaultCornerRadius"].ToString().Split(',');
            _defaultCornerRadius = string.Join(",", cornerRadiusarrary.Select(p => p.PadLeft(2, '0')));

            _fontSize = (double)sizes["AIStudio.Font.Size"];

            _fontFamily = sizes["AIStudio.Fonts.Family"] as FontFamily;
        }

        public ICommand ToggleStyleCommand { get; } = new DelegateCommand<bool>(o => ApplyStyle(o));

        public IEnumerable<Swatch> PrimarySwatches
        {
            get;
        }

        public IEnumerable<Swatch> AccentSwatches
        {
            get;
        }

        private Swatch _selectedPrimary;
        public Swatch SelectedPrimary
        {
            get => _selectedPrimary;
            set
            {
                if (SetProperty(ref _selectedPrimary, value))
                {
                    ApplyPrimary(value);
                }
            }
        }

        private Swatch _selectedAccent;
        public Swatch SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                if (SetProperty(ref _selectedAccent, value))
                {
                    ApplyAccent(value);
                }
            }
        }


        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ApplyBase(value);
                }
            }
        }

        private bool _isColorAdjusted;
        public bool IsColorAdjusted
        {
            get => _isColorAdjusted;
            set
            {
                if (SetProperty(ref _isColorAdjusted, value))
                {
                    ModifyTheme(theme => {
                        if (theme is Theme internalTheme)
                        {
                            internalTheme.ColorAdjustment = value
                                ? new ColorAdjustment
                                {
                                    DesiredContrastRatio = DesiredContrastRatio,
                                    Contrast = ContrastValue,
                                    Colors = ColorSelectionValue
                                }
                                : null;
                        }
                    });
                }
            }
        }

        private float _desiredContrastRatio = 4.5f;
        public float DesiredContrastRatio
        {
            get => _desiredContrastRatio;
            set
            {
                if (SetProperty(ref _desiredContrastRatio, value))
                {
                    ModifyTheme(theme => {
                        if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                            internalTheme.ColorAdjustment.DesiredContrastRatio = value;
                    });
                }
            }
        }

        public IEnumerable<Contrast> ContrastValues => Enum.GetValues(typeof(Contrast)).Cast<Contrast>();

        private Contrast _contrastValue;
        public Contrast ContrastValue
        {
            get => _contrastValue;
            set
            {
                if (SetProperty(ref _contrastValue, value))
                {
                    ModifyTheme(theme => {
                        if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                            internalTheme.ColorAdjustment.Contrast = value;
                    });
                }
            }
        }

        public IEnumerable<ColorSelection> ColorSelectionValues => Enum.GetValues(typeof(ColorSelection)).Cast<ColorSelection>();

        private ColorSelection _colorSelectionValue;
        public ColorSelection ColorSelectionValue
        {
            get => _colorSelectionValue;
            set
            {
                if (SetProperty(ref _colorSelectionValue, value))
                {
                    ModifyTheme(theme => {
                        if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                            internalTheme.ColorAdjustment.Colors = value;
                    });
                }
            }
        }

        private bool _isMahApps;
        public bool IsMahApps
        {
            get => _isMahApps;
            set
            {
                if (SetProperty(ref _isMahApps, value))
                {
                    ApplyControl(value);
                }
            }
        }

        private string _defaultControlHeight;
        public string DefaultControlHeight
        {
            get => _defaultControlHeight;
            set
            {
                if (SetProperty(ref _defaultControlHeight, value))
                {
                    ApplyDefaultControlHeight(value);
                }
            }
        }

        private string _defaultControlPadding;
        public string DefaultControlPadding
        {
            get => _defaultControlPadding;
            set
            {
                if (SetProperty(ref _defaultControlPadding, value))
                {
                    ApplyDefaultControlPadding(value);
                }
            }
        }

        private string _defaultCornerRadius;
        public string DefaultCornerRadius
        {
            get => _defaultCornerRadius;
            set
            {
                if (SetProperty(ref _defaultCornerRadius, value))
                {
                    ApplyDefaultCornerRadius(value);
                }
            }
        }

        private double _fontSize;
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (SetProperty(ref _fontSize, value))
                {
                    ApplyFontSize(_fontSize);
                }
            }
        }



        private FontFamily _fontFamily;
        public FontFamily FontFamily
        {
            get
            {
                return _fontFamily;
            }
            set
            {
                if (SetProperty(ref _fontFamily, value))
                {
                    ApplyFontFamily(_fontFamily);
                }
            }
        }


        private static void ApplyStyle(bool alternate)
        {

        }

        private static void ApplyBase(bool isDark)
            => ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));

        private static void ApplyPrimary(Swatch swatch)
            => ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));

        private static void ApplyControl(bool isMahApps)
        {
            string removeCulture = isMahApps ? materialDesignCulture : mahAppsCulture;
            string addCulture = isMahApps ? mahAppsCulture : materialDesignCulture;
            ResourceDictionary removeDictionary = GetControlResourceDictionary();
            ResourceDictionary addDictionary = new ResourceDictionary();
            addDictionary.Source = new Uri(addCulture, UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Remove(removeDictionary);
            Application.Current.Resources.MergedDictionaries.Add(addDictionary);
        }

        private static void ApplyAccent(Swatch swatch)
        {
            if (swatch.AccentExemplarHue is Hue accentHue)
            {
                ModifyTheme(theme => theme.SetSecondaryColor(accentHue.Color));
            }
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }


        private void ApplyDefaultControlHeight(string value)
        {
            ResourceDictionary resourceDictionary = GetControlResourceDictionary().GetResourceDictionary(sizesCulture);

            resourceDictionary["DefaultControlHeight"] = double.Parse(value);
        }

        private void ApplyDefaultControlPadding(string value)
        {
            ResourceDictionary resourceDictionary = GetControlResourceDictionary().GetResourceDictionary(sizesCulture);

            var paddingarray = value.Split(',').Select(p => double.Parse(p)).ToArray();
            resourceDictionary["DefaultControlPadding"] = new Thickness(paddingarray[0], paddingarray[1], paddingarray[2], paddingarray[3]);
        }

        private void ApplyDefaultCornerRadius(string value)
        {
            ResourceDictionary resourceDictionary = GetControlResourceDictionary().GetResourceDictionary(sizesCulture);

            var cornerRadiusarray = value.Split(',').Select(p => double.Parse(p)).ToArray();
            resourceDictionary["DefaultCornerRadius"] = new CornerRadius(cornerRadiusarray[0], cornerRadiusarray[1], cornerRadiusarray[2], cornerRadiusarray[3]);
        }

        public static ResourceDictionary GetControlResourceDictionary()
        {
            ResourceDictionary resourceDictionary = Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source != null && (d.Source.OriginalString.Contains(materialDesignCulture) || d.Source.OriginalString.Contains(mahAppsCulture)));
            return resourceDictionary;
        }

        private void ApplyFontSize(double fontSize)
        {
            ResourceDictionary resourceDictionary = GetControlResourceDictionary().GetResourceDictionary(sizesCulture);
            resourceDictionary["AIStudio.Font.Size"] = fontSize;
            resourceDictionary["AIStudio.Icon.Size.Minimum"] = fontSize - 4;
            resourceDictionary["AIStudio.Icon.Size.Small"] = fontSize - 2;
            resourceDictionary["AIStudio.Icon.Size.Medium"] = fontSize + 2;
            resourceDictionary["AIStudio.Icon.Size.Large"] = fontSize + 6;
            resourceDictionary["AIStudio.Icon.Size.ExtraLarge"] = fontSize + 18;
            resourceDictionary["AIStudio.Avatar.Size.Small"] = fontSize + 12;
            resourceDictionary["AIStudio.Avatar.Size.Medium"] = fontSize + 20;
            resourceDictionary["AIStudio.Avatar.Size.Large"] = fontSize + 28;
            resourceDictionary["AIStudio.Avatar.Size.ExtraLarge"] = fontSize + 48;
            resourceDictionary["AIStudio.Badged.Size"] = fontSize + 10;
            resourceDictionary["AIStudio.Hamburger.Size"] = fontSize + 20;
            resourceDictionary["AIStudio.Hamburger.Size.OpenPanel"] = (fontSize + 20) * 6.5;

            resourceDictionary["AIStudio.Notice.Width"] = (fontSize + 38) * 6;
            resourceDictionary["AIStudio.Notice.Height"] = (fontSize + 24) * 3;

            resourceDictionary["AIStudio.Header.Size"] = fontSize + 20;
        }

        private void ApplyFontFamily(FontFamily fontFamily)
        {
            ResourceDictionary resourceDictionary = GetControlResourceDictionary().GetResourceDictionary(sizesCulture);
            resourceDictionary["AIStudio.Font.FontFamily"] = fontFamily;
        }

    }

    public static class ResourceDictionaryHelper
    {
        public static ResourceDictionary GetResourceDictionary(this ResourceDictionary resourceDictionary, string name)
        {
            return resourceDictionary.MergedDictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains(name));
        }
    }
}
