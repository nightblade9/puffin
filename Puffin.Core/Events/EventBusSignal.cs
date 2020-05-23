namespace Puffin.Core.Events
{
    enum EventBusSignal
    {
        SpriteSheetFrameIndexChanged, // Changed a spritesheet frame index, recalc rect
        MouseClicked, // Clicked mouse, fire click handlers,
        MouseReleased,
        PlayAudio, // Play audio at a specific pitch and volume
        StopAudio, // Stop all instances of a given sound effect
        LabelFontChanged, // Font name/size changed, load/generate required font
        ActionPressed,
        ActionReleased,
        SubSceneShown, // Broadcast so we can call init on it properly
        SubSceneHidden,
        SpriteChanged, // Sprite changed at runtime
    }
}