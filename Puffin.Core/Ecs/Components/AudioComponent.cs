using System;
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
        internal float Pitch = 0;
        public float Volume { get {
            return _volume;
            }
            set {
                _volume = value;
                this.Parent.Scene.EventBus.Broadcast(EventBusSignal.VolumeChanged, this);
            }
        }

        private float _volume;


        public AudioComponent(Entity parent, string fileName) : base(parent)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Plays the audio file specified in <c>fileName</c> at the specified volume and pitch.
        /// </summary>
        /// <param name="volume">A volume of 1.0 is 100%; a volume of 0 is 0% (completely muted).</param>
        /// <param name="pitch">A pitch of 0 is 100% (normal); -1 plays at half the pitch, 1 plays at double pitch.</param>
        public void Play(float volume = 1.0f, float pitch = 0f)
        {
            if (volume < 0 || volume > 1)
            {
                throw new ArgumentException("Volume must be in the range [0..1]");
            }

            if (pitch < -1 || pitch > 1)
            {
                throw new ArgumentException("Pitch must be in the range [-1..1].");
            }

            this.Parent.Scene.EventBus.Broadcast(EventBusSignal.PlayAudio, this);
            this.Volume = volume;
            this.Pitch = pitch;
        }

        /// <summary>
        /// Stops all instances of the current sound effect that are playing.
        /// </summary>
        public void Stop()
        {
            this.Parent.Scene.EventBus.Broadcast(EventBusSignal.StopAudio, this);
        }
    }
}