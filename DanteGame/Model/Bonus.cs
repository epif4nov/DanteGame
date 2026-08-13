using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DanteGame.Model
{
    public enum BonusType
    {
        Health,
        Invincibility
    }

    public class Bonus
    {
        public Rectangle Bounds;
        private Texture2D texture;
        private BonusType type;
        public bool IsActive = true;

        private Vector2 basePosition;
        private float bobTimer = 0f;

        public Bonus(Texture2D texture, int x, int y, int width, int height, BonusType type)
        {
            this.texture = texture;
            this.type = type;
            basePosition = new Vector2(x, y);
            Bounds = new Rectangle(x, y, width, height);
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            bobTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            var bobOffset = (float)Math.Sin(bobTimer * 4) * 3;

            Bounds = new Rectangle(
                (int)basePosition.X,
                (int)(basePosition.Y + bobOffset),
                Bounds.Width,
                Bounds.Height
            );
        }

        public void Apply(Player player)
        {
            if (!IsActive) return;
            
            switch (type)
            {
                case BonusType.Health:
                    if (player.Health < 3)
                    {
                        player.Health++;
                        IsActive = false;
                    }
                    break;

                case BonusType.Invincibility:
                    if (player.InvincibilityTimer <= 0)
                    {
                        player.ActivateInvincibility(5f);
                        IsActive = false;
                    }
                    break;
            }

            if (player.Health < 3)
            {
                player.Health++;
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(texture, Bounds, Color.White);
            }
        }
    }
}