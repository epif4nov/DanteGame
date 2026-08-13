using DanteGame.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DanteGame
{
    public class GameView
    {
        private SpriteBatch spriteBatch;
        private Texture2D backgroundTexture;
        private SpriteFont font;

        public GameView(SpriteBatch spriteBatch, Texture2D backgroundTexture, SpriteFont font)
        {
            this.spriteBatch = spriteBatch;
            this.backgroundTexture = backgroundTexture;
            this.font = font;
        }

        public void Draw(Player player, List<Platform> platforms, List<Enemy> enemies, GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);

            foreach (Platform platform in platforms)
                platform.Draw(spriteBatch);

            foreach (Enemy enemy in enemies)
                enemy.Draw(spriteBatch);

            player.Draw(spriteBatch, gameTime);

            for (var i = 0; i < player.Health; i++)
            {
                Texture2D heart = new Texture2D(spriteBatch.GraphicsDevice, 30, 30);
                Color[] heartColor = new Color[30 * 30];
                for (var j = 0; j < heartColor.Length; j++)
                    heartColor[j] = Color.Red;
                heart.SetData(heartColor);
                spriteBatch.Draw(heart, new Rectangle(20 + i * 35, 20, 30, 30), Color.White);
            }
        }
    }
}