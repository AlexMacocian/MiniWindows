using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Miwi
{
    public sealed class MiniWindowManager : IMiniWindowManager
    {
        private Canvas windowContainer;
        private StackPanel windowTabContainer;
        private List<MiniWindowItem> Windows { get; } = new List<MiniWindowItem>();

        /// <summary>
        /// Returns currently focused window. Returns null if there are no windows.
        /// </summary>
        public MiniWindow Focused { get => this.Windows.Count > 0 ? this.Windows.Last().MiniWindow : null; }
        /// <summary>
        /// <see cref="Canvas"/> used to contain the windows.
        /// </summary>
        public Canvas WindowContainer
        {
            get => this.windowContainer;
            set
            {
                if (this.windowContainer is object) throw new InvalidOperationException($"{nameof(WindowContainer)} can be set only once!");

                this.windowContainer = value;
                this.windowContainer.SizeChanged += WindowContainer_SizeChanged;
                this.RefreshView();
            }
        }
        /// <summary>
        /// <see cref="StackPanel"/> used to contain the window tabs.
        /// </summary>
        public StackPanel WindowTabContainer
        {
            get => this.windowTabContainer;
            set
            {
                if (this.windowTabContainer is object) throw new InvalidOperationException($"{WindowTabContainer} can be set only once!");

                this.windowTabContainer = value;
                this.RefreshView();
            }
        }
        /// <summary>
        /// Color used when a window is active.
        /// </summary>
        public Color ActiveColor { get; set; }
        /// <summary>
        /// Color used when a window is inactive.
        /// </summary>
        public Color InactiveColor { get; set; }
        /// <summary>
        /// Color used as highlight for buttons.
        /// </summary>
        public Color HighlightColor { get; set; }
        /// <summary>
        /// Color of the text.
        /// </summary>
        public Color ForegroundColor { get; set; }

        public MiniWindowManager WithWindowContainer(Canvas canvas)
        {
            this.WindowContainer = canvas;
            return this;
        }
        public MiniWindowManager WithWindowTabContainer(StackPanel stackPanel)
        {
            this.WindowTabContainer = stackPanel;
            return this;
        }
        public MiniWindowManager WithActiveColor(Color color)
        {
            this.ActiveColor = color;
            return this;
        }
        public MiniWindowManager WithInactiveColor(Color color)
        {
            this.InactiveColor = color;
            return this;
        }
        public MiniWindowManager WithHighlightColor(Color color)
        {
            this.HighlightColor = color;
            return this;
        }
        public MiniWindowManager WithForegroundColor(Color color)
        {
            this.ForegroundColor = color;
            return this;
        }
        /// <summary>
        /// Add a window to the manager.
        /// </summary>
        /// <param name="content">Element to be displayed as content of the window.</param>
        /// <param name="name">Name of the window. Displayed in the titlebar as well as in the tab.</param>
        public void AddWindow(string name, UIElement content)
        {
            this.InnerAddWindow(name, content);
        }
        /// <summary>
        /// Refreshes the window container.
        /// </summary>
        public void RefreshView()
        {
            this.WindowContainer?.Children.Clear();
            foreach (var item in this.Windows)
            {
                this.WindowContainer?.Children.Add(item.MiniWindow);
            }
        }
        /// <summary>
        /// Activate a specific window.
        /// </summary>
        public void Activate(MiniWindow miniWindow)
        {
            this.InnerActivate(miniWindow);
        }
        /// <summary>
        /// Deactivates all windows.
        /// </summary>
        public void DeactivateAll()
        {
            this.InnerDeactivateAll();
        }
        /// <summary>
        /// Deselects all tabs.
        /// </summary>
        public void DeselectAllTabs()
        {
            this.InnerDeselectAllTabs();
        }
        /// <summary>
        /// Close and remove a window.
        /// </summary>
        public void RemoveWindow(MiniWindow miniWindow)
        {
            this.InnerRemoveWindow(miniWindow);
        }
        /// <summary>
        /// Deactivate a window.
        /// </summary>
        public void Deactivate(MiniWindow miniWindow)
        {
            this.InnerDeactivate(miniWindow);
        }
        /// <summary>
        /// Deactivates current window and activates the following window in the order of the tabs.
        /// </summary>
        public void FocusNext()
        {
            this.InnerFocusNext();
        }
        /// <summary>
        /// Deactivates current window and activates the previous window in the order of the tabs.
        /// </summary>
        public void FocusPrevious()
        {
            this.InnerFocusPrevious();
        }

        private void InnerAddWindow(string name, UIElement content)
        {
            var miniWindow = new MiniWindow()
            {
                ActiveColor = this.ActiveColor,
                InactiveColor = this.InactiveColor,
                Foreground = new SolidColorBrush(this.ForegroundColor),
                WindowState = WindowState.Normal,
                WindowTitle = name,
                TitlebarForeground = new SolidColorBrush(this.ForegroundColor),
                TitlebarHighlight = new SolidColorBrush(this.HighlightColor),
                Child = content,
                Height = 480,
                Width = 640
            };
            var miniWindowTab = new MiniWindowTab
            {
                ActiveColor = this.ActiveColor,
                InactiveColor = this.InactiveColor,
                Foreground = new SolidColorBrush(this.ForegroundColor),
                Title = name,
                Highlight = new SolidColorBrush(this.HighlightColor),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var miniWindowItem = new MiniWindowItem(miniWindow, miniWindowTab);
            this.Windows.Add(miniWindowItem);
            miniWindow.Clicked += (s, e) => this.Activate(miniWindow);
            miniWindow.ExitClicked += (s, e) => this.RemoveWindow(miniWindow);
            miniWindowTab.CloseClicked += (s, e) => this.RemoveWindow(miniWindow);
            miniWindowTab.Clicked += (s, e) =>
            {
                this.Activate(miniWindow);
                if (miniWindow.WindowState == WindowState.Minimized)
                {
                    miniWindow.WindowState = WindowState.Normal;
                }
            };
            this.WindowTabContainer?.Children.Add(miniWindowTab);
            Activate(miniWindow);
        }
        private void InnerRemoveWindow(MiniWindow miniWindow)
        {
            var item = this.Windows.Where(i => i.MiniWindow == miniWindow).FirstOrDefault();
            if (item is null)
            {
                throw new InvalidOperationException($"Cannot remove a window that is not part of this {nameof(MiniWindowManager)}.");
            }

            this.Windows.Remove(item);
            this.WindowContainer?.Children.Remove(item.MiniWindow);
            this.WindowTabContainer?.Children.Remove(item.MiniWindowTab);
            RefreshView();
        }
        private void InnerActivate(MiniWindow miniWindow)
        {
            var item = this.Windows.Where(i => i.MiniWindow == miniWindow).FirstOrDefault();
            if (item is null)
            {
                return;
            }

            if (!item.MiniWindow.Active)
            {
                item.MiniWindow.Active = true;
                item.MiniWindow.Focus();
                this.Windows.Remove(item);
                DeactivateAll();
                item.MiniWindowTab.Active = true;
                this.Windows.Add(item);
                RefreshView();
            }
        }
        private void InnerDeactivateAll()
        {
            foreach (var item in this.Windows)
            {
                item.MiniWindow.Active = false;
                item.MiniWindowTab.Active = false;
            }
        }
        private void InnerDeselectAllTabs()
        {
            foreach (var item in this.Windows)
            {
                item.MiniWindowTab.Active = false;
            }
        }
        private void InnerDeactivate(MiniWindow miniWindow)
        {
            var item = this.Windows.Find(i => i.MiniWindow == miniWindow);
            if (item is null)
            {
                return;
            }

            item.MiniWindow.Active = false;
            item.MiniWindowTab.Active = false;
        }
        private void InnerFocusNext()
        {
            if (this.Windows.Count < 2)
            {
                return;
            }

            var currentFocusedIndex = this.FindCurrentFocusedTabIndex();
            if (currentFocusedIndex < 0 || currentFocusedIndex > this.Windows.Count)
            {
                return;
            }

            var nextFocusedIndex = (currentFocusedIndex + 1) % this.Windows.Count;
            this.ActivateWindowFromTabIndex(nextFocusedIndex);
        }
        private void InnerFocusPrevious()
        {
            if (this.Windows.Count < 2)
            {
                return;
            }

            var currentFocusedIndex = this.FindCurrentFocusedTabIndex();
            if (currentFocusedIndex < 0 || currentFocusedIndex > this.Windows.Count)
            {
                return;
            }

            var nextFocusedIndex = (currentFocusedIndex - 1) % this.Windows.Count;
            if (nextFocusedIndex < 0)
            {
                nextFocusedIndex = this.Windows.Count + nextFocusedIndex;
            }

            this.ActivateWindowFromTabIndex(nextFocusedIndex);
        }
        private int FindCurrentFocusedTabIndex()
        {
            var currentFocusedTab = this.Windows.Find(w => w.MiniWindow == this.Focused)?.MiniWindowTab;
            var currentFocusedIndex = this.WindowTabContainer.Children.IndexOf(currentFocusedTab);
            return currentFocusedIndex;
        }
        private MiniWindowTab FindTabAtIndex(int index)
        {
            return this.WindowTabContainer.Children[index] as MiniWindowTab;
        }
        private void ActivateWindowFromTabIndex(int tabIndex)
        {
            var newTab = this.FindTabAtIndex(tabIndex);
            if (newTab is null)
            {
                return;
            }

            var miniWindow = this.Windows.Find(w => w.MiniWindowTab == newTab)?.MiniWindow;
            this.InnerActivate(miniWindow);
        }
        private void WindowContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in this.Windows)
            {
                item.MiniWindow.Resize();
            }
        }
    }
}
