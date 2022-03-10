using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bubblepuz.GameObjects
{
    class Cannon : GameObject
    {
        public Ball ball = null;
        Random rnd = new Random();
        Vector2 mousePosition;


        public Cannon(Texture2D texture) : base(texture)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture,
                            Position,
                            null,
                            Color.White,
                            Rotation,
                            new Vector2(Singleton.BALLHITBOX, Singleton.BALLHITBOX),
                            1f,
                            SpriteEffects.None,
                            0
                            );
            Reset();
            base.Draw(spriteBatch);
        }

        public override void Reset()
        {

            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {

            Singleton.Instance.CurrentMouse = Mouse.GetState();
            //rotaion
            mousePosition = new Vector2(Singleton.Instance.CurrentMouse.X, Singleton.Instance.CurrentMouse.Y);
            if(mousePosition.Y >= 600)
            {
                mousePosition.Y = 600;
            }
            Vector2 direction = mousePosition - Position;
            direction.Normalize();
            Rotation = (float)Math.Atan2((double)direction.X, (double)-direction.Y);
            base.Update(gameTime, gameObjects);
        }
    }
}