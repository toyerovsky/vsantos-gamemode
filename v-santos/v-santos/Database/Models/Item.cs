using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class Item
    {
        [Key]
        //ID przedmiotu
        public long Id { get; set; }
        public string Name { get; set; }

        //Id właściciela
        public Character Character { get; set; }
        public Building Building { get; set; }
        public Vehicle Vehicle { get; set; }
        public Group Group { get; set; }

        //Zamysł: Przy tworzeniu przedmiotu będzie zapisywane kto go stworzył, a jak stworzy go serwer to 0
        public long CreatorId { get; set; }

        //Kiedy właścicielem jest budynek pole wykorzystujemy do trzymania ceny za ten przedmiot
        public decimal? Cost { get; set; }

        //Pomysł może kiedyś będzie jakiś system wagi
        public int Weight { get; set; }

        public int ItemHash { get; set; }

        public int? FirstParameter { get; set; }
        public int? SecondParameter { get; set; }
        public int? ThirdParameter { get; set; }

        public int ItemType { get; set; }

        public bool? CurrentlyInUse { get; set; }
    }
}
