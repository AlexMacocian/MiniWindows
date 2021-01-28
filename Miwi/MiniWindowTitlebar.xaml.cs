using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MiniWindows
{
    /// <summary>
    /// Interaction logic for MiniWindowTitlebar.xaml
    /// </summary>
    public partial class MiniWindowTitlebar : UserControl
    {
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(MiniWindowTitlebar));
        public readonly static DependencyProperty HighlightProperty = DependencyProperty.Register(nameof(Highlight), typeof(Brush), typeof(MiniWindowTitlebar));
        public readonly static DependencyProperty WindowStateProperty = DependencyProperty.Register(nameof(WindowState), typeof(WindowState), typeof(MiniWindowTitlebar));

        public event EventHandler ExitButtonClicked, RestoreButtonClicked, MaximizeButtonClicked, MinimizeButtonClicked;

        public WindowState WindowState
        {
            get => (WindowState)this.GetValue(WindowStateProperty);
            set
            {
                this.SetValue(WindowStateProperty, value);
                this.UpdateWindowState();
            }
        }
        public Brush Highlight
        {
            get => this.GetValue(HighlightProperty) as Brush;
            set => this.SetValue(HighlightProperty, value);
        }
        public string Title
        {
            get => this.GetValue(TitleProperty) as string;
            set => this.SetValue(TitleProperty, value);
        }

        public MiniWindowTitlebar()
        {
            InitializeComponent();
        }

        private void UpdateWindowState()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.MaximizeButton.Visibility = Visibility.Collapsed;
                this.RestoreButton.Visibility = Visibility.Visible;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.MaximizeButton.Visibility = Visibility.Visible;
                this.RestoreButton.Visibility = Visibility.Collapsed;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitButtonClicked?.Invoke(this, e);
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            MaximizeButtonClicked?.Invoke(this, e);
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            RestoreButtonClicked?.Invoke(this, e);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            MinimizeButtonClicked?.Invoke(this, e);
        }
    }
}
