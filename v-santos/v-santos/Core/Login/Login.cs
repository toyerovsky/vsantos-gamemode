using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GTANetworkServer;
using Serverside.Core;

namespace Serverside.Login
{
    class Login
    {
        private string Username { get; }
        private string Password { get; }
        private readonly API Api = new API();

        public Login(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public bool LogInPlayer(Client player)
        {
            string trueMd5Hash;
            if (RPCore.Db.GetPassword(Username, out trueMd5Hash))
            {
                string salt = RPCore.Db.GetSalt(Username);

                if (Api.getAllPlayers().Count > 1 && Api.getAllPlayers().Any(c => c.getData("ForumName") == Username))
                {
                    RPChat.SendMessageToPlayer(player, "Ktoś jest już obecnie zalogowany przy pomocy tej nazwy użytkownika.", ChatMessageType.ServerInfo);
                }
                else if (RPCore.Db.UserExist(Username) && trueMd5Hash.Equals(GenerateIpbHash(Password, salt)))
                {
                    player.setSyncedData("AccountID", RPCore.Db.GetAid(Username));
                    player.setData("AccountID", RPCore.Db.GetAid(Username));
                    RPChat.SendMessageToPlayer(player, "UID Twojego konta to: " + player.getData("AccountID"), ChatMessageType.ServerInfo);
                    return true;
                }
                else
                {
                    RPChat.SendMessageToPlayer(player, "Podano złe dane, spróbuj jeszcze raz.", ChatMessageType.ServerInfo);
                    return false;
                }
            }
            RPChat.SendMessageToPlayer(player, "Podano złe dane, spróbuj jeszcze raz.", ChatMessageType.ServerInfo);
            return false;
        }

        private static string GenerateIpbHash(string plaintext, string salt)
        {
            //return CalculateMD5Hash(CalculateMD5Hash(salt) + CalculateMD5Hash(plaintext));
            return BCrypt.Net.BCrypt.HashPassword(plaintext, "$2a$13$" + salt);
        }
    }
}