using System.Linq;
using GrandTheftMultiplayer.Server.Constant;
using Serverside.Controllers;
using Serverside.Database.Models;
using Serverside.Groups.Enums;

namespace Serverside.Groups.Base
{
    public class CityHall : GroupController
    {
        /* OPCJONALNE PRAWA
         * 1 - wydawanie dowodu osobistego 
         * 2 - wydawanie prawa jazdy
         */

        public CityHall(Group editor) : base(editor)
        {
        }

        public CityHall(string name, string tag, GroupType type, Color color) : base(name, tag, type, color)
        {
        }

        public bool CanPlayerGiveIdCard(AccountController account)
        {
            if (!ContainsWorker(account)) return false;
            Worker worker = GroupData.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.FirstRight.HasValue && worker.FirstRight.Value;
        }

        public bool CanPlayerGiveDrivingLicense(AccountController account)
        {
            if (!ContainsWorker(account)) return false;
            Worker worker = GroupData.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.SecondRight.HasValue && worker.SecondRight.Value;
        }
    }
}