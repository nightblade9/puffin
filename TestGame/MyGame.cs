using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Puffin.Infrastructure.MonoGame;

namespace MyGame
{
    public class MyGame : PuffinGame
    {
        public MyGame() : base(960, 540)
        {
            this.ActionToKeys[CustomAction.Next] = new List<Keys>()
            {
                Keys.Space,
                Keys.Enter,
            };
        }
        
        override protected void Ready()
        {
            // TODO: don't use references to source for Puffin.Infrastructure.MonoGame!
            this.ShowScene(new CoreGameScene());
        }
    }



    public enum CustomAction
    {
        Next,
        Previous,
    }
}
