﻿using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class Item
    {
        [Key]
        //ID przedmiotu
        public long ItemId { get; set; }
        public string Name { get; set; }

        //Id właściciela
        public Character Character { get; set; }
        public Building Building { get; set; }
        //Typ właściciela przedmiotu 0 leży na ziemi 1 gracz 2 grupa 3 budynek(sejf)
        //public int OwnerType { get; set; } // lepiej sprawdzać czy characterid lub BuildingId == null

        //Zamysł: Przy tworzeniu przedmiotu będzie zapisywane kto go stworzył, a jak stworzy go serwer to 0
        public long CreatorId { get; set; }

        //TODO Dodać do helpera
        //Kiedy właścicielem jest firma pole wykorzystujemy do trzymania ceny za ten przedmiot
        public decimal? Cost { get; set; }

        //TODO Dodać do helpera
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
