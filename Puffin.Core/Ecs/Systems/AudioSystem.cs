using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Systems
{
    class AudioSystem : ISystem
    {
        private readonly IAudioPlayer audioPlayer;
        private readonly IList<Entity> entities = new List<Entity>();
        
        public AudioSystem(IAudioPlayer audioPlayer)
        {
            this.audioPlayer = audioPlayer;
        }

        public void OnAddEntity(Entity entity)
        {
            if (entity.Get<AudioComponent>() != null)
            {
                this.entities.Add(entity);
                this.audioPlayer.AddEntity(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
            this.audioPlayer.RemoveEntity(entity);
        }

        public void OnUpdate(TimeSpan elapsed)
        {
            this.audioPlayer.OnUpdate();
        }
    }
}