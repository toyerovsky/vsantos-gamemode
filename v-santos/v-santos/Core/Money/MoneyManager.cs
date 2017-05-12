using System;
using GTANetworkServer;
using Serverside.Core.Finders;
using Serverside.Database;

namespace Serverside.Core.Money
{
    public class MoneyChangedEventArgs : EventArgs
    {
        public Client Player { get; }

        public MoneyChangedEventArgs(Client player)
        {
            Player = player;
        }
    }
    
    public class MoneyManager
    {
        public delegate void MoneyChangedEventHandler(object sender, MoneyChangedEventArgs e);

        public event MoneyChangedEventHandler MoneyChanged;

        API api = new API();

        public MoneyManager()
        {
            MoneyChanged += RPMoney_MoneyChanged;
        }

        private void RPMoney_MoneyChanged(object sender, MoneyChangedEventArgs e)
        {
            CharacterEditor givingCharacterEditor =
                RPCore.Db.SelectCharacter(
                    Convert.ToInt64(api.getEntityData(e.Player.handle, "CharacterID")));
            //tu sie wysyla event, zeby narysopwalo graczowi stan jego gotowki
            api.triggerClientEvent(e.Player, "Money_Changed", String.Format(@"${0}", givingCharacterEditor.Money));
        }

        public bool CanPay(long cid, decimal count, bool bank = false)
        {
            CharacterEditor editor = RPCore.Db.SelectCharacter(cid);
            if (bank)
            {
                return editor.BankMoney >= count;
            }
            return editor.Money >= count;
        }

        public void AddMoney(long cid, decimal count, bool bank = false)
        {
            CharacterEditor character = RPCore.Db.SelectCharacter(cid);
            if (bank)
            {
                character.BankMoney += count;
            }
            else
            {
                character.Money += count;
            }
            RPCore.Db.UpdateCharacter(character);
            MoneyChangedEventArgs e = new MoneyChangedEventArgs(PlayerFinder.FindClientByCid(cid));
            if (MoneyChanged != null) MoneyChanged.Invoke(this, e);
        }

        public void RemoveMoney(long cid, decimal count, bool bank = false)
        {
            CharacterEditor character = RPCore.Db.SelectCharacter(cid);
            if (bank)
            {
                character.BankMoney -= count;
            }
            else
            {
                character.Money -= count;
            }
            RPCore.Db.UpdateCharacter(character);
            MoneyChangedEventArgs e = new MoneyChangedEventArgs(PlayerFinder.FindClientByCid(cid));
            if (MoneyChanged != null) MoneyChanged.Invoke(this, e);
        }
    }
}