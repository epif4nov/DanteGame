using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanteGame.Model
{
    public class Platform
    {
        public Rectangle Bounds;
        private Texture2D texture;

        public Platform(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture;
            Bounds = new Rectangle(x, y, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.White);
        }
    }
}
