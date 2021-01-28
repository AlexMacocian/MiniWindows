using Miwi.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Miwi
{
    /// <summary>
    /// Interaction logic for MiniWindowTab.xaml
    /// </summary>
    public partial class MiniWindowTab : UserControl
    {
        public readonly static DependencyProperty HighlightVisibilityProperty = DependencyProperty.Register(nameof(HighlightVisibility), typeof(Visibility), typeof(MiniWindowTab));
        public readonly static DependencyProperty HighlightProperty = DependencyProperty.Register(nameof(Highlight), typeof(Brush), typeof(MiniWindowTab));
        public readonly static DependencyProperty HighlightOpacityProperty = DependencyProperty.Register(nameof(HighlightOpacity), typeof(double), typeof(MiniWindowTab), new PropertyMetadata(0.2));
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(MiniWindowTab));

        private bool active;
        private Color activeColor, inactiveColor;

        public event EventHandler CloseClicked, Clicked;

        public bool Active
        {
            get => this.active;
            set
            {
                this.active = value;
                if (this.active)
                {
                    this.Activate();
                }
                else
                {
                    this.Deactivate();
                }
            }
        }
        public Color ActiveColor { get => this.activeColor;
            set
            {
                this.activeColor = value;
                if (this.Active)
                {
                    this.Activate();
                }
            }
        }
        public Color InactiveColor { get => this.inactiveColor;
            set
            {
                this.inactiveColor = value;
                if (!this.Active)
                {
                    this.Deactivate();
                }
            }
        }
        public Visibility HighlightVisibility
        {
            get => (Visibility)this.GetValue(HighlightVisibilityProperty);
            set => this.SetValue(HighlightVisibilityProperty, value);
        }
        public string Title
        {
            get => this.GetValue(TitleProperty) as string;
            set => this.SetValue(TitleProperty, value);
        }
        public double HighlightOpacity
        {
            get => (double)this.GetValue(HighlightOpacityProperty);
            set => this.SetValue(HighlightOpacityProperty, value);
        }
        public Brush Highlight
        {
            get => this.GetValue(HighlightProperty) as Brush;
            set => this.SetValue(HighlightProperty, value);
        }

        public MiniWindowTab()
        {
            InitializeComponent();
            this.HighlightVisibility = Visibility.Collapsed;
            Deactivate();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.HighlightVisibility = Visibility.Visible;
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.HighlightVisibility = Visibility.Collapsed;
        }
        private void Close_ClicK(object sender, RoutedEventArgs e)
        {
            this.CloseClicked?.Invoke(this, e);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Clicked?.Invoke(this, e);
        }

        private void Activate()
        {
            this.Background = new SolidColorBrush(this.ActiveColor);
            this.Foreground = Helper.AdaptColor1ToColor2(Colors.White, this.ActiveColor);
        }
        private void Deactivate()
        {
            this.Background = new SolidColorBrush(this.InactiveColor);
            this.Foreground = Helper.AdaptColor1ToColor2(Colors.White, this.InactiveColor);
        }
    }
}
