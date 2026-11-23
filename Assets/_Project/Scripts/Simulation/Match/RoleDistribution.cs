using System;

namespace TurtleDDay.Simulation.Match
{
    /// <summary>
    /// Répartition des joueurs humains entre les deux camps d'une manche.
    /// Règle de design (GDD, Q20) : 1 rapace pour 4 tortues, soit 1 rapace par tranche de 5 joueurs,
    /// avec toujours au moins 1 rapace et au moins 1 tortue.
    /// </summary>
    /// <remarks>
    /// Type valeur immuable (readonly struct) : aucune allocation sur le tas, aucun état caché.
    /// L'assembly Simulation n'a pas accès à UnityEngine : cette logique est testable en millisecondes.
    /// </remarks>
    public readonly struct RoleDistribution : IEquatable<RoleDistribution>
    {
        /// <summary>Nombre minimum de joueurs pour lancer une manche (1 rapace contre 1 tortue).</summary>
        public const int MinPlayers = 2;

        /// <summary>Un rapace supplémentaire par tranche de <c>PlayersPerRaptor</c> joueurs.</summary>
        public const int PlayersPerRaptor = 5;

        public int Raptors { get; }
        public int Turtles { get; }
        public int Total => Raptors + Turtles;

        private RoleDistribution(int raptors, int turtles)
        {
            Raptors = raptors;
            Turtles = turtles;
        }

        /// <summary>Calcule la répartition pour un nombre de joueurs donné.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="playerCount"/> &lt; <see cref="MinPlayers"/>.</exception>
        public static RoleDistribution For(int playerCount)
        {
            if (playerCount < MinPlayers)
            {
                throw new ArgumentOutOfRangeException(nameof(playerCount), playerCount,
                    $"Une manche nécessite au moins {MinPlayers} joueurs.");
            }

            int raptors = Math.Max(1, playerCount / PlayersPerRaptor);
            return new RoleDistribution(raptors, playerCount - raptors);
        }

        public bool Equals(RoleDistribution other) => Raptors == other.Raptors && Turtles == other.Turtles;
        public override bool Equals(object obj) => obj is RoleDistribution other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Raptors, Turtles);
        public override string ToString() => $"{Raptors} rapace(s) / {Turtles} tortue(s)";
    }
}
