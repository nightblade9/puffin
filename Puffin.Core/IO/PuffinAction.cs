namespace Puffin.Core.IO
{
    /// <summary>
    /// Internal puffin actions; you can subscribe to them if you're interested in receiving the events.
    /// For example, you can use the up/down/left/right action events to implement tile-based movement.
    /// </summary>
    public enum PuffinAction
    {

        Up,
        Down,
        Left,
        Right,
    }
}