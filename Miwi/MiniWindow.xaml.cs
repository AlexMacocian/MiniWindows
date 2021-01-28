using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Miwi
{
    /// <summary>
    /// Interaction logic for MiniWindow.xaml
    /// </summary>
    public partial class MiniWindow : UserControl
    {
        public event EventHandler ExitClicked, MinimizeClicked, MaximizeClicked, RestoreClicked, Clicked;

        private Size prevWindowSize = new Size(0, 0);
        private Point prevWindowPosition = new Point(0, 0);
        private Point clickPosition, startPosition;
        private bool active, dragging, resizing;
        private ResizeDirection resizeDirection = ResizeDirection.Undefined;
        private Color activeColor, inactiveColor;
        private Docking docking;
        private WindowState prevState;

        public UIElement Child
        {
            get => this.Holder.Children.Count > 0 ? this.Holder.Children[0] : null;
            set
            {
                this.Holder.Children.Clear();
                this.Holder.Children.Add(value);
            }
        }
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
        public Color ActiveColor
        {
            get => this.activeColor;
            set
            {
                this.activeColor = value;
                if (this.Active)
                {
                    this.Activate();
                }
            }
        }
        public Color InactiveColor
        {
            get => this.inactiveColor;
            set
            {
                this.inactiveColor = value;
                if (!this.Active)
                {
                    this.Deactivate();
                }
            }
        }
        public WindowState WindowState
        {
            get => this.Titlebar.WindowState;
            set 
            {
                this.prevState = this.Titlebar.WindowState;
                this.Titlebar.WindowState = value;
                this.UpdateState();
            }
        }
        public string WindowTitle
        {
            get => this.Titlebar.Title;
            set => this.Titlebar.Title = value;
        }
        public Brush TitlebarForeground
        {
            get => this.Titlebar.Foreground;
            set => this.Titlebar.Foreground = value;
        }
        public Brush TitlebarHighlight
        {
            get => this.Titlebar.Highlight;
            set => this.Titlebar.Highlight = value;
        }
        public Docking Docking
        {
            get => this.docking;
            set
            {
                this.docking = value;
                this.UpdateDocking();
            }
        }

        public MiniWindow()
        {
            InitializeComponent();
        }

        public void Resize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.Width = (this.Parent as FrameworkElement).ActualWidth;
                this.Height = (this.Parent as FrameworkElement).ActualHeight;
            }
            else if (this.Docking != Docking.None)
            {
                this.UpdateDocking();
            }
        }

        private void MiniWindowTitlebar_ExitButtonClicked(object sender, EventArgs e)
        {
            this.ExitClicked?.Invoke(this, e);
        }
        private void MiniWindowTitlebar_MinimizeButtonClicked(object sender, EventArgs e)
        {
            this.MinimizeClicked?.Invoke(this, e);
            this.WindowState = WindowState.Minimized;
        }
        private void MiniWindowTitlebar_MaximizeButtonClicked(object sender, EventArgs e)
        {
            this.MaximizeClicked?.Invoke(this, e);
            this.WindowState = WindowState.Maximized;
        }
        private void MiniWindowTitlebar_RestoreButtonClicked(object sender, EventArgs e)
        {
            this.RestoreClicked?.Invoke(this, e);
            this.WindowState = WindowState.Normal;
        }

        private void Titlebar_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Point newPos = e.GetPosition(this.Parent as UIElement);
                double offsetX = this.clickPosition.X - newPos.X;
                double offsetY = this.clickPosition.Y - newPos.Y;
                if (this.Docking != Docking.None)
                {
                    if (Math.Abs(offsetX) < 10 && Math.Abs(offsetY) < 10)
                    {
                        return;
                    }
                    else
                    {
                        this.Docking = Docking.None;
                    }
                }
                Canvas.SetTop(this, startPosition.Y - (offsetY * 1.01));
                Canvas.SetLeft(this, startPosition.X - (offsetX * 1.01));
            }
            else
            {
                this.dragging = false;
            }
        }
        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.clickPosition = e.GetPosition(this.Parent as UIElement);
                this.startPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                if (double.IsNaN(startPosition.X))
                {
                    startPosition.X = 0;
                }
                if (double.IsNaN(startPosition.Y))
                {
                    startPosition.Y = 0;
                }
                this.dragging = true;
            }

            this.Clicked?.Invoke(sender, e);
            e.Handled = true;
        }
        private void Holder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Clicked?.Invoke(this, e);
        }
        private void Titlebar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.dragging = false;
                Point pos = e.GetPosition(this.Parent as UIElement);
                if (this.Docking == Docking.None && this.WindowState == WindowState.Normal)
                {
                    if (pos.X <= 20)
                    {
                        if (pos.Y <= 20)
                        {
                            this.Docking = Docking.TopLeft;
                        }
                        else if (pos.Y >= (this.Parent as FrameworkElement).ActualHeight - 20)
                        {
                            this.Docking = Docking.BottomLeft;
                        }
                        else
                        {
                            this.Docking = Docking.Left;
                        }

                    }
                    else if (pos.Y <= 20)
                    {
                        if (pos.X <= 20)
                        {
                            this.Docking = Docking.TopLeft;
                        }
                        else if (pos.X >= (this.Parent as FrameworkElement).ActualWidth - 20)
                        {
                            this.Docking = Docking.TopRight;
                        }
                        else
                        {
                            this.Docking = Docking.Top;
                        }
                    }
                    else if (pos.X >= (this.Parent as FrameworkElement).ActualWidth - 20)
                    {
                        if (pos.Y <= 20)
                        {
                            this.Docking = Docking.TopRight;
                        }
                        else if (pos.Y >= (this.Parent as FrameworkElement).ActualHeight - 20)
                        {
                            this.Docking = Docking.BottomRight;
                        }
                        else
                        {
                            this.Docking = Docking.Right;
                        }
                    }
                    else if (pos.Y >= (this.Parent as FrameworkElement).ActualHeight - 20)
                    {
                        if (pos.X <= 20)
                        {
                            this.Docking = Docking.BottomLeft;
                        }
                        else if (pos.X >= (this.Parent as FrameworkElement).ActualWidth - 20)
                        {
                            this.Docking = Docking.BottomRight;
                        }
                        else
                        {
                            this.Docking = Docking.Bottom;
                        }
                    }
                    else
                    {
                        this.Docking = Docking.None;
                    }
                }
            }
            catch
            {
            }
        }
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.resizing)
                {
                    if (this.resizeDirection == ResizeDirection.Top)
                    {
                        Point currentPosition = e.GetPosition(TopBorder);
                        double Yoffset = currentPosition.Y - clickPosition.Y;
                        if (Height <= 240 && Yoffset > 0)
                        {

                        }
                        else
                        {
                            Canvas.SetTop(this, Canvas.GetTop(this) + Yoffset / 2);
                            Height -= Yoffset / 2;
                        }

                    }
                    else if (this.resizeDirection == ResizeDirection.Bottom)
                    {
                        Point currentPosition = e.GetPosition(BottomBorder);
                        double Yoffset = currentPosition.Y - clickPosition.Y;
                        if (Height <= 240 && Yoffset < 0)
                        {

                        }
                        else
                        {
                            Height += Yoffset / 2;
                        }
                    }
                    else if (this.resizeDirection == ResizeDirection.Left)
                    {
                        Point currentPosition = e.GetPosition(LeftBorder);
                        double Xoffset = currentPosition.X - clickPosition.X;
                        if (Width <= 320 && Xoffset > 0)
                        {

                        }
                        else
                        {
                            Canvas.SetLeft(this, Canvas.GetLeft(this) + Xoffset / 2);
                            Width -= Xoffset / 2;
                        }

                    }
                    else if (this.resizeDirection == ResizeDirection.Right)
                    {
                        Point currentPosition = e.GetPosition(RightBorder);
                        double Xoffset = currentPosition.X - clickPosition.X;
                        if (Width <= 320 && Xoffset < 0)
                        {

                        }
                        else
                        {
                            Width += Xoffset / 2;
                        }
                    }
                    else if (this.resizeDirection == ResizeDirection.TopLeft)
                    {
                        Point currentPosition = e.GetPosition(TopLeftCorner);
                        double Xoffset = currentPosition.X - clickPosition.X;
                        double Yoffset = currentPosition.Y - clickPosition.Y;
                        if (Width <= 320 && Xoffset > 0)
                        {

                        }
                        else
                        {
                            Canvas.SetLeft(this, Canvas.GetLeft(this) + Xoffset / 2);
                            Width -= Xoffset / 2;
                        }
                        if (Height <= 240 && Yoffset > 0)
                        {

                        }
                        else
                        {
                            Canvas.SetTop(this, Canvas.GetTop(this) + Yoffset / 2);
                            Height -= Yoffset / 2;
                        }
                    }
                    else if (this.resizeDirection == ResizeDirection.TopRight)
                    {
                        Point currentPosition = e.GetPosition(TopRightCorner);
                        double Xoffset = currentPosition.X - clickPosition.X;
                        double Yoffset = currentPosition.Y - clickPosition.Y;
                        if (Width <= 320 && Xoffset < 0)
                        {

                        }
                        else
                        {
                            Width += Xoffset / 2;
                        }
                        if (Height <= 240 && Yoffset > 0)
                        {

                        }
                        else
                        {
                            Canvas.SetTop(this, Canvas.GetTop(this) + Yoffset);
                            Height -= Yoffset;
                        }
                    }
                    else if (this.resizeDirection == ResizeDirection.BottomLeft)
                    {
                        Point currentPosition = e.GetPosition(BottomLeftCorner);
                        double Xoffset = currentPosition.X - clickPosition.X;
                        double Yoffset = currentPosition.Y - clickPosition.Y;
                        if (Width <= 320 && Xoffset > 0)
                        {

                        }
                        else
                        {
                            Canvas.SetLeft(this, Canvas.GetLeft(this) + Xoffset / 2);
                            Width -= Xoffset / 2;
                        }
                        if (Height <= 240 && Yoffset < 0)
                        {

                        }
                        else
                        {
                            Height += Yoffset / 2;
                        }
                    }
                    else if (this.resizeDirection == ResizeDirection.BottomRight)
                    {
                        Point currentPosition = e.GetPosition(BottomRightCorner);
                        double Xoffset = currentPosition.X - clickPosition.X;
                        double Yoffset = currentPosition.Y - clickPosition.Y;
                        if (Width <= 320 && Xoffset < 0)
                        {

                        }
                        else
                        {
                            Width += Xoffset / 2;
                        }
                        if (Height <= 240 && Yoffset < 0)
                        {

                        }
                        else
                        {
                            Height += Yoffset / 2;
                        }
                    }
                }
            }
            catch { }
        }

        private void Activate()
        {
            this.Titlebar.Background = new SolidColorBrush(this.ActiveColor);
        }
        private void Deactivate()
        {
            this.Titlebar.Background = new SolidColorBrush(this.InactiveColor);
        }

        private void UpdateState()
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                if (this.prevState == WindowState.Minimized)
                {
                    this.Visibility = Visibility.Visible;
                }
                else if (this.prevState == WindowState.Maximized)
                {
                    Canvas.SetLeft(this, this.prevWindowPosition.X);
                    Canvas.SetTop(this, this.prevWindowPosition.Y);
                    this.Width = this.prevWindowSize.Width;
                    this.Height = this.prevWindowSize.Height;
                }
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                if (this.prevState == WindowState.Normal)
                {
                    this.prevWindowSize = new Size(this.Width, this.Height);
                    this.prevWindowPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                    this.Width = (this.Parent as Canvas).ActualWidth;
                    this.Height = (this.Parent as Canvas).ActualHeight;
                }
                else if (this.prevState == WindowState.Minimized)
                {
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                    this.Width = (this.Parent as Canvas).ActualWidth;
                    this.Height = (this.Parent as Canvas).ActualHeight;
                    this.Visibility = Visibility.Visible;
                }
            }
        }
        private void UpdateDocking()
        {
            switch (this.Docking)
            {
                case Docking.TopLeft:
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                    this.Width = (this.Parent as FrameworkElement).ActualWidth / 2;
                    this.Height = (this.Parent as FrameworkElement).ActualHeight / 2;
                    break;
                case Docking.TopRight:
                    Canvas.SetLeft(this, (this.Parent as FrameworkElement).ActualWidth / 2);
                    Canvas.SetTop(this, 0);
                    this.Width = (this.Parent as FrameworkElement).ActualWidth / 2;
                    this.Height = (this.Parent as FrameworkElement).ActualHeight / 2;
                    break;
                case Docking.BottomLeft:
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, (this.Parent as FrameworkElement).ActualHeight / 2);
                    this.Width = (this.Parent as FrameworkElement).ActualWidth / 2;
                    this.Height = (this.Parent as FrameworkElement).ActualHeight / 2;
                    break;
                case Docking.BottomRight:
                    Canvas.SetLeft(this, (this.Parent as FrameworkElement).ActualWidth / 2);
                    Canvas.SetTop(this, (this.Parent as FrameworkElement).ActualHeight / 2);
                    this.Width = (this.Parent as FrameworkElement).ActualWidth / 2;
                    this.Height = (this.Parent as FrameworkElement).ActualHeight / 2;
                    break;
                case Docking.Top:
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                    this.Width = (this.Parent as FrameworkElement).ActualWidth;
                    this.Height = (this.Parent as FrameworkElement).ActualHeight / 2;
                    break;
                case Docking.Right:
                    Canvas.SetLeft(this, (this.Parent as FrameworkElement).ActualWidth / 2);
                    Canvas.SetTop(this, 0);
                    this.Width = (this.Parent as FrameworkElement).ActualWidth / 2;
                    this.Height = (this.Parent as FrameworkElement).ActualHeight;
                    break;
                case Docking.Left:
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                    this.Width = (this.Parent as FrameworkElement).ActualWidth / 2;
                    this.Height = (this.Parent as FrameworkElement).ActualHeight;
                    break;
                case Docking.Bottom:
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, (this.Parent as FrameworkElement).ActualHeight / 2);
                    this.Width = (this.Parent as FrameworkElement).ActualWidth;
                    this.Height = (this.Parent as FrameworkElement).ActualHeight / 2;
                    break;
                case Docking.None:
                    break;
            }
        }

        private void BeginResize(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal && this.Docking == Docking.None)
            {
                var clickedShape = sender as Rectangle;
                switch (clickedShape.Name)
                {
                    case "TopBorder":
                        this.clickPosition = e.GetPosition(clickedShape);
                        this.resizeDirection = ResizeDirection.Top;
                        break;
                    case "RightBorder":
                        this.clickPosition = e.GetPosition(clickedShape);
                        this.resizeDirection = ResizeDirection.Right;
                        break;
                    case "BottomBorder":
                        this.clickPosition = e.GetPosition(clickedShape);
                        this.resizeDirection = ResizeDirection.Bottom;
                        break;
                    case "LeftBorder":
                        this.clickPosition = e.GetPosition(clickedShape);
                        this.resizeDirection = ResizeDirection.Left;
                        break;
                    case "TopLeftCorner":
                        this.clickPosition = e.GetPosition(clickedShape);
                        this.resizeDirection = ResizeDirection.TopLeft;
                        break;
                    case "TopRightCorner":
                        this.clickPosition = e.GetPosition(clickedShape);
                        this.resizeDirection = ResizeDirection.TopRight;
                        break;
                    case "BottomRightCorner":
                        this.clickPosition = e.GetPosition(clickedShape);
                        this.resizeDirection = ResizeDirection.BottomRight;
                        break;
                    case "BottomLeftCorner":
                        this.clickPosition = e.GetPosition(clickedShape);
                        this.resizeDirection = ResizeDirection.BottomLeft;
                        break;
                    default:
                        break;
                }
                this.resizing = true;
                clickedShape.CaptureMouse();
            }
        }
        private void EndResize(object sender, MouseButtonEventArgs e)
        {
            var clickedShape = sender as Rectangle;
            clickedShape.ReleaseMouseCapture();
            this.resizing = false;
            this.resizeDirection = ResizeDirection.Undefined;
        }
    }
}
