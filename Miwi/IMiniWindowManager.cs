using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Miwi
{
    public interface IMiniWindowManager : IMiniWindowProducer
    {
        /// <summary>
        /// Returns currently focused window. Returns null if there are no windows.
        /// </summary>
        MiniWindow Focused { get; }
        /// <summary>
        /// <see cref="Canvas"/> used to contain the windows.
        /// </summary>
        Canvas WindowContainer { get; set; }
        /// <summary>
        /// <see cref="StackPanel"/> used to contain the window tabs.
        /// </summary>
        StackPanel WindowTabContainer { get; set; }
        /// <summary>
        /// Color used when a window is active.
        /// </summary>
        Color ActiveColor { get; set; }
        /// <summary>
        /// Color used when a window is inactive.
        /// </summary>
        Color InactiveColor { get; set; }
        /// <summary>
        /// Color used as highlight for buttons.
        /// </summary>
        Color HighlightColor { get; set; }
        /// <summary>
        /// Color of the text.
        /// </summary>
        Color ForegroundColor { get; set; }
        MiniWindowManager WithWindowContainer(Canvas canvas);
        MiniWindowManager WithWindowTabContainer(StackPanel stackPanel);
        MiniWindowManager WithActiveColor(Color color);
        MiniWindowManager WithInactiveColor(Color color);
        MiniWindowManager WithHighlightColor(Color color);
        MiniWindowManager WithForegroundColor(Color color);
        /// <summary>
        /// Refreshes the window container.
        /// </summary>
        void RefreshView();
        /// <summary>
        /// Activate a specific window.
        /// </summary>
        void Activate(MiniWindow miniWindow);
        /// <summary>
        /// Deactivates all windows.
        /// </summary>
        void DeactivateAll();
        /// <summary>
        /// Deselects all tabs.
        /// </summary>
        void DeselectAllTabs();
        /// <summary>
        /// Close and remove a window.
        /// </summary>
        void RemoveWindow(MiniWindow miniWindow);
        /// <summary>
        /// Deactivate a window.
        /// </summary>
        void Deactivate(MiniWindow miniWindow);
        /// <summary>
        /// Deactivates current window and activates the following window in the order of the tabs.
        /// </summary>
        void FocusNext();
        /// <summary>
        /// Deactivates current window and activates the previous window in the order of the tabs.
        /// </summary>
        void FocusPrevious();
    }
}
