namespace Assets.Scripts.Managers
{
    public class GameConfig
    {
        private bool _tournamentMode = false;
        public bool TeachAI = true;
        public bool UseGhostAI = true;

        public bool TournamentMode
        {
            get
            {
                return _tournamentMode;
            }
            set
            {
                TeachAI = !value;
                UseGhostAI = !value;
                _tournamentMode = value;
            }
        }
    }
}