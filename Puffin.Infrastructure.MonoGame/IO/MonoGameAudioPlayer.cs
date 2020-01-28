using Microsoft.Xna.Framework.Audio;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using System.Collections.Generic;
using System.IO;

namespace Puffin.Infrastructure.MonoGame
{
    internal class MonoGameAudioPlayer : IAudioPlayer
    {
        private List<Entity> entities = new List<Entity>();
        private IDictionary<AudioComponent, SoundEffect> entitySounds = new Dictionary<AudioComponent, SoundEffect>();

        public MonoGameAudioPlayer()
        {
            EventBus.LatestInstance.Subscribe(EventBusSignal.PlayAudio, this.Play);
        }

        public void AddEntity(Entity entity)
        {
            var sound = entity.Get<AudioComponent>();
            if (sound != null)
            {
                this.entities.Add(entity);
                this.entitySounds[sound] = LoadSound(sound.FileName);
            }
        }

        public void RemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
            var sound = entity.Get<AudioComponent>();
            if (sound != null && entitySounds.ContainsKey(sound))
            {
                entitySounds.Remove(sound);
            }
        }

        public void OnUpdate()
        {
        }

        private static SoundEffect LoadSound(string fileName)
        {
             using (var stream = File.Open(fileName, FileMode.Open))
            {
                var soundEffect = SoundEffect.FromStream(stream);
                return soundEffect;
            }
        }

        // Mostly copied from https://stackoverflow.com/questions/35183043/how-do-i-play-a-sound-effect-on-monogame-for-android
        private void Play(object data)
        {
            var audioComponent = data as AudioComponent;
            var soundEffect = entitySounds[audioComponent];
            
            var soundInstance = soundEffect.CreateInstance();
            soundInstance.Pitch = audioComponent.Pitch;
            //soundInstance.IsLooped = loop;
            //soundInstance.Volume =  volume;
            soundInstance.Play();
        }
    }
}