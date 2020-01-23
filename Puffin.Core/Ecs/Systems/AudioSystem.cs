using Puffin.Core.Drawing;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Systems
{
    class AudioSystem : ISystem
    {
        private IAudioPlayer audioPlayer;
        private IList<Entity> entities = new List<Entity>();
        
        // For unit testing
        internal AudioSystem() { }

        public AudioSystem(IAudioPlayer audioPlayer)
        {
            this.audioPlayer = audioPlayer;
        }

        public virtual void OnAddEntity(Entity entity)
        {
            if (entity.GetIfHas<AudioComponent>() != null)
            {
                this.entities.Add(entity);
                this.audioPlayer.AddEntity(entity);
            }
        }

        public virtual void OnUpdate(TimeSpan elapsed)
        {
            this.audioPlayer.OnUpdate();
        }
    }
}