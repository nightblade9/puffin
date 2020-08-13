using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using NAudio.Wave;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Puffin.Infrastructure.MonoGame
{
    internal class MonoGameAudioPlayer : IAudioPlayer, IDisposable
    {
        private List<Entity> entities = new List<Entity>();
        private IDictionary<AudioComponent, SoundEffect> entitySounds = new Dictionary<AudioComponent, SoundEffect>();
        private IDictionary<AudioComponent, WaveOutEvent> entitySongs = new Dictionary<AudioComponent, WaveOutEvent>();

        private static SoundEffect LoadSound(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open))
            {
                var soundEffect = SoundEffect.FromStream(stream);
                return soundEffect;
            }
        }

        private static WaveOutEvent LoadSong(string fileName)
        {
            // using (var stream = File.Open(fileName, FileMode.Open))
            // {
            //     var song = Song.FromUri(fileName, new Uri(fileName, UriKind.Relative));
            //     return song;
            // }

            var reader = new Mp3FileReader(fileName);
            var waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            return waveOut;
        }

        public MonoGameAudioPlayer(EventBus eventBus)
        {
            eventBus.Subscribe(EventBusSignal.PlayAudio, this.Play);
            eventBus.Subscribe(EventBusSignal.StopAudio, this.Stop);
            eventBus.Subscribe(EventBusSignal.VolumeChanged, (data) =>
            {
                // Don't change volume if you didn't call Play. Just. Don't.
                var audio = data as AudioComponent;
                var instance = audio.soundEffectInstance as SoundEffectInstance;
                if (instance == null)
                {
                    throw new InvalidOperationException("We shouldn't be sending a volume-changed event when the sound didn't play yet!");
                }
                instance.Volume = audio.Volume;
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

            if (sound != null)
            {
                if (entitySounds.ContainsKey(sound))
                {
                    entitySounds.Remove(sound);
                }
                else if (entitySongs.ContainsKey(sound))
                {
                    entitySongs[sound].Stop();
                    entitySongs[sound].Dispose();
                    entitySongs.Remove(sound);
                }
            }
        }

        public void OnUpdate()
        {
        }
        
        public void Dispose()
        {
            foreach (var kvp in this.entitySongs)
            {
                var song = this.entitySongs[kvp.Key];
                if (song != null)
                {
                    song.Stop();
                    song.Dispose();
                }
            }
        }

        private void Play(object data)
        {
            var audioComponent = data as AudioComponent;
            if (this.entitySongs.ContainsKey(audioComponent))
            {
                this.PlaySong(audioComponent);
            }
            else
            {
                this.PlaySoundEffect(audioComponent);
            }
        }

        private void PlaySong(AudioComponent audioComponent)
        {
            this.entitySongs[audioComponent].Play();
        }

        private void PlaySoundEffect(AudioComponent audioComponent)
        {
            var soundEffect = entitySounds[audioComponent];
            
            // Mostly copied from https://stackoverflow.com/questions/35183043/how-do-i-play-a-sound-effect-on-monogame-for-android
            var soundInstance = soundEffect.CreateInstance();
            soundInstance.Pitch = audioComponent.Pitch;
            soundInstance.Volume =  audioComponent.Volume;
            soundInstance.IsLooped = audioComponent.ShouldLoop;
            soundInstance.Play();
            audioComponent.soundEffectInstance = soundInstance;
        }

        private void Stop(object data)
        {
            var audioComponent = data as AudioComponent;
            var instance = audioComponent.soundEffectInstance as SoundEffectInstance;
            // Instance may be null if it was never played / instantly stopped? This also appears to be null if OpenAL
            // has an issue, e.g. prints out this error: 
            // AL lib: (EE) SetChannelMap: Failed to match front-center channel (2) in channel map
            instance?.Stop(true);
        }
    }
}