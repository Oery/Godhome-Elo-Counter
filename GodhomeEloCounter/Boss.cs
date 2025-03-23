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
        public double peakElo;

        public int RoundedElo() => (int)Math.Round(elo);
        public int RoundedLastElo() => (int)Math.Round(lastElo);
        public int RoundedPeakElo() => (int)Math.Round(peakElo);

		public int streak;
		public int bestWinStreak;

		public int wins;
		public int losses;
        public string matchHistory = "";

		public TimeSpan timeSpent;

        public void Update(bool hasWon, float duration) {
			UpdateTime(duration);
			UpdateWins(hasWon);
			UpdateStreak(hasWon);
			UpdateELO(hasWon);
            UpdateMatchHistory(hasWon);
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

            if (elo > peakElo) { peakElo = elo; }
		}

		private void UpdateTime(float duration) { timeSpent += TimeSpan.FromSeconds(duration); }

        private void UpdateMatchHistory(bool hasWon) 
		{
            if (matchHistory.Length >= 15) matchHistory = matchHistory.Substring(2);
            if (matchHistory.Length != 0) matchHistory += " ";
            if (hasWon) matchHistory += "W";
            else matchHistory += "L";
        }
	}
}

