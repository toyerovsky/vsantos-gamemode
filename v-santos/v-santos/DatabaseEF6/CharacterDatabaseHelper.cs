using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public static class CharacterDatabaseHelper
    {
        public static List<Character> SelectCharactersList(Account account)
        {
            return ContextFactory.Instance.Characters.Where(x => x.Account.Id == account.Id).ToList();
        }

        public static Character SelectCharacter(long characterid)
        {
            return ContextFactory.Instance.Characters.Where(x => x.CharacterId == characterid).FirstOrDefault();
        }

        public static void AddCharacter(Character character)
        {
            ContextFactory.Instance.Characters.Add(character);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateCharacter(Character character)
        {
            ContextFactory.Instance.Characters.Attach(character);
            ContextFactory.Instance.Entry(character).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteCharacter(long character)
        {
            Character delobj = ContextFactory.Instance.Characters.Where(x => x.CharacterId == character).FirstOrDefault();
            ContextFactory.Instance.Characters.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
