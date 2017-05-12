using System;
using System.Dynamic;
using GTANetworkShared;

namespace Serverside.Database
{
    [Serializable]
    public class CharacterList
    {
        public long CID { get; set; }
        public long AccountID { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public decimal Money { get; set; }
        public decimal MoneyBank { get; set; }
    }

    [Serializable]
    public class CharacterEditor
    {
        
        public long CID { get; set; }
        public long AID { get; set; }

        public DateTime? LastLoginTime { get; set; }
        public DateTime? TodayPlayedTime { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public decimal Money { get; set; }
        public long BankAccountNumber { get; set; }
        public decimal BankMoney { get; set; }

        //Tak wiem, że to niepoprawne politycznie
        public bool Gender { get; set; }

        public short Weight { get; set; }
        public DateTime BornDate { get; set; }
        public short Height { get; set; }

        public short Force { get; set; }
        public short RunningEfficiency { get; set; }
        public short DivingEfficiency { get; set; }

        public long? FirstGID { get; set; }
        public long? SecondGID { get; set; }
        public long? ThirdGID { get; set; }

        public bool HasIDCard { get; set; }
        public bool HasDrivingLicense { get; set; }

        //Pomysł zezwolenie na bron jako enum, zeby byly różne typy licencji

        public string ForumDescription { get; set; }
        public string Story { get; set; }

        public bool IsAlive { get; set; }

        public int HitPoits { get; set; }

        public Vector3 LastPosition { get; set; }
        public Vector3 SpawnPosition { get; set; }

        public int CurrentDimension { get; set; }

        public int BWState { get; set; }

        //TODO Dodać do helpera
        public bool? IsCreated { get; set; }
        //Pole determinuje czy gracz będzie grał w gotowym skinie, czy może sam sobie zrobi
        public bool Freemode { get; set; }
        public int? Skin { get; set; }

        //TODO Dodać wszystkie pola na możliwości ubrania
        public int? Job { get; set; }
        public decimal? MoneyJob { get; set; }
        public decimal? JobLimit { get; set; }
    }
}
