using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DanteGame.Model
{
    public enum EnemyType
    {
        Slow,
        Fast,
        Flying,
        Archer
    }

    public class Enemy
    {
        public Rectangle Bounds;
        private Texture2D texture;
        private Vector2 position;
        public Vector2 Velocity;

        private float speed;
        private float attackCooldown;
        private float attackRange;
        private float meleeRange = 55f;
        private float stopRange = 20f;
        private float detectionRange = 400f;
        private EnemyType enemyType;
        private bool isWounded = false;

        public int Health;
        public int MaxHealth;

        private float attackTimer = 0f;
        private float meleeAttackTimer = 0f;
        private float woundedTimer = 0f;
        private float dodgeTimer = 0f;
        private float dodgeDirection = 0;

        private List<Projectile> projectiles = new List<Projectile>();

        public Enemy(Texture2D texture, float x, float y, int width, int height, EnemyType type)
        {
            this.texture = texture;
            position = new Vector2(x, y);
            Bounds = new Rectangle((int)x, (int)y, width, height);
            Velocity = Vector2.Zero;
            enemyType = type;

            switch (type)
            {
                case EnemyType.Slow:
                    speed = 60f;
                    attackCooldown = 1.2f;
                    attackRange = 120f;
                    Health = 20;
                    MaxHealth = 20;
                    break;
                case EnemyType.Fast:
                    speed = 140f;
                    attackCooldown = 1.0f;
                    attackRange = 100f;
                    Health = 1;
                    MaxHealth = 1;
                    break;
                case EnemyType.Flying:
                    speed = 85f;
                    attackCooldown = 1.2f;
                    attackRange = 130f;
                    Health = 10;
                    MaxHealth = 10;
                    break;
            }
        }

        public void Update(GameTime gameTime, Player player, List<Enemy> allEnemies, List<Platform> platforms)
        {
            if (Health <= 0) return;

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var distanceToPlayer = Vector2.Distance(position, player.Position);
            var horizontalDist = Math.Abs(position.X - player.Position.X);
            var verticalDist = Math.Abs(position.Y - player.Position.Y);

            if (horizontalDist < meleeRange && verticalDist < 80f)
            {
                meleeAttackTimer += deltaTime;
                if (meleeAttackTimer >= attackCooldown)
                {
                    meleeAttackTimer = 0;
                    player.TakeDamageFromEnemy(1);
                }
            }
            else
            {
                meleeAttackTimer = 0;
            }

            for (var i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(deltaTime);
                if (projectiles[i].IsActive == false)
                    projectiles.RemoveAt(i);
                else if (projectiles[i].Bounds.Intersects(player.GetBounds()))
                {
                    player.TakeDamageFromEnemy(1);
                    projectiles.RemoveAt(i);
                }
            }

            if (Health <= MaxHealth / 2 && !isWounded)
            {
                isWounded = true;
                woundedTimer = 3f;
                speed *= 0.7f;
            }

            if (woundedTimer > 0)
                woundedTimer -= deltaTime;

            if (distanceToPlayer < stopRange + 30 && dodgeTimer <= 0 && (enemyType == EnemyType.Slow || enemyType == EnemyType.Fast))
            {
                dodgeDirection = (player.Position.X < position.X) ? 1 : -1;
                dodgeTimer = 0.4f;
            }

            if (dodgeTimer > 0)
            {
                dodgeTimer -= deltaTime;
                position.X += dodgeDirection * speed * 1.5f * deltaTime;
            }

            if (distanceToPlayer < detectionRange && dodgeTimer <= 0)
            {
                if (distanceToPlayer < detectionRange && dodgeTimer <= 0)
                {
                    if (player.Position.X < position.X)
                        position.X -= speed * deltaTime;
                    else if (player.Position.X > position.X)
                        position.X += speed * deltaTime;
                }

                if (enemyType == EnemyType.Flying || enemyType == EnemyType.Archer)
                {
                    if (distanceToPlayer < attackRange - 50)
                    {
                        if (player.Position.X < position.X)
                            position.X += speed * 0.8f * deltaTime;
                        else
                            position.X -= speed * 0.8f * deltaTime;
                    }
                }
            }

            if (enemyType == EnemyType.Flying || enemyType == EnemyType.Archer)
            {
                if (distanceToPlayer < attackRange && verticalDist < 80f)
                {
                    attackTimer += deltaTime;
                    if (attackTimer >= attackCooldown)
                    {
                        attackTimer = 0;
                        var direction = (player.Position.X < position.X) ? -1 : 1;
                        var projectile = new Projectile(
                            new Rectangle((int)position.X, (int)position.Y + Bounds.Height / 2, 12, 8),
                            direction, 6f);
                        projectiles.Add(projectile);
                    }
                }
                else
                {
                    attackTimer = 0;
                }
            }

            if (enemyType != EnemyType.Flying && enemyType != EnemyType.Archer)
            {
                var gravity = 1200f;
                Velocity.Y += gravity * deltaTime;
                position.Y += Velocity.Y * deltaTime;
            }
            else if (enemyType == EnemyType.Flying)
            {
                if (player.Position.Y < position.Y - 15)
                    position.Y -= speed * 0.8f * deltaTime;
                else if (player.Position.Y > position.Y + 15)
                    position.Y += speed * 0.8f * deltaTime;

                var hoverOffset = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) * 3;
                position.Y += hoverOffset * deltaTime;
            }
            else if (enemyType == EnemyType.Archer)
            {
                var hoverOffset = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3) * 5;
                position.Y += hoverOffset * deltaTime;
            }

            if (enemyType != EnemyType.Flying && enemyType != EnemyType.Archer)
            {
                foreach (var platform in platforms)
                {
                    if (Bounds.Intersects(platform.Bounds))
                    {
                        if (Velocity.Y > 0 && position.Y + Bounds.Height > platform.Bounds.Y)
                        {
                            position.Y = platform.Bounds.Y - Bounds.Height;
                            Velocity.Y = 0;
                        }
                    }
                }
            }

            foreach (Enemy other in allEnemies)
            {
                if (other != this && Bounds.Intersects(other.Bounds))
                {
                    if (Bounds.Center.X < other.Bounds.Center.X)
                        position.X -= speed * deltaTime;
                    else
                        position.X += speed * deltaTime;
                }
            }

            Bounds = new Rectangle((int)position.X, (int)position.Y, Bounds.Width, Bounds.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color enemyColor;
            switch (enemyType)
            {
                case EnemyType.Slow:
                    enemyColor = isWounded ? Color.DarkCyan : Color.Cyan;
                    break;
                case EnemyType.Fast:
                    enemyColor = isWounded ? Color.DarkOrange : Color.Orange;
                    break;
                case EnemyType.Flying:
                    enemyColor = isWounded ? Color.DarkViolet : Color.Purple;
                    break;
                case EnemyType.Archer:
                    enemyColor = isWounded ? Color.DarkGreen : Color.LimeGreen;
                    break;
                default:
                    enemyColor = Color.White;
                    break;
            }

            spriteBatch.Draw(texture, Bounds, enemyColor);

            var barWidth = Bounds.Width;
            var barHeight = 5;
            var barX = Bounds.X;
            var barY = Bounds.Y - 8;

            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkRed);

            var currentHealthPercent = (int)((float)Health / MaxHealth * barWidth);
            var healthColor = isWounded ? Color.Orange : Color.Red;
            spriteBatch.Draw(pixel, new Rectangle(barX, barY, currentHealthPercent, barHeight), healthColor);

            foreach (var projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }

    public class Projectile
    {
        public Rectangle Bounds;
        private int direction;
        private float speed;
        public bool IsActive = true;

        public Projectile(Rectangle bounds, int direction, float speed)
        {
            this.Bounds = bounds;
            this.direction = direction;
            this.speed = speed;
        }

        public void Update(float deltaTime)
        {
            Bounds = new Rectangle(Bounds.X + (int)(direction * speed * deltaTime), Bounds.Y, Bounds.Width, Bounds.Height);

            if (Bounds.X < -200 || Bounds.X > 1500)
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