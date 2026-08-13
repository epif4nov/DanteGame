using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DanteGame.Model
{
    public class Player
    {
        public Vector2 Position;
        public int Width = 64;
        public int Height = 128;

        private Vector2 drawOffset;

        public Vector2 Velocity;
        private bool isGrounded;

        private float speed = 450f;
        private float jumpPower = -850f;

        private Texture2D texture;

        private KeyboardState currentKeyState;
        private KeyboardState previousKeyState;

        private SpriteEffects spriteEffect = SpriteEffects.None;

        private float jumpBuffer = 0f;

        public int Health = 3;
        private int maxHealth = 3;
        private float invincibilityTimer = 0f;

        private bool isAttacking = false;
        private float attackCooldown = 0f;
        private float attackTimer = 0f;

        private bool showAttackEffect = false;
        private float attackEffectTimer = 0f;
        private float attackEffectDuration = 0.12f;
        private float attackCooldownDuration = 0.45f;

        public float RespawnInvincibility = 0f;

        public bool IsAttacking => isAttacking;

        public int Score = 0;

        private bool wasGrounded = false;

        private float landSoundCooldown = 0f;

        private bool wasInAir = false;

        private bool attackSoundPlayed = false;

        public int BestScore = 0;

        public bool AttackHitDone { get; set; } = false;

        private float attackAnimationProgress = 0f;
        private float attackAnimationSpeed = 10f;

        private float invincibilityPowerUpTimer = 0f;

        private bool isInvincibleFromBonus = false;

        public float InvincibilityTimer => invincibilityPowerUpTimer > 0 ? invincibilityPowerUpTimer : invincibilityTimer;


        public void ActivateInvincibility(float duration)
        {
            isInvincibleFromBonus = true;
            invincibilityPowerUpTimer = duration;
            invincibilityTimer = duration;
        }


        public Player(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            Position = startPosition;
            Velocity = Vector2.Zero;
            drawOffset = new Vector2(-5, 8);
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, List<Platform> platforms, List<Enemy> enemies)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();

            if (RespawnInvincibility > 0)
                RespawnInvincibility -= deltaTime;

            if (invincibilityTimer > 0)
                invincibilityTimer -= deltaTime;

            var attackPressed = currentKeyState.IsKeyDown(Keys.E) || currentKeyState.IsKeyDown(Keys.J);

            if (attackPressed && attackCooldown <= 0 && !isAttacking)
            {
                isAttacking = true;
                showAttackEffect = true;
                attackEffectTimer = attackEffectDuration;
                attackTimer = attackEffectDuration;
                attackCooldown = attackCooldownDuration;
                attackSoundPlayed = false;
                AttackHitDone = false;
                attackAnimationProgress = 0f;
            }

            if (isAttacking)
            {
                attackAnimationProgress += deltaTime * attackAnimationSpeed;
                if (attackAnimationProgress >= 1f)
                {
                    attackAnimationProgress = 1f;
                    isAttacking = false;
                    showAttackEffect = false;
                }
            }
            else
            {
                attackAnimationProgress = 0f;
            }

            if (isAttacking && !attackSoundPlayed)
            {
                SoundManager.AttackSound?.Play();
                attackSoundPlayed = true;
            }

            if (showAttackEffect)
            {
                attackEffectTimer -= deltaTime;
                if (attackEffectTimer <= 0)
                {
                    showAttackEffect = false;
                }
            }

            if (attackTimer > 0)
            {
                attackTimer -= deltaTime;
                if (attackTimer <= 0)
                {
                    isAttacking = false;
                }
            }

            if (attackCooldown > 0)
            {
                attackCooldown -= deltaTime;
            }

            var moveDirection = 0;
            if (currentKeyState.IsKeyDown(Keys.A) || currentKeyState.IsKeyDown(Keys.Left))
                moveDirection = -1;
            if (currentKeyState.IsKeyDown(Keys.D) || currentKeyState.IsKeyDown(Keys.Right))
                moveDirection = 1;

            if (moveDirection < 0)
                spriteEffect = SpriteEffects.FlipHorizontally;
            else if (moveDirection > 0)
                spriteEffect = SpriteEffects.None;

            Velocity.X = moveDirection * speed;

            if (currentKeyState.IsKeyDown(Keys.Space))
                jumpBuffer = 0.15f;
            else
                jumpBuffer -= deltaTime;

            if (jumpBuffer > 0 && isGrounded)
            {
                Velocity.Y = jumpPower;
                isGrounded = false;
                jumpBuffer = 0;
                wasInAir = true;
            }

            var gravity = 1800f;
            Velocity.Y += gravity * deltaTime;

            Position += Velocity * deltaTime;

            if (Position.X < 0) Position.X = 0;
            if (Position.X + Width > graphicsDevice.Viewport.Width)
                Position.X = graphicsDevice.Viewport.Width - Width;

            if (Position.Y + Height > graphicsDevice.Viewport.Height)
            {
                Health--;
                Position = new Vector2(110, 600 - Height);
                Velocity = Vector2.Zero;
            }

            ApplyPlatformCollision(platforms);

            if (landSoundCooldown > 0)
                landSoundCooldown -= deltaTime;

            if (wasInAir && isGrounded)
            {
                SoundManager.LandSound?.Play();
                wasInAir = false;
            }

            if (invincibilityPowerUpTimer > 0)
            {
                invincibilityPowerUpTimer -= deltaTime;
                if (invincibilityPowerUpTimer <= 0)
                {
                    isInvincibleFromBonus = false;
                }
            }

            CheckEnemyCollision(enemies);
        }

        public void CheckEnemyCollision(List<Enemy> enemies)
        {
            foreach (var enemy in enemies)
            {
                if (GetBounds().Intersects(enemy.Bounds) && invincibilityTimer <= 0)
                {
                    Health--;
                    invincibilityTimer = 1f;
                    RespawnInvincibility = 1f;

                    if (Health <= 0)
                    {
                        Position = new Vector2(110, 600 - Height);
                        Velocity = Vector2.Zero;
                    }
                    else
                    {
                        Position = new Vector2(110, 600 - Height);
                        Velocity = Vector2.Zero;
                    }
                }
            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            invincibilityTimer = 1f;
            RespawnInvincibility = 1.5f;

            if (Health <= 0)
            {
                Position = new Vector2(110, 600 - Height);
                Velocity = Vector2.Zero;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var drawColor = Color.White;
            if (invincibilityTimer > 0)
            {
                var alpha = (float)(Math.Sin(invincibilityTimer * 30) * 0.5 + 0.5);
                drawColor = Color.White * alpha;
            }

            spriteBatch.Draw(texture,
                new Rectangle((int)(Position.X + drawOffset.X), (int)(Position.Y + drawOffset.Y), Width, Height),
                null,
                drawColor,
                0f,
                Vector2.Zero,
                spriteEffect,
                0f);

            if (isAttacking)
            {
                var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });

                var progress = attackAnimationProgress;
                var facingLeft = (spriteEffect == SpriteEffects.FlipHorizontally);

                var baseX = (int)Position.X;
                var baseY = (int)Position.Y + Height / 2;

                var offset = 20;
                var swordLength = (int)(progress * 60f);
                if (progress > 0.6f)
                    swordLength = (int)((1f - (progress - 0.6f) / 0.4f) * 60f);

                var swordX = baseX;
                var swordY = baseY - 4;

                if (facingLeft)
                    swordX -= (swordLength + offset);
                else
                    swordX += offset;

                spriteBatch.Draw(pixel, new Rectangle(swordX, swordY, swordLength, 6), null, Color.White * 0.95f, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                for (var i = 0; i < 3; i++)
                {
                    var trailProgress = progress - (i + 1) * 0.1f;
                    if (trailProgress > 0 && trailProgress < 0.8f)
                    {
                        var trailLength = (int)(trailProgress * 50f);
                        var trailX = baseX;
                        var trailY = baseY - 4 + (i - 1) * 4;

                        if (facingLeft)
                            trailX -= (trailLength + offset - i * 5);
                        else
                            trailX += (trailLength + offset - i * 5);

                        var alpha = (int)((1f - trailProgress) * 150);
                        spriteBatch.Draw(pixel, new Rectangle(trailX, trailY, trailLength, 2), new Color(255, 255, 255, alpha));
                    }
                }

                if (invincibilityPowerUpTimer > 0)
                {
                    var pulse = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10) * 0.3f + 0.7f;
                    drawColor = Color.White * pulse;
                }

                if (progress > 0.3f && progress < 0.8f)
                {
                    var sparkX = baseX;
                    var sparkY = baseY;

                    if (facingLeft)
                        sparkX -= 45;
                    else
                        sparkX += 45;

                    for (var i = 0; i < 4; i++)
                    {
                        var sparkAlpha = (int)((1f - Math.Abs(progress - 0.55f) * 6) * 200);
                        if (sparkAlpha > 0)
                        {
                            spriteBatch.Draw(pixel, new Rectangle(sparkX + i * 4, sparkY - 5 + i * 3, 2, 2), new Color(255, 255, 200, sparkAlpha));
                        }
                    }
                }
            }
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public void ApplyPlatformCollision(List<Platform> platforms)
        {
            var playerBounds = GetBounds();
            var grounded = false;

            foreach (var platform in platforms)
            {
                var platformBounds = platform.Bounds;

                if (playerBounds.Intersects(platformBounds))
                {
                    var intersection = Rectangle.Intersect(playerBounds, platformBounds);

                    if (intersection.Width < intersection.Height)
                    {
                        if (playerBounds.Center.X < platformBounds.Center.X)
                            Position.X = platformBounds.Left - Width;
                        else
                            Position.X = platformBounds.Right;

                        playerBounds = GetBounds();
                    }
                    else
                    {
                        if (playerBounds.Center.Y < platformBounds.Center.Y)
                        {
                            Position.Y = platformBounds.Top - Height;
                            Velocity.Y = 0;
                            grounded = true;
                        }
                        else
                        {
                            Position.Y = platformBounds.Bottom;
                            Velocity.Y = 0;
                        }
                        playerBounds = GetBounds();
                    }
                }
            }

            isGrounded = grounded;
        }

        public Rectangle GetAttackBounds()
        {
            var attackWidth = 65;
            var attackHeight = Height - 20;
            var attackX = (int)Position.X;
            var attackY = (int)Position.Y + 10;

            if (spriteEffect == SpriteEffects.FlipHorizontally)
                attackX -= attackWidth;
            else
                attackX += Width - 15;

            return new Rectangle(attackX, attackY, attackWidth, attackHeight);
        }

        public void DrawHealthBar(SpriteBatch spriteBatch, SpriteFont font)
        {
            var barWidth = Width;
            var barHeight = 8;
            var barX = (int)Position.X;
            var barY = (int)Position.Y - 15;

            var currentHealthPercent = (int)((float)Health / 3f * barWidth);

            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkRed);

            Color barColor;
            if (Health == 3)
                barColor = Color.LimeGreen;
            else if (Health == 2)
                barColor = Color.Yellow;
            else
                barColor = Color.Orange;

            spriteBatch.Draw(pixel, new Rectangle(barX, barY, currentHealthPercent, barHeight), barColor);
        }

        public void TakeDamageFromEnemy(int damage)
        {
            if (invincibilityTimer <= 0)
            {
                Health--;
                invincibilityTimer = 1f;
                RespawnInvincibility = 1f;
                SoundManager.HurtSound?.Play();

                Position = new Vector2(110, 600 - Height);
                Velocity = Vector2.Zero;
            }
        }
    }
}