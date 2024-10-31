using System;
using System.Collections.Generic;
using System.Linq;

namespace GodhomeEloCounter {

    [Serializable]
    public class LocalData { 
        public List<Boss> bosses = new List<Boss>();

        public void UpdateBoss(string sceneName, bool hasWon, TimeSpan timeSpan) {
            foreach (Boss boss in bosses) {
                if (boss.sceneName == sceneName) {
                    boss.Update(hasWon, timeSpan);
                    return;
                }
            }
            bosses.Add(new Boss() { sceneName = sceneName, elo = 0, streak = 0, bestWinStreak = 0, wins = 0, losses = 0, timeSpent = TimeSpan.Zero });
            bosses.Last().Update(hasWon, timeSpan);
        }
    }
}
