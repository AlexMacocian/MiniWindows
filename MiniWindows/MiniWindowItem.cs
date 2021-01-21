namespace MiniWindows
{
    internal sealed class MiniWindowItem
    {
        public MiniWindow MiniWindow { get; }
        public MiniWindowTab MiniWindowTab { get; }

        public MiniWindowItem(MiniWindow miniWindow, MiniWindowTab miniWindowTab)
        {
            this.MiniWindow = miniWindow;
            this.MiniWindowTab = miniWindowTab;
        }
    }
}
