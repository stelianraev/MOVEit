using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MoveitDesktopUI.Services
{
    public static class IconExtractor
    {
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_SMALLICON = 0x1;  // small icon
        private const uint SHGFI_LARGEICON = 0x0;  // large icon

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public static ImageSource GetIcon(string path, bool smallIcon = true)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | (smallIcon ? SHGFI_SMALLICON : SHGFI_LARGEICON);

            IntPtr hImg = SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            if (hImg == IntPtr.Zero)
                return null;

            ImageSource icon = Imaging.CreateBitmapSourceFromHIcon(
                shinfo.hIcon,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DestroyIcon(shinfo.hIcon);
            return icon;
        }
    }
}
