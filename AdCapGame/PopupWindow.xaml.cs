using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;


namespace AdCapGame
{
    public partial class PopupWindow : Window
    {

        private Window mainWindow;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_LAYERED = 0x00080000;

        public PopupWindow(Window mainWindow)
        {
            InitializeComponent();
            Loaded += PopupWindow_Loaded;
            this.mainWindow = mainWindow; // Store the reference
        }

        private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }

        public void ShowMessage(string message, int displayTimeMilliseconds)
        {
            messageTextBlock.Text = message;
            // Positioning code...
            if (mainWindow != null)
            {
                double top = mainWindow.Top + 30; // Adjust based on your needs, adding a margin if necessary
                double left = mainWindow.Left + mainWindow.Width - this.Width - 30; // Same here for the horizontal margin

                this.Top = top;
                this.Left = left;
            }

            Show();
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(displayTimeMilliseconds);
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                Close(); // Use Close here to ensure the window closes properly
            };
            timer.Start();
        }

    }
}
