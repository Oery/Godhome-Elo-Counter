using System;

namespace GodhomeEloCounter
{
	[Serializable]
	public class Boss
	{
		public string sceneName;

		public int elo;

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
			if (hasWon) { elo += 1; }
			else { elo -= 1; }
		}

		private void UpdateTime(TimeSpan timeSpan) { timeSpent += timeSpan; }
	}
}

