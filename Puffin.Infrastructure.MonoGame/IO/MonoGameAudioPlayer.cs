using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Puffin.Infrastructure.MonoGame
{
    internal class MonoGameAudioPlayer : IAudioPlayer
    {
        private List<Entity> entities = new List<Entity>();
        private IDictionary<AudioComponent, SoundEffect> entitySounds = new Dictionary<AudioComponent, SoundEffect>();
        private IDictionary<AudioComponent, Song> entitySongs = new Dictionary<AudioComponent, Song>();
        private IDictionary<AudioComponent, List<SoundEffectInstance>> soundInstances = new Dictionary<AudioComponent, List<SoundEffectInstance>>();

        private static SoundEffect LoadSound(string fileName)
        {
             using (var stream = File.Open(fileName, FileMode.Open))
            {
                var soundEffect = SoundEffect.FromStream(stream);
                return soundEffect;
            }
        }

        private static Song LoadSong(string fileName)
        {
            var name = fileName.Substring(fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            using (var stream = File.Open(fileName, FileMode.Open))
            {
                var song = Song.FromUri(name, new Uri(fileName, UriKind.Relative));
                return song;
            }
        }

        public MonoGameAudioPlayer(EventBus eventBus)
        {
            eventBus.Subscribe(EventBusSignal.PlayAudio, this.Play);
            eventBus.Subscribe(EventBusSignal.StopAudio, this.Stop);
            eventBus.Subscribe(EventBusSignal.VolumeChanged, (data) => {
                var audio = data as AudioComponent;
                if (this.soundInstances.ContainsKey(audio))
                {
                    foreach (var instance in this.soundInstances[audio])
                    {
                        // TODO: changing one component shouldn't change all instances of the audio
                        instance.Volume = audio.Volume;
                    }
                }
            });
        }

        public void AddEntity(Entity entity)
        {
            var sound = entity.Get<AudioComponent>();
            if (sound != null)
            {
                this.entities.Add(entity);
                if (sound.FileName.ToUpperInvariant().EndsWith(".MP3") || sound.FileName.ToUpperInvariant().EndsWith(".OGG"))
                {
                    // Assume Song instance
                    this.entitySongs[sound] = LoadSong(sound.FileName);
                }
                else
                {
                    // SFX, assume SoundEffect
                    this.entitySounds[sound] = LoadSound(sound.FileName);
                }
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

        private void Play(object data)
        {
            var audioComponent = data as AudioComponent;
            var soundEffect = entitySounds[audioComponent];
            
            // Mostly copied from https://stackoverflow.com/questions/35183043/how-do-i-play-a-sound-effect-on-monogame-for-android
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