using System;
using NUnit.Framework;
using TurtleDDay.Simulation.Match;

namespace TurtleDDay.Tests.EditMode.Simulation
{
    /// <summary>
    /// Tests unitaires de la règle de répartition des rôles.
    /// Convention de nommage : Méthode_Situation_RésultatAttendu.
    /// Structure de chaque test : Arrange (préparer) / Act (agir) / Assert (vérifier).
    /// </summary>
    public class RoleDistributionTests
    {
        [TestCase(2, 1, 1)]
        [TestCase(5, 1, 4)]   // cas nominal du GDD : 1 rapace pour 4 tortues
        [TestCase(9, 1, 8)]
        [TestCase(10, 2, 8)]  // 2 rapaces pour 8 tortues
        [TestCase(15, 3, 12)]
        public void For_ValidPlayerCount_ReturnsExpectedSplit(int players, int expectedRaptors, int expectedTurtles)
        {
            // Act
            RoleDistribution split = RoleDistribution.For(players);

            // Assert
            Assert.That(split.Raptors, Is.EqualTo(expectedRaptors));
            Assert.That(split.Turtles, Is.EqualTo(expectedTurtles));
        }

        [Test]
        public void For_AnyValidCount_TotalEqualsPlayerCountAndBothSidesExist()
        {
            // Test de propriété : on vérifie des invariants sur une plage de valeurs
            // plutôt que des exemples isolés.
            for (int players = RoleDistribution.MinPlayers; players <= 64; players++)
            {
                RoleDistribution split = RoleDistribution.For(players);

                Assert.That(split.Total, Is.EqualTo(players), $"players={players}");
                Assert.That(split.Raptors, Is.GreaterThanOrEqualTo(1), $"players={players}");
                // >= et non > : à 2 joueurs (1 contre 1), il y a autant de tortues que de rapaces.
                Assert.That(split.Turtles, Is.GreaterThanOrEqualTo(split.Raptors), $"players={players}");
            }
        }

        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-3)]
        public void For_TooFewPlayers_Throws(int players)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => RoleDistribution.For(players));
        }
    }
}
