using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AIStudio.Wpf.ComeCapture.Helpers
{
    public static class ScreenHelper
    {
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(
            IntPtr hWnd,
            IntPtr hDc
            );

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(
            IntPtr hdc, // handle to DC
            int nIndex // index of capability
            );

        public static System.Drawing.Size GetPhysicalDisplaySize()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int physicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.Desktopvertres);
            int physicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.Desktophorzres);
            ReleaseDC(IntPtr.Zero, desktop);
            g.Dispose();
            return new System.Drawing.Size(physicalScreenWidth, physicalScreenHeight);
        }

        public enum DeviceCap
        {
            Desktopvertres = 117,
            Desktophorzres = 118
        }

        public static double ResetScreenScale()
        {
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr desktop = g.GetHdc();
                int physicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.Desktophorzres);
                return physicalScreenWidth * 1.0000 / System.Windows.SystemParameters.PrimaryScreenWidth;
            }
        }

        //public static void GetScreenSize()
        //{
        //    foreach (Screen screen in Screen.AllScreens)
        //    {
        //        var dm = new DEVMODE();
        //        dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
        //        EnumDisplaySettings(screen.DeviceName, ENUM_CURRENT_SETTINGS, ref dm);

        //        Debug.WriteLine($"Device: {screen.DeviceName}");
        //        Debug.WriteLine($"Real Resolution: {dm.dmPelsWidth}x{dm.dmPelsHeight}");
        //        Debug.WriteLine($"Virtual Resolution: {screen.Bounds.Width}x{screen.Bounds.Height}");
        //    }            
        //}

        //const int ENUM_CURRENT_SETTINGS = -1;

        //[DllImport("user32.dll")]
        //static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct DEVMODE
        //{
        //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        //    public string dmDeviceName;
        //    public short dmSpecVersion;
        //    public short dmDriverVersion;
        //    public short dmSize;
        //    public short dmDriverExtra;
        //    public int dmFields;
        //    public int dmPositionX;
        //    public int dmPositionY;
        //    public ScreenOrientation dmDisplayOrientation;
        //    public int dmDisplayFixedOutput;
        //    public short dmColor;
        //    public short dmDuplex;
        //    public short dmYResolution;
        //    public short dmTTOption;
        //    public short dmCollate;
        //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        //    public string dmFormName;
        //    public short dmLogPixels;
        //    public int dmBitsPerPel;
        //    public int dmPelsWidth;
        //    public int dmPelsHeight;
        //    public int dmDisplayFlags;
        //    public int dmDisplayFrequency;
        //    public int dmICMMethod;
        //    public int dmICMIntent;
        //    public int dmMediaType;
        //    public int dmDitherType;
        //    public int dmReserved1;
        //    public int dmReserved2;
        //    public int dmPanningWidth;
        //    public int dmPanningHeight;
        //}
    }
}
