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
        private IDictionary<Entity, SoundEffect> entitySounds = new Dictionary<Entity, SoundEffect>();

        public void AddEntity(Entity entity)
        {
            var sound = entity.GetIfHas<AudioComponent>();
            if (sound != null)
            {
                this.entities.Add(entity);
                this.entitySounds[entity] = LoadSound(sound.FileName);
            }
        }

        public void OnUpdate()
        {
            foreach (var entity in entities)
            {
                var sound = entity.GetIfHas<AudioComponent>();
                if (sound.ShouldPlay == true)
                {
                    sound.ShouldPlay = false;
                    var soundEffect = entitySounds[entity];
                    Play(soundEffect);
                }
            }
        }

        private static SoundEffect LoadSound(string fileName)
        {
             using (var stream = File.Open(fileName, FileMode.Open))
            {
                var texture = SoundEffect.FromStream(stream);
                return texture;
            }
        }

        // Mostly copied from https://stackoverflow.com/questions/35183043/how-do-i-play-a-sound-effect-on-monogame-for-android
        private void Play(SoundEffect soundEffect, float volume = 1.0f, float pitch = 1.0f, bool loop = false)
        {
            var soundInstance = soundEffect.CreateInstance();
            soundInstance.IsLooped = loop;
            soundInstance.Pitch = pitch;
            soundInstance.Volume =  volume;
            soundInstance.Play();
        }
    }
}