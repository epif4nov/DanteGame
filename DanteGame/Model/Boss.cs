using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DanteGame.Model
{
    public class Boss
    {
        public Rectangle Bounds;
        private Texture2D texture;
        private Vector2 position;
        private SpriteFont font;

        public int Health;
        private int maxHealth;

        private float speed = 50f;
        private float attackTimer = 0f;
        private float phaseTimer = 0f;
        private int currentPhase = 1;

        private bool isAttacking = false;
        private float attackDuration = 0f;
        private float attackCooldown = 2f;

        private List<BossProjectile> projectiles = new List<BossProjectile>();

        private Color bossColor = Color.White;

        public Boss(Texture2D texture, float x, float y, int width, int height, SpriteFont font)
        {
            this.texture = texture;
            this.font = font;
            position = new Vector2(x, y);
            Bounds = new Rectangle((int)x, (int)y, width, height);
            maxHealth = 30;
            Health = maxHealth;
        }

        public void Update(GameTime gameTime, Player player)
        {
            if (Health <= 0) return;

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var healthPercent = (float)Health / maxHealth;

            if (healthPercent > 0.66f)
                currentPhase = 1;
            else if (healthPercent > 0.33f)
                currentPhase = 2;
            else
                currentPhase = 3;

            switch (currentPhase)
            {
                case 1: bossColor = Color.White; break;
                case 2: bossColor = Color.Orange; break;
                case 3: bossColor = Color.Red; break;
            }

            for (var i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(deltaTime);
                if (projectiles[i].IsActive == false)
                    projectiles.RemoveAt(i);
                else if (projectiles[i].Bounds.Intersects(player.GetBounds()))
                {
                    player.TakeDamageFromEnemy(2);
                    projectiles.RemoveAt(i);
                }
            }

            if (player.Position.X < position.X)
                position.X -= speed * deltaTime;
            else if (player.Position.X > position.X)
                position.X += speed * deltaTime;

            attackTimer += deltaTime;

            if (attackTimer >= attackCooldown && !isAttacking)
            {
                attackTimer = 0;
                isAttacking = true;
                attackDuration = 0.5f;

                switch (currentPhase)
                {
                    case 1:
                        FireProjectile(player);
                        break;
                    case 2:
                        FireProjectile(player, -0.3f);
                        FireProjectile(player, 0.3f);
                        break;
                    case 3:
                        speed = 70f;
                        FireProjectile(player, -0.4f);
                        FireProjectile(player, 0f);
                        FireProjectile(player, 0.4f);
                        break;
                }
            }

            if (isAttacking)
            {
                attackDuration -= deltaTime;
                if (attackDuration <= 0)
                    isAttacking = false;
            }

            Bounds = new Rectangle((int)position.X, (int)position.Y, Bounds.Width, Bounds.Height);
        }

        private void FireProjectile(Player player, float angleOffset = 0)
        {
            var directionToPlayer = player.Position - position;
            directionToPlayer.Normalize();

            var angle = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X) + angleOffset;
            var velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 300f;

            var projectile = new BossProjectile(
                new Rectangle((int)position.X + Bounds.Width / 2 - 8, (int)position.Y + Bounds.Height / 2 - 8, 16, 16),
                velocity);
            projectiles.Add(projectile);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color drawColor = bossColor;
            if (isAttacking)
            {
                var flash = (float)Math.Sin(attackDuration * 30) * 0.5f + 0.5f;
                drawColor = Color.Lerp(bossColor, Color.White, flash);
            }

            spriteBatch.Draw(texture, Bounds, drawColor);

            var barWidth = 500;
            var barHeight = 25;
            var barX = 1280 / 2 - barWidth / 2;
            var barY = 30;

            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkRed);

            var currentWidth = (int)(barWidth * ((float)Health / maxHealth));
            Color barColor = currentPhase == 1 ? Color.Red : (currentPhase == 2 ? Color.Orange : Color.DarkRed);
            spriteBatch.Draw(pixel, new Rectangle(barX, barY, currentWidth, barHeight), barColor);

            var bossName = $"ЛЮЦИФЕР - ФАЗА {currentPhase}";
            var nameSize = font.MeasureString(bossName);
            spriteBatch.DrawString(font, bossName, new Vector2(1280 / 2 - nameSize.X / 2, barY - 25), Color.White);

            foreach (var projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }
    }

    public class BossProjectile
    {
        public Rectangle Bounds;
        private Vector2 velocity;
        public bool IsActive = true;

        public BossProjectile(Rectangle bounds, Vector2 velocity)
        {
            this.Bounds = bounds;
            this.velocity = velocity;
        }

        public void Update(float deltaTime)
        {
            Bounds = new Rectangle(
                Bounds.X + (int)(velocity.X * deltaTime),
                Bounds.Y + (int)(velocity.Y * deltaTime),
                Bounds.Width,
                Bounds.Height);

            if (Bounds.X < -200 || Bounds.X > 1500 || Bounds.Y < -200 || Bounds.Y > 1000)
                IsActive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            spriteBatch.Draw(pixel, Bounds, Color.OrangeRed);
        }
    }
}