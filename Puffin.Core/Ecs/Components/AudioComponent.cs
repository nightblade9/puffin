namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A component that can play an audio file (short or long), optionally at a modified pitch.
    public class AudioComponent : Component
    {
        internal readonly string FileName;
        internal float Pitch = 1.0f;

        public AudioComponent(Entity parent, string fileName) : base(parent)
        {
            this.FileName = fileName;
        }

        public void Play(float pitch = 1.0f)
        {
            this.Pitch = pitch;
            EventBus.LatestInstance.Broadcast(EventBusSignal.PlayAudio, this);
        }
    }
}