using System.Linq;
using Serverside.Controllers;
using Serverside.Database.Models;

namespace Serverside.Groups.Base
{
    public class Police : GroupController
    {
        /* OPCJONALNE PRAWA
         * 1 - Megafon
         * 2 - Kajdanki, Prowadzenie gracza, Wpychanie do pojazdu innego gracza
         * 3 - Blokady drogowe
         * 4 - Kolczatka
         */

        public Police(Group editor) : base(editor)
        {
        }

        public bool CanPlayerUseMegaphone(AccountController account)
        {
            if (Data.Workers.All(w => w.Character.Id != account.CharacterController.Character.Id)) return false;
            Worker worker = Data.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.FirstRight.HasValue && worker.FirstRight.Value;
        }

        public bool CanPlayerDoPolice(AccountController account)
        {
            if (Data.Workers.All(w => w.Character.Id != account.CharacterController.Character.Id)) return false;
            Worker worker = Data.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.SecondRight.HasValue && worker.SecondRight.Value;
        }

        public bool CanPlayerPlaceRoadblocks(AccountController account)
        {
            if (Data.Workers.All(w => w.Character.Id != account.CharacterController.Character.Id)) return false;
            Worker worker = Data.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.SecondRight.HasValue && worker.SecondRight.Value;
        }

        public bool CanPlayerPlaceSpike(AccountController account)
        {
            if (Data.Workers.All(w => w.Character.Id != account.CharacterController.Character.Id)) return false;
            Worker worker = Data.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.SecondRight.HasValue && worker.SecondRight.Value;
        }
    }
}