using System;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Serverside.Core.Animations.Models;
using Serverside.Core.Extensions;

namespace Serverside.Core.Animations
{
    public class RPAnimations : Script
    {
        public RPAnimations()
        {
            API.onResourceStart += OnResourceStartHandler;
            API.onClientEventTrigger += OnClientEventTriggerHandler;
        }

        private void OnClientEventTriggerHandler(Client sender, string eventName, params object[] arguments)
        {
            //args[0] Polska nazwa animacji
            //args[1] Słownik animacji z GTA:N
            //args[2] Nazwa animacji z GTA:N
            if (eventName == "OnPlayerAddAnim")
            {
                XmlHelper.AddXmlObject(new Animation
                {
                    Name = arguments[0].ToString(),
                    AnimDictionary = arguments[1].ToString(),
                    AnimName = arguments[2].ToString(),
                }, Constant.ConstantAssemblyInfo.XmlDirectory + @"Animations\", arguments[2].ToString());

                sender.Notify("Animacja została dodana pomyślnie.");
            }
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPAnimations] Uruchomione pomyślnie", ConsoleColor.DarkMagenta);
        }

        #region Komendy administracji
        [Command("dodajanim")]
        public void AddAnim(Client player)
        {
            //Wyświetlamy menu w którym administrator może grupować animacje
            //Przesyłamy dostępne animacje
            player.triggerEvent("ShowAdminAnimMenu", Constant.ConstantItems.Animations.Select(anim => $"{anim.Key},{anim.Value}").ToList());
        }
        #endregion

    }
}