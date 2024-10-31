using System;

namespace GodhomeEloCounter
{
	[Serializable]
	public class Boss(string sceneName, int tier)
    {
		public string sceneName = sceneName;

        public int tier = tier;

		public double elo;
        public double lastElo;

        public int RoundedElo() => (int)Math.Round(elo);
        public int RoundedLastElo() => (int)Math.Round(lastElo);

		public int streak;
		public int bestWinStreak;

		public int wins;
		public int losses;

		public TimeSpan timeSpent;

        public void Update(bool hasWon, TimeSpan timeSpan) {
			this.UpdateTime(timeSpan);
			this.UpdateWins(hasWon);
			this.UpdateStreak(hasWon);
			this.UpdateELO(hasWon);
		}

		private void UpdateWins(bool hasWon)
		{
			if (hasWon) { wins++; }
			else { losses++; }
		}

		private void UpdateStreak(bool hasWon)
		{
			if (hasWon)
			{
				if (streak > 0) { streak++; }
				else { streak = 1; }
			}

			else
			{
				if (streak < 0) { streak--; }
				else { streak = -1; }
			}

			if (streak > bestWinStreak) { bestWinStreak = streak; }
		}

		private void UpdateELO(bool hasWon)
		{
            const double difficulty = 1600.0;
            const double kFactor = 32.0;

            lastElo = elo;
            double expectedScore = 1.0 / (1.0 + Math.Pow(10.0, (difficulty - elo) / 400.0));

            double actualScore = hasWon ? 1.0 : 0.0;
            elo += kFactor * (actualScore - expectedScore);
		}

		private void UpdateTime(TimeSpan timeSpan) { timeSpent += timeSpan; }
	}
}

