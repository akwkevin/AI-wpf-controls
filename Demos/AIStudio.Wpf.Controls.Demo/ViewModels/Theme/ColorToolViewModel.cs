using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    public class ColorToolViewModel : BindableBase
    {
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

        private ColorScheme _activeScheme;
        public ColorScheme ActiveScheme
        {
            get => _activeScheme;
            set
            {
                SetProperty(ref _activeScheme, value);
            }
        }

        private Color? _selectedColor;
        public Color? SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (SetProperty(ref _selectedColor, value))
                {
                    // if we are triggering a change internally its a hue change and the colors will match
                    // so we don't want to trigger a custom color change.
                    Color? currentSchemeColor;
                    switch (ActiveScheme)
                    {
                        case ColorScheme.Primary:
                            currentSchemeColor = _primaryColor; 
                            break;
                        case ColorScheme.Secondary:
                            currentSchemeColor = _secondaryColor; 
                            break;
                        case ColorScheme.PrimaryForeground:
                            currentSchemeColor = _primaryForegroundColor; 
                            break;
                        case ColorScheme.SecondaryForeground:
                            currentSchemeColor = _secondaryForegroundColor; 
                            break;
                        default: throw new NotSupportedException($"{ActiveScheme} is not a handled ColorScheme.. Ye daft programmer!"); 
                            break;
                    };

                    if (_selectedColor != currentSchemeColor && value is Color color)
                    {
                        ChangeCustomColor(color);
                    }
                }
            }
        }

        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        public ICommand ChangeCustomHueCommand
        {
            get;
        }

        public ICommand ChangeHueCommand
        {
            get;
        }
        public ICommand ChangeToPrimaryCommand
        {
            get;
        }
        public ICommand ChangeToSecondaryCommand
        {
            get;
        }
        public ICommand ChangeToPrimaryForegroundCommand
        {
            get;
        }
        public ICommand ChangeToSecondaryForegroundCommand
        {
            get;
        }

        public ICommand ToggleBaseCommand
        {
            get;
        }

        private void ApplyBase(bool isDark)
        {
            ITheme theme = _paletteHelper.GetTheme();
            IBaseTheme baseTheme = isDark ? (IBaseTheme)(new MaterialDesignDarkTheme()) : (IBaseTheme)(new MaterialDesignLightTheme());
            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
        }


        public ColorToolViewModel()
        {
            ToggleBaseCommand = new DelegateCommand<bool>(o => ApplyBase(o));
            ChangeHueCommand = new DelegateCommand<object>(o => ChangeHue(o));
            ChangeCustomHueCommand = new DelegateCommand<object>(o => ChangeCustomColor(o));
            ChangeToPrimaryCommand = new DelegateCommand(() => ChangeScheme(ColorScheme.Primary));
            ChangeToSecondaryCommand = new DelegateCommand(() => ChangeScheme(ColorScheme.Secondary));
            ChangeToPrimaryForegroundCommand = new DelegateCommand(() => ChangeScheme(ColorScheme.PrimaryForeground));
            ChangeToSecondaryForegroundCommand = new DelegateCommand(() => ChangeScheme(ColorScheme.SecondaryForeground));


            ITheme theme = _paletteHelper.GetTheme();

            _primaryColor = theme.PrimaryMid.Color;
            _secondaryColor = theme.SecondaryMid.Color;

            SelectedColor = _primaryColor;
        }

        private void ChangeCustomColor(object obj)
        {
            var color = (Color)obj;

            if (ActiveScheme == ColorScheme.Primary)
            {
                _paletteHelper.ChangePrimaryColor(color);
                _primaryColor = color;
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                _paletteHelper.ChangeSecondaryColor(color);
                _secondaryColor = color;
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SetPrimaryForegroundToSingleColor(color);
                _primaryForegroundColor = color;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SetSecondaryForegroundToSingleColor(color);
                _secondaryForegroundColor = color;
            }
        }

        private void ChangeScheme(ColorScheme scheme)
        {
            ActiveScheme = scheme;
            if (ActiveScheme == ColorScheme.Primary)
            {
                SelectedColor = _primaryColor;
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                SelectedColor = _secondaryColor;
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SelectedColor = _primaryForegroundColor;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SelectedColor = _secondaryForegroundColor;
            }
        }

        private Color? _primaryColor;

        private Color? _secondaryColor;

        private Color? _primaryForegroundColor;

        private Color? _secondaryForegroundColor;

        private void ChangeHue(object obj)
        {
            var hue = (Color)obj;

            SelectedColor = hue;
            if (ActiveScheme == ColorScheme.Primary)
            {
                _paletteHelper.ChangePrimaryColor(hue);
                _primaryColor = hue;
                _primaryForegroundColor = _paletteHelper.GetTheme().PrimaryMid.GetForegroundColor();
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                _paletteHelper.ChangeSecondaryColor(hue);
                _secondaryColor = hue;
                _secondaryForegroundColor = _paletteHelper.GetTheme().SecondaryMid.GetForegroundColor();
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SetPrimaryForegroundToSingleColor(hue);
                _primaryForegroundColor = hue;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SetSecondaryForegroundToSingleColor(hue);
                _secondaryForegroundColor = hue;
            }
        }

        private void SetPrimaryForegroundToSingleColor(Color color)
        {
            ITheme theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(theme.PrimaryLight.Color, color);
            theme.PrimaryMid = new ColorPair(theme.PrimaryMid.Color, color);
            theme.PrimaryDark = new ColorPair(theme.PrimaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }

        private void SetSecondaryForegroundToSingleColor(Color color)
        {
            ITheme theme = _paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(theme.SecondaryLight.Color, color);
            theme.SecondaryMid = new ColorPair(theme.SecondaryMid.Color, color);
            theme.SecondaryDark = new ColorPair(theme.SecondaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }
    }

    public enum ColorScheme
    {
        Primary,
        Secondary,
        PrimaryForeground,
        SecondaryForeground
    }

    public static class PaletteHelperExtensions
    {
        public static void ChangePrimaryColor(this PaletteHelper paletteHelper, Color color)
        {
            ITheme theme = paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());

            paletteHelper.SetTheme(theme);
        }

        public static void ChangeSecondaryColor(this PaletteHelper paletteHelper, Color color)
        {
            ITheme theme = paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(color.Lighten());
            theme.SecondaryMid = new ColorPair(color);
            theme.SecondaryDark = new ColorPair(color.Darken());

            paletteHelper.SetTheme(theme);
        }
    }
}
