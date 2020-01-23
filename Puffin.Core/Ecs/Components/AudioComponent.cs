namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A component that can play an audio file (short or long), optionally at a modified pitch.
    public class AudioComponent : Component
    {
        internal readonly string FileName;
        internal bool ShouldPlay { get; set; }

        public AudioComponent(Entity parent, string fileName) : base(parent)
        {
            this.FileName = fileName;
        }

        public void Play()
        {
            this.ShouldPlay = true;
        }
    }
}