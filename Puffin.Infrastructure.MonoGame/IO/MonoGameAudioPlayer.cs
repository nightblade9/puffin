using Microsoft.Xna.Framework.Audio;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using System.Collections.Generic;
using System.IO;

namespace Puffin.Infrastructure.MonoGame
{
    internal class MonoGameAudioPlayer : IAudioPlayer
    {
        private List<Entity> entities = new List<Entity>();
        private IDictionary<AudioComponent, SoundEffect> entitySounds = new Dictionary<AudioComponent, SoundEffect>();
        private IDictionary<AudioComponent, List<SoundEffectInstance>> soundInstances = new Dictionary<AudioComponent, List<SoundEffectInstance>>();

        public MonoGameAudioPlayer(EventBus eventBus)
        {
            eventBus.Subscribe(EventBusSignal.PlayAudio, this.Play);
            eventBus.Subscribe(EventBusSignal.StopAudio, this.Stop);
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
            // Trim dead/done sound effect instances
            foreach (var kvp in soundInstances)
            {
                foreach (var instance in kvp.Value.ToArray())
                {
                    if (instance.State == SoundState.Stopped)
                    {
                        kvp.Value.Remove(instance);
                    }
                }
            }
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
            soundInstance.Volume =  audioComponent.Volume;
            //soundInstance.IsLooped = loop;
            soundInstance.Play();

            if (!soundInstances.ContainsKey(audioComponent) || soundInstances[audioComponent] == null)
            {
                soundInstances[audioComponent] = new List<SoundEffectInstance>();
            }
            soundInstances[audioComponent].Add(soundInstance);
        }

        private void Stop(object data)
        {
            var audioComponent = data as AudioComponent;
            if (soundInstances.ContainsKey(audioComponent) && soundInstances[audioComponent] != null)
            {
                foreach (var instance in soundInstances[audioComponent])
                {
                    instance.Stop();
                }
            }
        }
    }
}