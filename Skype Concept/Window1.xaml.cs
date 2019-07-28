using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Skype_Concept
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        #region field

        private int _mouseDownCount = 0;

        #endregion

        #region reference user32
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

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

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);

            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
        #endregion

        #region construction method

        public Window1()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            MouseLeftButtonDown += Window_MouseLeftButtonDown;
        }
        #endregion

        #region some event & some method
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            switch (button.Tag)
            {
                case "Minimized":
                    WindowState = WindowState.Minimized;
                    break;
                case "Maximized":
                    if (WindowState == WindowState.Normal)
                    {
                        button.Content = "\xE923";
                        WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        button.Content = "\xE922";
                        WindowState = WindowState.Normal;
                    }
                    break;
                case "Close":
                    Close();
                    break;
                default:
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => EnableBlur();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDownCount++;

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300),
                IsEnabled = true
            };
            timer.Tick += (s, ev) =>
            {
                timer.IsEnabled = false; _mouseDownCount = 0;
            };
            if (_mouseDownCount % 2 == 0)
            {
                WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
            }

        }

        #endregion
    }
}
