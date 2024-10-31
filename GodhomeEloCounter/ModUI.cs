using MagicUI.Core;
using MagicUI.Elements;

namespace GodhomeEloCounter
{
    public static class ModUI
    {
        public static void SpawnBossUI(LayoutRoot layout, Boss boss) {
            string sceneName = boss.sceneName;
            string eloString = $"Elo: {boss.elo} ({boss.bestWinStreak})";
            string streakString = $"Streak: {boss.streak} / Best: {boss.bestWinStreak}";
            string winLossString = $"Wins: {boss.wins} / Losses: {boss.losses}";
            string timeString = $"Time: {boss.timeSpent:hh\\:mm\\:ss}";

            string textUI = $"{sceneName}\n{eloString}\n{streakString}\n{winLossString}\n{timeString}";

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = UI.TrajanBold,
                Text = textUI,
                Padding = new(20)
            };
        }
    }
}
