using System;
using System.Timers;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using Serverside.Core.Extensions;

//using Serverside.Core.Finders;


namespace Serverside.Core
{
    public sealed class RPBw : Script
    {
        enum WeaponType //czas w minutach do odrodzenia po smierci z danego rodzaju broni
        {
            Piesc = 4,
            BronBiala = 8,
            Reszta = 20
        }

        public RPBw()
        {
            API.onResourceStart += API_onResourceStart;
            API.onPlayerDeath += PlayerDeathHandler;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPBw] Uruchomione pomyslnie!", ConsoleColor.DarkMagenta);
        }

        #region Komendy

        [Command("akceptujsmierc")]
        public void CharacterKill(Client sender)
        {
            var player = sender.GetAccountController();
            player.CharacterController.Character.IsAlive = false;
            player.Save();
            sender.kick("CK");
        }

        [Command("bw", "~y~UŻYJ: ~w~ /bw [id]")]
        public void SetPlayerBw(Client sender, int id)
        {
            if (RPEntityManager.GetAccountByServerId(id) != null)
            {
                Client getter = RPEntityManager.GetAccountByServerId(id).Client;
                getter.triggerEvent("ToggleHud", true);
                getter.ResetData("CharacterBW");
            }
            else
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
            }
        }
        #endregion

        //Odliczanie czasu do odrodzenia
        private void PlayerDeathHandler(Client sender, NetHandle reason, int weapon) 
        {
            if (sender.hasData("CharacterBW")) return;
            var player = sender.GetAccountController();
            
            var playerCharacter = player.CharacterController.Character;

            //tutaj sprawdzam czy gracz ktory aktualnie umarl nie mial juz wczesniej BWState (np. przez to ze wlasnie sie zalogowal i ma BW w bazie)
            DateTime reviveDate = DateTime.Now.AddMinutes(playerCharacter.BWState > 0 ? playerCharacter.BWState : GetTimeToRespawn(weapon));

            //API.shared.sendNativeToPlayer(sender, Hash._RESET_LOCALPLAYER_STATE, sender.handle);
            //API.shared.sendNativeToPlayer(sender, Hash.RESET_PLAYER_ARREST_STATE, sender.handle);
            //API.shared.sendNativeToPlayer(sender, Hash.IGNORE_NEXT_RESTART, true);
            API.shared.sendNativeToPlayer(sender, Hash._DISABLE_AUTOMATIC_RESPAWN, true);
            //API.shared.sendNativeToPlayer(sender, Hash.SET_FADE_IN_AFTER_DEATH_ARREST, true);
            API.shared.sendNativeToPlayer(sender, Hash.SET_FADE_OUT_AFTER_DEATH, false);
            //API.shared.sendNativeToPlayer(sender, Hash.NETWORK_REQUEST_CONTROL_OF_ENTITY, sender.handle);

            API.shared.triggerClientEvent(sender, "ToggleHud", false);

            RPChat.SendMessageToPlayer(sender, "Zostałeś brutalnie zraniony, aby uśmiercić swoją postać wpisz: /akceptujsmierc", ChatMessageType.ServerInfo);

            sender.setData("CanTalk", false);
            sender.setData("CharacterBW", GetTimeToRespawn(weapon));

            Timer timer = new Timer(1000);
            timer.Start();

            API.onPlayerDisconnected += (client, s) =>
            {
                if (sender == client) timer.Dispose();
            };

            timer.Elapsed += (s, e) =>
            {
                if (sender.IsNull || !sender.exists || !playerCharacter.IsAlive)
                {
                    timer.Dispose();
                }

                //Zdejmowanie BW
                if (!sender.hasData("CharacterBW"))
                {
                    playerCharacter.BWState = 0;
                    playerCharacter.HitPoints = 20;
                    API.setPlayerHealth(sender, 20);

                    API.shared.sendNativeToPlayer(sender, Hash.NETWORK_RESURRECT_LOCAL_PLAYER, sender.position.X, sender.position.Y, sender.position.Z, sender.rotation.Z, false, false);
                    API.shared.sendNativeToPlayer(sender, Hash.RESURRECT_PED, sender.handle);
                    API.shared.triggerClientEvent(sender, "ToggleHud", true);
                    API.shared.freezePlayer(sender, false);

                    RPChat.SendMessageToPlayer(sender, "Twoje BW zostało anulowane.", ChatMessageType.ServerInfo);

                    sender.setData("CanTalk", true);
                    timer.Dispose();
                }
                //Odliczanie
                else if (reviveDate.CompareTo(DateTime.Now) > 0)
                {
                    //playerCharacter.HitPoits = 1;
                    //API.shared.setPlayerHealth(sender, 1);
                    if (DateTime.Now.Second == 0) sender.setData("CharacterBW", (reviveDate - DateTime.Now).Minutes);

                    string secondsToShow = (reviveDate - DateTime.Now).Seconds.ToString();
                    if (secondsToShow.Length == 1) secondsToShow = "0" + secondsToShow;
                    API.shared.triggerClientEvent(sender, "BWTimerTick", String.Format("{0}:{1}", (reviveDate - DateTime.Now).Minutes, secondsToShow));
                }
                //Koniec BW
                else
                {
                    API.shared.sendNativeToPlayer(sender, Hash.NETWORK_RESURRECT_LOCAL_PLAYER, sender.position.X, sender.position.Y, sender.position.Z, sender.rotation.Z, false, false);
                    API.shared.sendNativeToPlayer(sender, Hash.RESURRECT_PED, sender.handle);
                    //API.shared.sendNativeToPlayer(sender, Hash.NETWORK_REQUEST_CONTROL_OF_ENTITY, sender.handle);

                    API.shared.triggerClientEvent(sender, "ToggleHud", true);

                    //update sender BWState = false, health = 20
                    playerCharacter.BWState = 0;
                    playerCharacter.HitPoints = 20;
                    API.shared.setPlayerHealth(sender, 20);

                    API.shared.freezePlayer(sender, false);
                    sender.setData("CanTalk", true);
                    sender.resetData("CharacterBW");

                    timer.Dispose();
                }
                player.CharacterController.Save();
            };
        }


        private int GetTimeToRespawn(int weapon) //metoda oblicza ile minut bedzie trwal respawn po smierci z danej broni na podst enuma
        {

            if (weapon == ((int)WeaponHash.Unarmed))
            {
                return (int)WeaponType.Piesc;
            }
            if (weapon == ((int)WeaponHash.Ball))
            {
                return (int)WeaponType.Piesc;
            }
            if (weapon == ((int)WeaponHash.Snowball))
            {
                return (int)WeaponType.Piesc;
            }
            if (weapon == ((int)WeaponHash.Golfclub))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.Hammer))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.Nightstick))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.Crowbar))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.Dagger))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.Bat))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.Knife))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.KnuckleDuster))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.Machete))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.SwitchBlade))
            {
                return (int)WeaponType.BronBiala;
            }
            if (weapon == ((int)WeaponHash.Bottle))
            {
                return (int)WeaponType.BronBiala;
            }
            return (int)WeaponType.Reszta;
        }
    }
}
