using System.Windows;

namespace Miwi
{
    public interface IMiniWindowProducer
    {
        /// <summary>
     /// Add a window to the manager.
     /// </summary>
     /// <param name="content">Element to be displayed as content of the window.</param>
     /// <param name="name">Name of the window. Displayed in the titlebar as well as in the tab.</param>
        void AddWindow(string name, UIElement content);
    }
}
