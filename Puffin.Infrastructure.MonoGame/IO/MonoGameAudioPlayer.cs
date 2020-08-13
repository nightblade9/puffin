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
        // WAVs
        private IDictionary<AudioComponent, SoundEffect> entitySounds = new Dictionary<AudioComponent, SoundEffect>();
        // OGGs
        private IDictionary<AudioComponent, Song> entitySongs = new Dictionary<AudioComponent, Song>();
        // MP3s
        private IDictionary<AudioComponent, WaveOutEvent> entityWaveOutEvents = new Dictionary<AudioComponent, WaveOutEvent>();

        // WAV files
        private static SoundEffect LoadSound(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open))
            {
                var soundEffect = SoundEffect.FromStream(stream);
                return soundEffect;
            }
        }

        // OGG files
        private static Song LoadSong(string fileName)
        {
            var name = fileName.Substring(fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            using (var stream = File.Open(fileName, FileMode.Open))
            {
                var song = Song.FromUri(name, new Uri(fileName, UriKind.Relative));
                return song;
            }
        }

        // MP3 files
        private static WaveOutEvent LoadWaveOutEvent(string fileName)
        {
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
                var instance = audio.MonoGameAudioInstance as SoundEffectInstance;
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
                if (sound.FileName.ToUpperInvariant().EndsWith(".MP3"))
                {
                    this.entityWaveOutEvents[sound] = LoadWaveOutEvent(sound.FileName);
                }
                else if (sound.FileName.ToUpperInvariant().EndsWith(".OGG"))
                {
                    this.entitySongs[sound] = LoadSong(sound.FileName);
                }
                else
                {
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
                    entitySongs.Remove(sound);
                }
                else if (entityWaveOutEvents.ContainsKey(sound))
                {
                    var waveOutEvent = entityWaveOutEvents[sound];
                    waveOutEvent.Stop();
                    waveOutEvent.Dispose();
                    entityWaveOutEvents.Remove(sound);
                }
            }
        }

        public void OnUpdate()
        {
        }
        
        public void Dispose()
        {
            foreach (var kvp in this.entityWaveOutEvents)
            {
                var waveOutEvent = this.entityWaveOutEvents[kvp.Key];
                if (waveOutEvent != null)
                {
                    waveOutEvent.Stop();
                    waveOutEvent.Dispose();
                }
            }
        }

        private void Play(object data)
        {
            var audioComponent = data as AudioComponent;
            if (this.entityWaveOutEvents.ContainsKey(audioComponent))
            {
                this.PlayWaveOutEvent(audioComponent);
            }
            else
            {
                this.PlaySoundEffect(audioComponent);
            }
        }

        private void PlayWaveOutEvent(AudioComponent audioComponent)
        {
            this.entityWaveOutEvents[audioComponent].Play();
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
            audioComponent.MonoGameAudioInstance = soundInstance;
        }

        private void Stop(object data)
        {
            var audioComponent = data as AudioComponent;
            var instance = audioComponent.MonoGameAudioInstance as SoundEffectInstance;
            // Instance may be null if it was never played / instantly stopped? This also appears to be null if OpenAL
            // has an issue, e.g. prints out this error: 
            // AL lib: (EE) SetChannelMap: Failed to match front-center channel (2) in channel map
            instance?.Stop(true);
        }
    }
}