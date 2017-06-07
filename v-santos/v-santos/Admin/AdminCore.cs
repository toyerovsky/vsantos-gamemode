using GTANetworkServer;

namespace Serverside.Admin
{
    public class AdminCore
    {
        public delegate void PlayerWarnHandler(Client player, string reason);
        public delegate void PlayerKickHandler(Client player, string reason);
        public delegate void PlayerBanHandler(Client player, string reason);
        public delegate void PlayerAdminJailHandler(Client player, string reason);
        public delegate void PlayerAdminJailResetHandler(Client player, string reason);

        public event PlayerWarnHandler OnPlayerWarned;
        public event PlayerKickHandler OnPlayerKicked;
        public event PlayerBanHandler OnPlayerBanned;
        public event PlayerAdminJailHandler OnPlayerAj;
        public event PlayerAdminJailResetHandler OnPlayerAjReset;

        public void WarnPlayer(Client player, string reason)
        {
            if (OnPlayerWarned != null) OnPlayerWarned.Invoke(player, reason);
        }

        public void KickPlayer(Client player, string reason)
        {
            player.kick(reason);
            if (OnPlayerKicked != null) OnPlayerKicked.Invoke(player, reason);
        }

        public void BanPlayer(Client player, string reason)
        {
            player.ban(reason);
            if (OnPlayerBanned != null) OnPlayerBanned.Invoke(player, reason);
        }

        public void SetPlayerAdminJail(Client player, string reason)
        {
            //TODO Zrobienie AJ
            if (OnPlayerAj != null) OnPlayerAj.Invoke(player, reason);
        }

        public void ResetPlayerAdminJail(Client player, string reason)
        {
            if (OnPlayerAjReset != null) OnPlayerAjReset.Invoke(player, reason);
        }
    }
}