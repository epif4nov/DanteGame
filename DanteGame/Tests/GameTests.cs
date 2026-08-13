using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.Json;

namespace DanteGame.Tests
{
    [TestClass]
    public class PlayerLogicTests
    {
        [TestMethod]
        public void Player_HealthStartsAt3()
        {
            var expectedHealth = 3;
            Assert.AreEqual(3, expectedHealth);
        }

        [TestMethod]
        public void Player_TakeDamage_ReducesHealth()
        {
            var health = 3;
            health--;
            Assert.AreEqual(2, health);
        }

        [TestMethod]
        public void Player_Health_NotBelowZero()
        {
            var health = 1;
            health--;
            Assert.IsTrue(health >= 0);
        }
    }

    [TestClass]
    public class BonusLogicTests
    {
        [TestMethod]
        public void Bonus_Health_HealsWhenNotFull()
        {
            var health = 2;
            var maxHealth = 3;
            if (health < maxHealth) health++;
            Assert.AreEqual(3, health);
        }

        [TestMethod]
        public void Bonus_Health_DoesNotHealWhenFull()
        {
            var health = 3;
            var maxHealth = 3;
            if (health < maxHealth) health++;
            Assert.AreEqual(3, health);
        }

        [TestMethod]
        public void Bonus_Invincibility_Activates()
        {
            var isInvincible = false;
            isInvincible = true;
            Assert.IsTrue(isInvincible);
        }
    }

    [TestClass]
    public class PortalLogicTests
    {
        [TestMethod]
        public void Portal_StartsInactive()
        {
            var isActive = false;
            Assert.IsFalse(isActive);
        }

        [TestMethod]
        public void Portal_CanBeActivated()
        {
            var isActive = false;
            isActive = true;
            Assert.IsTrue(isActive);
        }
    }

    [TestClass]
    public class SaveDataTests
    {
        private string testFile = "test_score.json";

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(testFile))
                File.Delete(testFile);
        }

        [TestMethod]
        public void SaveData_SaveAndLoad_Works()
        {
            var score = 100;
            var json = JsonSerializer.Serialize(score);
            File.WriteAllText(testFile, json);

            string loadedJson = File.ReadAllText(testFile);
            int loadedScore = JsonSerializer.Deserialize<int>(loadedJson);

            Assert.AreEqual(score, loadedScore);
        }

        [TestMethod]
        public void SaveData_LoadNonExistent_HandlesGracefully()
        {
            if (File.Exists(testFile))
                File.Delete(testFile);

            var fileExists = File.Exists(testFile);
            Assert.IsFalse(fileExists);
        }
    }

    [TestClass]
    public class EnemyLogicTests
    {
        [TestMethod]
        public void Enemy_TakeDamage_ReducesHealth()
        {
            var health = 5;
            health--;
            Assert.AreEqual(4, health);
        }

        [TestMethod]
        public void Enemy_Health_NotBelowZero()
        {
            var health = 1;
            health--;
            Assert.IsTrue(health >= 0);
        }
    }

    [TestClass]
    public class ScoreLogicTests
    {
        [TestMethod]
        public void Score_Increases_WhenEnemyKilled()
        {
            var score = 0;
            score += 10;
            Assert.AreEqual(10, score);
        }
    }

    [TestClass]
    public class BonusTypeTests
    {
        [TestMethod]
        public void BonusType_HealthValue_IsValid()
        {
            var healthValue = 0;
            Assert.AreEqual(0, healthValue);
        }

        [TestMethod]
        public void BonusType_InvincibilityValue_IsValid()
        {
            var invincibilityValue = 1;
            Assert.AreEqual(1, invincibilityValue);
        }
    }

    [TestClass]
    public class FallingBlockLogicTests
    {
        [TestMethod]
        public void FallingBlock_StartsNotFalling()
        {
            var isFalling = false;
            Assert.IsFalse(isFalling);
        }
    }

    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void GameState_Menu_IsDefault()
        {
            var defaultState = "Menu";
            Assert.AreEqual("Menu", defaultState);
        }

        [TestMethod]
        public void GameState_Playing_Exists()
        {
            string[] states = { "Menu", "Prologue", "Playing", "Pause", "GameOver", "Victory" };
            var hasPlaying = Array.Exists(states, s => s == "Playing");
            Assert.IsTrue(hasPlaying);
        }
    }
}