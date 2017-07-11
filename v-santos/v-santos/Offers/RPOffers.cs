using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Groups;
using Serverside.Groups.Base;
using Serverside.Groups.Enums;


namespace Serverside.Offers
{
    public class RPOffers : Script
    {
        private readonly API _api = new API();

        public RPOffers()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPOffers] zostało uruchomione pomyślnie!", ConsoleColor.DarkMagenta);
        }

        private void API_onClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "OnPlayerCancelOffer")
            {
                Offer offer = sender.GetData("Offer");

                offer.Sender.Notify($"Gracz {offer.Getter.GetAccountController().CharacterController.FormatName} odrzucił twoją ofertę.");
                offer.Getter.Notify($"Odrzuciłeś ofertę gracza { offer.Sender.GetAccountController().CharacterController.FormatName}");
                offer.Dispose();
            }
            else if (eventName == "OnPlayerPayOffer")
            {
                Offer offer = sender.GetData("Offer");

                if (offer.Sender.position.DistanceTo(offer.Getter.position) <= 10)
                {
                    //Trzeba nadać to pole przed wykonaniem oferty
                    offer.Bank = Convert.ToBoolean(arguments[0]);
                    offer.Trade();
                }
                else
                {
                    offer.Sender.Notify("Osoba do której wysyłasz ofertę znajduje się za daleko.");
                    offer.Getter.Notify("Znajdujesz się za daleko od osoby która wysłała ofertę.");
                }
                offer.Dispose();
            }
        }

        #region Komendy
        [Command("o", "~y~UŻYJ: ~w~ /o [id] [typ] [cena] (indeks)")]
        public void OfferItem(Client sender, int id, OfferType type, decimal safeMoneyCount, int index = -1)
        {
            if (id.Equals(sender.GetAccountController().ServerId))
            {
                sender.Notify("Nie możesz oferować przedmiotu samemu sobie.");
                return;
            }

            Offer offer = null;
            if (API.getPlayersInRadiusOfPlayer(6f, sender).Any(x => x.GetAccountController().ServerId == id))
            {
                Client getter = API.getPlayersInRadiusOfPlayer(6f, sender).Find(x => x.GetAccountController().ServerId == id);
                if (type == OfferType.Przedmiot && index != -1)
                {
                    var items = sender.GetAccountController().CharacterController.Character.Items.ToList();

                    //Tutaj sprawdzamy czy gracz posiada taki numer na liście. Numerujemy od 0 więc items.Count - 1
                    if (index > items.Count - 1 || index < 0)
                    {
                        sender.Notify("Nie posiadasz przedmiotu o takim indeksie.");
                        return;
                    }

                    var item = items[index];

                    if (item.CurrentlyInUse.HasValue && item.CurrentlyInUse.Value)
                    {
                        sender.Notify("Nie możesz używać przedmiotu podczas oferowania.");
                        return;
                    }

                    offer = new Offer(sender, getter, item, safeMoneyCount);
                }
                else if (type == OfferType.Pojazd)
                {
                    VehicleController vehicle = RPEntityManager.GetVehicle(_api.getPlayerVehicle(sender));
                    if (vehicle == null) return;

                    offer = new Offer(sender, getter, vehicle.VehicleData, safeMoneyCount);
                }
                else if (type == OfferType.Budynek)
                {
                    if (sender.GetAccountController().CharacterController.CurrentBuilding != null || sender.HasData("CurrentDoors"))
                    {
                        BuildingController building = sender.HasData("CurrentDoors")
                            ? sender.GetData("CurrentDoors")
                            : sender.GetAccountController().CharacterController.CurrentBuilding;

                        if (building.BuildingData.Group != null)
                        {
                            sender.Notify("Nie można sprzedać budynku przepisanego pod grupę.");
                            return;
                        }

                        if (building.BuildingData.Character.Id != sender.GetAccountController().CharacterController.Character
                                .Id)
                        {
                            sender.Notify("Nie jesteś właścicielem tego budynku.");
                            return;
                        }

                        offer = new Offer(sender, getter, building.BuildingData, safeMoneyCount);
                    }
                    else
                    {
                        sender.Notify("Aby oferować budynek musisz znajdować się w markerze bądź środku budynku");
                    }
                }
                //Tutaj są oferty wymagające uprawnień grupowych
                else if (type == OfferType.Dowod)
                {
                    var group = sender.GetAccountController().CharacterController.OnDutyGroup;
                    if (group == null) return;
                    if (group.GroupData.GroupType != GroupType.Urzad || !((CityHall)group).CanPlayerGiveIdCard(sender.GetAccountController()))
                    {
                        sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do wydawania dowodu osobistego.");
                        return;
                    }
                    offer = new Offer(sender, getter, safeMoneyCount, c => OfferActions.GiveIdCard(getter), true);
                }
                else if (type == OfferType.Prawko)
                {
                    var group = sender.GetAccountController().CharacterController.OnDutyGroup;
                    if (group == null) return;
                    if (group.GroupData.GroupType != GroupType.Urzad || !((CityHall)group).CanPlayerGiveDrivingLicense(sender.GetAccountController()))
                    {
                        sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do wydawania prawa jazdy.");
                        return;
                    }
                    offer = new Offer(sender, getter, safeMoneyCount, c => OfferActions.GiveDrivingLicense(getter), true);
                }

                if (offer != null) getter.SetData("Offer", offer);
            }

            if (offer != null)
            {
                List<string> cefList = new List<string>
                {
                    sender.name,
                    type.ToString(),
                    offer.Money.ToString(CultureInfo.InvariantCulture)
                };
                sender.Notify("Twoja oferta została wysłana.");
                offer.ShowWindow(cefList);
            }
        }
    }
    #endregion
}