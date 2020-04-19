namespace Puffin.Core.Events
{
    enum EventBusSignal
    {
        SpriteSheetFrameIndexChanged, // Changed a spritesheet frame index, recalc rect
        MouseClicked, // Clicked mouse, fire click handlers,
        MouseReleased,
        PlayAudio, // Play audio at a specific pitch and volume
        LabelFontChanged, // Font name/size changed, load/generate required font
        ActionPressed,
        ActionReleased,
        SubSceneShown, // broadcast so we can init it properly
    }
}