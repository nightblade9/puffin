using Puffin.Core.Events;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Allows an entity to play an audio file (short or long), optionally at a modified pitch.
    /// You should be able to play WAV files and OGG files.
    /// </summary>
    public class AudioComponent : Component
    {
        internal readonly string FileName;
        internal float Pitch = 1.0f;

        public AudioComponent(Entity parent, string fileName) : base(parent)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Plays the audio file specified in fileName` at the specified pitch.
        /// A pitch of 0 is 100% (normal); -1 plays at half the pitch, 1 plays at double pitch.
        /// </summary>
        public void Play(float pitch = 1.0f)
        {
            this.Pitch = pitch;
            this.Parent.Scene.EventBus.Broadcast(EventBusSignal.PlayAudio, this);
        }
    }
}