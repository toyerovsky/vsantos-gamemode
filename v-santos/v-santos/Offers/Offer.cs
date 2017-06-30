using System;
using System.Collections.Generic;
using System.Data.Entity;
using GTANetworkServer;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Database;
using Serverside.Database.Models;
using Vehicle = Serverside.Database.Models.Vehicle;


namespace Serverside.Offers
{
    public class Offer : IDisposable
    {
        public Vehicle Vehicle { get; }
        public Item Item { get; }
        public Building Building { get; }

        public decimal Money { get; }

        //Ten co wysyłał oferte
        public Client Sender { get; }

        //Ten co odbiera oferte
        public Client Getter { get; }

        //Płaci gotówką albo kartą
        public bool Bank { get; set; }

        //Oferta z przedmiotem
        public Offer(Client sender, Client getter, Item item, decimal moneyCount)
        {
            Item = item;
            Money = moneyCount;
            Sender = sender;
            Getter = getter;
        }

        //Oferta z pojazdem
        public Offer(Client sender, Client getter, Vehicle vehicle, decimal moneyCount)
        {
            Vehicle = vehicle;
            Money = moneyCount;
            Sender = sender;
            Getter = getter;
        }

        //Oferta z budynkiem
        public Offer(Client sender, Client getter, Building building, decimal moneyCount)
        {
            Building = building;
            Money = moneyCount;
            Sender = sender;
            Getter = getter;
        }

        //Oferta bez niczego, np. taxi, naprawa, itp.
        public Offer(Client sender, Client getter, decimal moneyCount, Action<Client> action, bool moneyToGroup)
        {
            _action = action;
            Money = moneyCount;
            Sender = sender;
            Getter = getter;
            _moneyToGroup = moneyToGroup;
        }

        private Action<Client> _action;
        //Determinuje czy gotówka ma iść do kieszeni gracza czy do grupy
        private bool _moneyToGroup;

        public void Trade()
        {
            if (Getter.HasMoney(Money, Bank))
            {
                if (Item != null)
                {
                    RPChat.SendMessageToNearbyPlayers(Sender,
                        $"{Sender.GetAccountController().CharacterController.FormatName} podaje przedmiot {Getter.GetAccountController().CharacterController.FormatName}",
                        ChatMessageType.ServerMe);

                    Item.Character = Getter.GetAccountController().CharacterController.Character;
                    ContextFactory.Instance.Items.Attach(Item);
                    ContextFactory.Instance.Entry(Item).State = EntityState.Modified;
                    ContextFactory.Instance.SaveChanges();
                }
                else if (Vehicle != null)
                {
                    Vehicle.Character = Getter.GetAccountController().CharacterController.Character;
                    ContextFactory.Instance.Vehicles.Attach(Vehicle);
                    ContextFactory.Instance.Entry(Vehicle).State = EntityState.Modified;
                    ContextFactory.Instance.SaveChanges();
                }
                else if (Building != null)
                {
                    Building.Character = Getter.GetAccountController().CharacterController.Character;
                    ContextFactory.Instance.Buildings.Attach(Building);
                    ContextFactory.Instance.Entry(Building).State = EntityState.Modified;
                    ContextFactory.Instance.SaveChanges();
                }

                if (_moneyToGroup)
                {
                    Sender.GetAccountController().CharacterController.OnDutyGroup.AddMoney(Money);
                }
                else
                {
                    Sender.AddMoney(Money, Bank);
                }

                Getter.RemoveMoney(Money, Bank);

                _action?.Invoke(Getter);
            }
            else
            {
                Getter.Notify(Bank
                    ? "Nie posiadasz wystarczającej ilości środków na karcie"
                    : "Nie posiadasz wystarczającej ilości gotówki.");
                Sender.Notify("Wymiana zakończona niepowodzeniem.");
            }
        }

        public void ShowWindow(List<string> dataSource)
        {
            API.shared.triggerClientEvent(Getter, "ShowOfferCef", dataSource);
        }

        public void Dispose()
        {
            Getter.ResetData("Offer");
        }
    }
}
