using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Skype_Concept
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd,ref WindowCompositionAttributeData data);

        public Window2() => InitializeComponent();

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 10,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        internal enum WindowCompositionAttribute
        {
            //...
            WCA_ACCENT_POLICY = 19
            //...
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        /// <summary>
        /// Enable blur
        /// </summary>
        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy()
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
            };

            var accentStructSize= Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);

            Marshal.StructureToPtr(accent,accentPtr,false);

            var data = new WindowCompositionAttributeData
            {
                Attribute=WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData=accentStructSize,
                Data=accentPtr
            };


            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => EnableBlur();

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

        private void ButtonClose_Click(object sender, RoutedEventArgs e) => Close();

        private void ButtonMax_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState==WindowState.Normal)
            {
                ButtonMax.Content = "\xE923";
                WindowState = WindowState.Maximized;
            }
            else
            {
                ButtonMax.Content = "\xE922";
                WindowState = WindowState.Normal;
            }
        }

        private void ButtonMin_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    }
}
