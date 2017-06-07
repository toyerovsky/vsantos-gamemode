using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GTANetworkServer;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Core.Finders;
using Serverside.Groups;
using Serverside.Groups.Base;


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
        [Command("o", "~y~UŻYJ: ~w~ /o [id] [typ] [cena] [indeks]")]
        public void OfferItem(Client sender, string id, string offerType, string unsafeMoneyCount, string index = null)
        {
            int getterId;
            decimal safeMoneyCount;

            OfferType type;

            if (!Enum.TryParse(offerType, true, out type))
            {
                RPChat.SendMessageToPlayer(sender, "Podano nieprawidłowy typ oferty.", ChatMessageType.ServerInfo);
                return;
            }

            if (Validator.IsIntIdValid(id) && Validator.IsMoneyStringValid(unsafeMoneyCount))
            {
                getterId = Convert.ToInt32(id);
                safeMoneyCount = Convert.ToDecimal(unsafeMoneyCount);
            }
            else
            {
                sender.Notify("Podano dane w nieprawidłowym formacie.");
                return;
            }

            if (getterId.Equals(sender.GetAccountController().ServerId))
            {
                sender.Notify("Nie możesz oferować przedmiotu samemu sobie.");
                return;
            }

            Client getter;
            Offer offer = null;
            if (PlayerFinder.TryFindClientInRadiusOfClientByServerId(sender, getterId, 10, out getter))
            {
                if (type == OfferType.Przedmiot)
                {
                    int itemIndex;

                    if (Validator.IsIntIdValid(index))
                    {
                        itemIndex = Convert.ToInt32(index);
                    }
                    else
                    {
                        sender.Notify("Podano numer przedmiotu w nieprawidłowym formacie");
                        return;
                    }

                    var items = sender.GetAccountController().CharacterController.Character.Item.ToList();

                    //Tutaj sprawdzamy czy gracz posiada taki numer na liście. Numerujemy od 0 więc items.Count - 1
                    if (itemIndex > items.Count - 1)
                    {
                        sender.Notify("Nie posiadasz przedmiotu o takim indeksie.");
                        return;
                    }

                    var item = items[itemIndex];

                    if (item.CurrentlyInUse.HasValue && item.CurrentlyInUse.Value)
                    {
                        sender.Notify("Nie możesz używać przedmiotu podczas oferowania.");
                        return;
                    }

                    offer = new Offer(sender, getter, item, safeMoneyCount);
                    getter.SetData("Offer", offer);
                }
                else if (type == OfferType.Pojazd)
                {
                    VehicleController vehicle = RPEntityManager.GetVehicle(_api.getPlayerVehicle(sender));
                    if (vehicle == null) return;

                    offer = new Offer(sender, getter, vehicle.VehicleData, safeMoneyCount);
                    getter.SetData("Offer", offer);
                }
                //Tutaj są oferty wymagające uprawnień grupowych
                else if (type == OfferType.Dowod || type == OfferType.Prawko || type == OfferType.Naprawa)
                {
                    //Sprawdzamy uprawnienia potrzebne do danych ofert
                    if (type == OfferType.Dowod)
                    {
                        var group = sender.GetOnDutyGroup();
                        if (group == null) return;
                        if (group.Data.GroupType != GroupType.CityHall ||
                            !((CityHall) group).CanPlayerGiveIdCard(sender.GetAccountController()))
                        {
                            sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do wydawania dowodu osobistego.");
                            return;
                        }
                    }
                    else if (type == OfferType.Prawko)
                    {
                        var group = sender.GetOnDutyGroup();
                        if (group == null) return;
                        if (group.Data.GroupType != GroupType.CityHall || 
                            !((CityHall) group).CanPlayerGiveDrivingLicense(sender.GetAccountController()))
                        {
                            sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do wydawania prawa jazdy.");
                            return;
                        }
                            
                    }

                    offer = new Offer(sender, getter, safeMoneyCount, type);
                    getter.SetData("Offer", offer);
                }

                if (offer == null) return;
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
        #endregion
    }
}