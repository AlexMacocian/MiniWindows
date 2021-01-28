using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Miwi.Tests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MiniWindowManager windowManager;

        public MainWindow()
        {
            InitializeComponent();
            this.windowManager = new MiniWindowManager()
                .WithActiveColor(Colors.CadetBlue)
                .WithInactiveColor(Colors.Gray)
                .WithForegroundColor(Colors.White)
                .WithHighlightColor(Colors.White)
                .WithWindowContainer(this.WindowContainer)
                .WithWindowTabContainer(this.TabContainer);
            this.windowManager.AddWindow("SomeWindow", new Grid() { Background = new SolidColorBrush(Colors.Red) });
            this.windowManager.AddWindow("SomeWindow2", new Grid() { Background = new SolidColorBrush(Colors.Yellow) });
            this.windowManager.AddWindow("SomeWindow3", new Grid() { Background = new SolidColorBrush(Colors.Green) });
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.OemPlus)
            {
                this.windowManager.FocusNext();
            }
            else if (e.Key == System.Windows.Input.Key.OemMinus)
            {
                this.windowManager.FocusPrevious();
            }
        }
    }
}
