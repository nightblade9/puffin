using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using NAudio.Wave;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using Puffin.Infrastructure.MonoGame.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Puffin.Infrastructure.MonoGame
{
    internal class MonoGameAudioPlayer : IAudioPlayer, IDisposable
    {
        private List<Entity> entities = new List<Entity>();
        private IDictionary<AudioComponent, SoundEffect> entityWavs = new Dictionary<AudioComponent, SoundEffect>();
        private IDictionary<AudioComponent, WaveOutEvent> entityMp3s = new Dictionary<AudioComponent, WaveOutEvent>();

        private static SoundEffect LoadWavFile(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open))
            {
                var soundEffect = SoundEffect.FromStream(stream);
                return soundEffect;
            }
        }

        private static WaveOutEvent LoadMp3File(string fileName, bool shouldLoop)
        {
            var reader = new Mp3FileReader(fileName);
        
            if (shouldLoop)
            {
                var looper = new LoopStream(reader);
                var waveOut = new WaveOutEvent();
                waveOut.Init(looper);
                return waveOut;
            }
            else
            {
                var waveOut = new WaveOutEvent();
                waveOut.Init(reader);
                return waveOut;
            }
        }

        public MonoGameAudioPlayer(EventBus eventBus)
        {
            eventBus.Subscribe(EventBusSignal.PlayAudio, this.Play);
            eventBus.Subscribe(EventBusSignal.StopAudio, this.Stop);
            eventBus.Subscribe(EventBusSignal.VolumeChanged, (data) =>
            {
                // Don't change volume if you didn't call Play. Just. Don't.
                var audio = data as AudioComponent;
                if (audio == null)
                {
                    throw new InvalidOperationException("We shouldn't be sending a volume-changed event when the sound didn't play yet!");
                }

                var soundEffectInstance = audio.MonoGameAudioInstance as SoundEffectInstance;
                if (soundEffectInstance != null)
                {
                    soundEffectInstance.Volume = audio.Volume;
                }
                else
                {
                    var songInstance = audio.MonoGameAudioInstance as WaveOutEvent;
                    if (songInstance != null)
                    {
                        songInstance.Volume = audio.Volume;
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
                if (sound.FileName.ToUpperInvariant().EndsWith(".MP3"))
                {
                    this.entityMp3s[sound] = LoadMp3File(sound.FileName, sound.ShouldLoop);
                }
                else
                {
                    this.entityWavs[sound] = LoadWavFile(sound.FileName);
                }
            }
        }

        public void RemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
            var sound = entity.Get<AudioComponent>();

            if (sound != null)
            {
                if (entityWavs.ContainsKey(sound))
                {
                    entityWavs.Remove(sound);
                }
                else if (entityMp3s.ContainsKey(sound))
                {
                    var waveOutEvent = entityMp3s[sound];
                    waveOutEvent.Stop();
                    waveOutEvent.Dispose();
                    entityMp3s.Remove(sound);
                }
            }
        }

        public void OnUpdate()
        {
        }
        
        public void Dispose()
        {
            foreach (var kvp in this.entityMp3s)
            {
                var waveOutEvent = this.entityMp3s[kvp.Key];
                if (waveOutEvent != null)
                {
                    waveOutEvent.Stop();
                    waveOutEvent.Dispose();
                }
            }

            MediaPlayer.Stop();
        }

        private void Play(object data)
        {
            var audioComponent = data as AudioComponent;
            if (this.entityMp3s.ContainsKey(audioComponent))
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
            this.entityMp3s[audioComponent].Play();
            audioComponent.MonoGameAudioInstance = this.entityMp3s[audioComponent];
        }

        private void PlaySoundEffect(AudioComponent audioComponent)
        {
            var soundEffect = entityWavs[audioComponent];
            
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

            var instance2 = audioComponent.MonoGameAudioInstance as WaveOutEvent;
            instance2?.Stop();
        }
    }
}