using GTANetworkShared;
using MySql.Data.MySqlClient;
using Serverside.Items;
using System;
using System.Collections.Generic;
using System.Data;

namespace Serverside.Database
{
    /* STRUKTURA TABELI SRV_PRZEMIOTY
     * +-----+----------+-----------+--------+-------+------------+-----------+---------+
     * | UID |TYP_WLASC | ID_WLASC | ID_TW | NAZWA | PRZED_HASH | TYP_PRZED | PARAM_1 |
     * +-----+----------+-----------+--------+-------+------------+-----------+---------+
     * ID_PRZ (long) - UID przedmiotu
     * TYP_WLASC (int) - Typ właściciela przedmiotu 0 leży na ziemi 1 gracz 2 grupa 3 budynek (sejf) 
     * ID_WLASC (long) - UID właściciela
     * ID_TW (long) - UID tworzącego - jeśli serwer to ma być 0
     * NAZWA (varchar) - Nazwa przedmiotu
     * PRZED_HASH (int) - hash obiektu przedmiotu
     * TYP_PRZED (int) - int, którzy będziemy rzutowali na enuma, żeby się dowiedzieć jaki to typ przedmiotu
     * PARAM_1 (int) - Parametr 1 przedmiotu, który przechowuje różne informacje dla różnych typów przedmiotu
     */

    /* STRUKTURA TABELI SRV_POSTACIE
     * +-----+-----------+----------------+-----------------+------+----------+---------+----------+------------+------+--------+---------+--------+------+-----------+-----------+----------+----------+----------+----------+---------------+------------+----------+----------+-----------+-----------+-----------+-----------+-------------+-------------+-------------+------+-------------+----+-------+
     * | UID | ID_KONTO | DATA_OST_ZALOG | DATA_DZIS_GRANE | IMIE | NAZWISKO | GOTOWKA | BANK_NR  | BANK_ILOSC | WAGA | WZROST | DATA_UR | WZROST | SILA | KOND_BIEG | KOND_NURK | UID_GR_1 | UID_GR_2 | UID_GR_3 | MA_DOWOD | MA_PRAW_JAZDY | OPIS_FORUM | HISTORIA | CZY_ZYJE | PKT_ZYCIE | OST_POZ_X | OST_POZ_Y | OST_POZ_Z | SPAWN_POZ_X | SPAWN_POZ_Y | SPAWN_POZ_Z | PLEC | OBEC_WYMIAR | BW | ZYCIE |
     * +-----+-----------+----------------+-----------------+------+----------+---------+----------+------------+------+--------+---------+--------+------+-----------+-----------+----------+----------+----------+----------+---------------+------------+----------+----------+-----------+-----------+-----------+-----------+-------------+-------------+-------------+------+-------------+----+-------+
     *  ID_POST (long) - UID postaci
     *  ID_KONTO (long) - UID konta do którego postać nalezy
     *  DATA_OST_ZALOG (date) - Data ostatniego logowania
     *  DATA_DZIS_GRANE (date) - Data ile dziś było się w grze na postaci
     *  IMIE (varchar) - Imie postaci
     *  NAZWISKO (varchar) - Nazwisko postaci
     *  GOTOWKA (money - Ile postać ma pieniędzy przy sobie
     *  BANK_NR (int) - Numer konta bankowego
     *  BANK_ILOSC (money) - Ile postać ma pieniędzy w banku
     *  WAGA (short) - Waga postaci
     *  WZROST (short) - Wzrost postaci
     *  DATA_UR (date) - Data urodzenia postaci
     *  SILA (short) - Siła postaci
     *  KOND_BIEG (short) - procent silla w bieganiu
     *  KOND_NURK (short) - procent silla w nurkowaniu
     *  UID_GR_1 (long) - UID grupy 1-szej do której postać należy
     *  UID_GR_2 (long) - UID grupy 2-giej do której postać należy
     *  UID_GR_3 (long) - UID grupy 3-ciej do której postać należy
     *  MA_DOWOD (tinyint) - Pole bool określające czy postać ma dowód
     *  MA_PRAWO_JAZD (tinyint) - Pole bool określające czy postać ma prawo jazdy
     *  OPIS_FORUM (varchar) - opis postaci, na forum
     *  HISTORIA (varchar) - historia postaci
     *  CZY_ZYJE (tinyint) - Pole bool określające czy postać ma CK
     *  PKT_ZYCIE (int) - Pole przechowujące punkty życia gracza
     *  OST_POZ_X (float) - koordynat X ostatniej pozycji
     *  OST_POZ_Y (float) - koordynat Y ostatniej pozycji
     *  OST_POZ_Z (float) - koordynat Z ostatniej pozycji
     *  SPAWN_POZ_X (float) - koordynat X ostatniej pozycji
     *  SPAWN_POZ_Y (float) - koordynat Y ostatniej pozycji
     *  SPAWN_POZ_Z (float) - koordynat Z ostatniej pozycji
     *  PLEC (tinyint) - bool, który przechowuje płeć postaci
     *  OBEC_WYMIAR (int) - wymiar w jakim obecnie znajduje się gracz
     *  BW (int) - czas jaki pozostał graczowi BW (0 - brak BW)
     */

    /*  STRUKTURA TABELI SRV_BUDYNKI
     *  +-----+-------+-----------+------+-----------+-----------+--------------+-----------------+-----------------+-----------------+------------------+-------------------+-------------------+--------------+-----------------+---------+---------+------------+--------+
     *  | UID | NAZWA | CZY_SPAWN | OPIS | TYP_WLASC | ID_WLASC | OPLATA_WEJSC | PICKUP_SRODEK_X | PICKUP_SRODEK_Y | PICKUP_SRODEK_Z | PICKUP_ZEWNARZ_X | PICKUP_ZEWNATRZ_Y | PICKUP_WEWNATRZ_Z | MAX_OBIEKTOW | AKTUAL_OBIEKTOW | MA_CCTV | MA_SEJF | WIRT_SWIAT | ID_TW |
     *  +-----+-------+-----------+------+-----------+-----------+--------------+-----------------+-----------------+-----------------+------------------+-------------------+-------------------+--------------+-----------------+---------+---------+------------+--------+
     *  UID_BUD (long) - UID budynku
     *  NAZWA (varchar) - nazwa budynku
     *  CZY_SPAWN (tinyint) - określa czy w budynku gracz moze sie spawnowac
     *  OPIS (varchar) - opis budynku
     *  TYP_WLASC (int) - określa typ wlasciciela, 0 - postać; 1 - grupa
     *  ID_WLASC (long) - UID właściciela
     *  OPLATA_WEJSC (money) - opłata pobierana za wejście do budynku
     *  PICKUP_WEW_X (float) - wspolrzedna X pickupu wewnątrz budynku
     *  PICKUP_WEW_Y (float) - wspolrzedna Y pickupu wewnątrz budynku
     *  PICKUP_WEW_Z (float) - wspolrzedna Z pickupu wewnątrz budynku
     *  PICKUP_ZEW_X (float) - wspolrzedna X pickupa gdzie jest budynek na zewnatrz
     *  PICKUP_ZEW_Y (float) - wspolrzedna Y pickupa gdzie jest budynek na zewnatrz
     *  PICKUP_ZEW_Z (float) - wspolrzedna Z pickupa gdzie jest budynek na zewnatrz
     *  MAX_OBIEKTOW (int) - maksymalna ilość obiektów w danym budynku
     *  AKTUAL_OBIEKTOW (int) - aktualna wartość ile obiektów jest w budynku
     *  MA_CCTV (tinyint) - czy budynek ma kamere CCTV
     *  MA_SEJF (tinyint) - czy budynek ma sejf
     *  WYMIAR (int) - wirtualny swiat w ktorym jest budynek
     *  ID_TW (long) - UID tworzącego budynek
     */

    /* STRUKTURA TABELI SRV_POJAZDY
     * ID_POJ (long) - UID pojazdu
     * ID_WLASC (long) - UID właściciela pojazdu
     * ID_TW (long) - UID tworzącego pojazd
     * NAZWA (string) - Nazwa pojazdu
     * REJESTRACJA (string) - Rejestracja pojazdu
     * POJ_HASH (int) - Hash pojazdu z GTA:N
     * TYP_WLASC (int) - Typ właściciela pojazdu 0 gracz, 1 grupa
     * SPAWN_POZ_X (float) - pozycja spawnu wspolrzedna X
     * SPAWN_POZ_Y (float) - pozycja spawnu wspolrzedna Y
     * SPAWN_POZ_Z (float) - pozycja spawnu wspolrzedna Z
     * ZESPAWNOWANY (tinyint) - czy pojazd jest zespawnowany
     */

    /* STRUKTURA TABELI SRV_GRUPY
     * ID_GRP (long) - UID grupy
     * TAG (string) - Tag/skrót grupy
     * NAZWA (string) - Nazwa grupy
     * GOTOWKA (decimal) ilość pieniedzy na koncie
     * DOTACJA (int) - Aktualna dotacja grupy
     * MAX_WYPLATA (int) - Maksymalna dotacja jaką mozna ustawic w panelu
     * TYP_GRP (int) - Typ grupy w incie
     * KOLOR (string) Kolor grupy
     */

    public class MySqlDatabaseHelper
    {
        #region Nie patrzeć publicznie
        //private readonly MySqlConnection Connection = new MySqlConnection
        //    ("server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=rp;");
        //private readonly MySqlConnection ForumConnection = new MySqlConnection
        //    ("server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=vsantos;");

        private readonly string ServerConnectionString = "server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=rp;";
        private readonly string ForumConnectionString = "server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=vsantos;";

        #endregion

        public bool GetPassword(string login, out string password)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT members_pass_hash FROM core_members WHERE name = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", login));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            password = reader.GetString(0);
                            return true;
                        }
                        password = string.Empty;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }
        }

        public string GetSalt(string login)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText =
                    "SELECT members_pass_salt FROM core_members WHERE name = @P0";
                command.Parameters.Add(new MySqlParameter("@P0", login));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                        return String.Empty;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }

        }

        public long GetAid(string login)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText =
                    "SELECT member_id FROM core_members WHERE name = @P0";
                command.Parameters.Add(new MySqlParameter("@P0", login));

                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    try
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt64(0);
                        }
                        return long.MinValue;
                    }
                    catch (Exception ex)
                    {
                        //trzeba jakos obsluzyc i przekazac do etapu logowania ze zly nick prawdopodobnie?                  
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }
        }


        public bool UserExist(string login)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText =
                    "SELECT COUNT(member_id) FROM core_members WHERE name = @P0";
                command.Parameters.Add(new MySqlParameter("@P0", login));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0) != 0;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Typy właściciela przedmiotu: 0 leży na ziemi, 1 gracz, 2 grupa, 3 budynek(sejf), 4 pojazd jako tuning
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="ownerType"></param>
        /// <returns></returns>
        public List<ItemList> SelectItemsList(long ownerId, int ownerType)
        {
            List<ItemList> items = new List<ItemList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT ID_PRZ, NAZWA, TYP_PRZED FROM srv_przedmioty WHERE ID_WLASC = @P0 AND TYP_WLASC = @P1";

                command.Parameters.Add(new MySqlParameter("@P0", ownerId));
                command.Parameters.Add(new MySqlParameter("@P1", ownerType));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            ItemList item = new ItemList
                            {
                                IID = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                ItemType = reader.GetInt32(2),

                            };
                            items.Add(item);
                        }
                        return items;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }



        public List<ItemEditor> SelectAllItemsForSpecifiedItemType(ItemType itemType)
        {
            List<ItemEditor> items = new List<ItemEditor>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT ID_PRZED, TYP_WLASC, ID_WLASC, NAZWA, PRZED_HASH, PARAM_1, PARAM_2, PARAM_3, OBECNIE_UZYWANY FROM srv_przedmioty WHERE TYP_PRZED = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", (int)itemType));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            ItemEditor item = new ItemEditor()
                            {
                                IID = reader.GetInt64(0),
                                OwnerType = reader.GetInt32(1),
                                OID = reader.GetInt64(2),
                                Name = reader.GetString(3),
                                ItemHash = reader.GetInt32(4),
                                ItemType = (int)itemType,
                                CurrentlyInUse = reader.GetBoolean(8),
                                FirstParameter = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                SecondParameter = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                ThirdParameter = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),

                            };

                            items.Add(item);

                        }
                        return items;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }


        public ItemEditor SelectItem(long itemId)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                "SELECT TYP_WLASC, ID_WLASC, NAZWA, PRZED_HASH, TYP_PRZED, PARAM_1, PARAM_2, PARAM_3, OBECNIE_UZYWANY FROM srv_przedmioty WHERE ID_PRZ = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", itemId));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    {
                        try
                        {
                            if (reader.Read())
                            {
                                ItemEditor item = new ItemEditor
                                {
                                    IID = itemId,
                                    OwnerType = reader.GetInt32(0),
                                    OID = reader.GetInt64(1),
                                    Name = reader.GetString(2),
                                    ItemHash = reader.GetInt32(3),
                                    ItemType = reader.GetInt32(4),

                                    FirstParameter = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                    SecondParameter = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                    ThirdParameter = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                                    CurrentlyInUse = reader.IsDBNull(8) ? (bool?)null : reader.GetBoolean(8)
                                };
                                return item;
                            }
                            return null;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                            reader.Close();
                        }
                    }
                }
            }
        }

        public void AddItem(ItemEditor item)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "INSERT INTO srv_przedmioty (TYP_WLASC, ID_WLASC, NAZWA, ID_TW, PRZED_HASH, TYP_PRZED, PARAM_1, PARAM_2, PARAM_3) VALUES (@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9)";

                command.Parameters.Add(new MySqlParameter("@P1", item.OwnerType));
                command.Parameters.Add(new MySqlParameter("@P2", item.OID));
                command.Parameters.Add(new MySqlParameter("@P3", item.Name));
                command.Parameters.Add(new MySqlParameter("@P4", item.CRID));
                command.Parameters.Add(new MySqlParameter("@P5", item.ItemHash));
                command.Parameters.Add(new MySqlParameter("@P6", item.ItemType));
                command.Parameters.Add(new MySqlParameter("@P7", item.FirstParameter));
                command.Parameters.Add(new MySqlParameter("@P8", item.SecondParameter));
                command.Parameters.Add(new MySqlParameter("@P9", item.ThirdParameter));

                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public void UpdateItem(ItemEditor item)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE srv_przedmioty SET TYP_WLASC = @P0, ID_WLASC = @P1, NAZWA = @P2, PRZED_HASH = @P3, TYP_PRZED = @P4, PARAM_1 = @P5, PARAM_2 = @P6, PARAM_3 = @P7, OBECNIE_UZYWANY = @P8 WHERE ID_PRZ = @P9";


                command.Parameters.Add(new MySqlParameter("@P0", item.OwnerType));
                command.Parameters.Add(new MySqlParameter("@P1", item.OID));
                command.Parameters.Add(new MySqlParameter("@P2", item.Name));
                command.Parameters.Add(new MySqlParameter("@P3", item.ItemHash));
                command.Parameters.Add(new MySqlParameter("@P4", item.ItemType));
                command.Parameters.Add(new MySqlParameter("@P5", item.FirstParameter));
                command.Parameters.Add(new MySqlParameter("@P6", item.SecondParameter));
                command.Parameters.Add(new MySqlParameter("@P7", item.ThirdParameter));
                command.Parameters.Add(new MySqlParameter("@P8", item.CurrentlyInUse));

                command.Parameters.Add(new MySqlParameter("@P9", item.IID));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteItem(long itemId)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM srv_przedmioty WHERE ID_PRZ = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", itemId));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public List<BuildingList> SelectBuildingList(long ownerId, int ownerType)
        {
            List<BuildingList> buildings = new List<BuildingList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT UID, NAZWA, CZY_SPAWN, OPIS, OPLATA_WEJSC, MAX_OBIEKTOW, AKTUAL_OBIEKTOW, MA_CCTV, MA_SEJF," +
                    "FROM --TUTAJ_WPISAC_NAZWE_TABELI-- WHERE ID_WLASC = @P0 AND TYP_WLASC = @P1";


                command.Parameters.Add(new MySqlParameter("@P0", ownerId));
                command.Parameters.Add(new MySqlParameter("@P1", ownerType));

                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    try
                    {
                        while (reader.Read())
                        {
                            BuildingList building = new BuildingList
                            {
                                BID = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                SpawnPossible = reader.GetBoolean(2),
                                Description = reader.GetString(3),
                                EnterCharge = reader.GetDecimal(4),
                                MaxObjectCount = reader.GetInt16(5),
                                CurrentObjectCount = reader.GetInt16(6),
                                HasCCTV = reader.GetBoolean(7),
                                HasSafe = reader.GetBoolean(8)
                            };

                            buildings.Add(building);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                    return buildings;
                }
            }
        }

        public BuildingEditor SelectBuilding(long buildingId)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT NAZWA, TYP_WLASC, ID_WLASC, OPLATA_WEJSC, PICKUP_ZEWNATRZ_X, PICKUP_ZEWNATRZ_Y, PICKUP_ZEWNATRZ_Z, PICKUP_SRODEK_X, PICKUP_SRODEK_Y, " +
                    "PICKUP_SRODEK_Z, MAX_OBIEKTOW, AKTUAL_OBIEKTOW, CZY_SPAWN, MA_CCTV, MA_SEJF, WIRT_SWIAT, OPIS, ID_TW " +
                    "FROM --TUTAJ_WPISAC_NAZWE_TABELI-- WHERE UID = @P0";


                command.Parameters.Add(new MySqlParameter("@P0", buildingId));
                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    try
                    {
                        if (reader.Read())
                        {
                            BuildingEditor building = new BuildingEditor
                            {
                                BID = buildingId,
                                Name = reader.GetString(0),
                                OwnerType = reader.GetInt32(1),
                                OwnerUID = reader.GetInt64(2),
                                EnterCharge = reader.GetDecimal(3),
                                ExternalPickupPosition = new Vector3(reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6)),
                                InternalPickupPosition = new Vector3(reader.GetFloat(7), reader.GetFloat(8), reader.GetFloat(9)),
                                MaxObjectCount = reader.GetInt16(10),
                                CurrentObjectCount = reader.GetInt16(11),
                                SpawnPossible = reader.GetBoolean(12),
                                HasCCTV = reader.GetBoolean(13),
                                HasSafe = reader.GetBoolean(14),
                                InternalDismension = reader.GetInt32(15),
                                Description = reader.GetString(16),
                                CreatorsUID = reader.GetInt64(17)

                            };
                            return building;
                        }

                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void AddBuilding(BuildingEditor building)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO --TUTAJ_WPISAC_NAZWE_TABELI-- (UID, NAZWA, CZY_SPAWN, OPIS, TYP_WLASC, ID_WLASC, OPLATA_WEJSC," + " PICKUP_SRODEK_X, PICKUP_SRODEK_Y, PICKUP_SRODEK_Z, PICKUP_SRODEK_X, PICKUP_SRODEK_Y, PICKUP_SRODEK_Z," + " MAX_OBIEKTOW, AKTUAL_OBIEKTOW, MA_CCTV, MA_SEJF, WIRT_SWIAT, ID_TW) VALUES (@P0, @P1, @P2, @P3, @P4, @P5, @P6, @P7, @8, @P9, @P10, @P11, @P12, @P13, @P14, @P15, @P16, @P17, @P18)";


                command.Parameters.Add(new MySqlParameter("@P0", building.BID));
                command.Parameters.Add(new MySqlParameter("@P1", building.Name));
                command.Parameters.Add(new MySqlParameter("@P2", building.SpawnPossible));
                command.Parameters.Add(new MySqlParameter("@P3", building.Description));
                command.Parameters.Add(new MySqlParameter("@P4", building.OwnerType));
                command.Parameters.Add(new MySqlParameter("@P5", building.OwnerUID));
                command.Parameters.Add(new MySqlParameter("@P6", building.EnterCharge));
                command.Parameters.Add(new MySqlParameter("@P7", building.InternalPickupPosition.X));
                command.Parameters.Add(new MySqlParameter("@P8", building.InternalPickupPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P9", building.InternalPickupPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P10", building.ExternalPickupPosition.X));
                command.Parameters.Add(new MySqlParameter("@P11", building.ExternalPickupPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P12", building.ExternalPickupPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P13", building.MaxObjectCount));
                command.Parameters.Add(new MySqlParameter("@P14", building.CurrentObjectCount));
                command.Parameters.Add(new MySqlParameter("@P15", building.HasCCTV));
                command.Parameters.Add(new MySqlParameter("@P16", building.HasSafe));
                command.Parameters.Add(new MySqlParameter("@P17", building.InternalDismension));
                command.Parameters.Add(new MySqlParameter("@P18", building.CreatorsUID));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateBuilding(BuildingEditor building)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE --TUTAJ_WPISAC_NAZWE_TABELI-- SET NAZWA = @P0, CZY_SPAWN = @P1, OPIS = @P2, TYP_WLASC = @P3," +
                    " ID_WLASC = @P4, OPLATA_WEJSC = @P5, PICKUP_SRODEK_X = @P6, PICKUP_SRODEK_Y = @P7, PICKUP_SRODEK_Z = @P8," +
                    " PICKUP_ZEWNATRZ_X = @P9, PICKUP_ZEWNATRZ_Y = @P10, PICKUP_ZEWNATRZ_Z = @P11, MAX_OBIEKTOW = @P12," +
                    " AKTUAL_OBIEKTOW = @P13, MA_CCTV = @P14, MA_SEJF = @P15, WIRT_SWIAT = @P16 WHERE UID = @P17";

                command.Parameters.Add(new MySqlParameter("@P0", building.Name));
                command.Parameters.Add(new MySqlParameter("@P1", building.SpawnPossible));
                command.Parameters.Add(new MySqlParameter("@P2", building.Description));
                command.Parameters.Add(new MySqlParameter("@P3", building.OwnerType));
                command.Parameters.Add(new MySqlParameter("@P4", building.OwnerUID));
                command.Parameters.Add(new MySqlParameter("@P5", building.EnterCharge));
                command.Parameters.Add(new MySqlParameter("@P6", building.InternalPickupPosition.X));
                command.Parameters.Add(new MySqlParameter("@P7", building.InternalPickupPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P8", building.InternalPickupPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P9", building.ExternalPickupPosition.X));
                command.Parameters.Add(new MySqlParameter("@P10", building.ExternalPickupPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P11", building.ExternalPickupPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P12", building.MaxObjectCount));
                command.Parameters.Add(new MySqlParameter("@P13", building.CurrentObjectCount));
                command.Parameters.Add(new MySqlParameter("@P14", building.HasCCTV));
                command.Parameters.Add(new MySqlParameter("@P15", building.HasSafe));
                command.Parameters.Add(new MySqlParameter("@P16", building.InternalDismension));

                command.Parameters.Add(new MySqlParameter("@P17", building.BID));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteBuilding(long buildingId)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM --TUTAJ_WPISAC_NAZWE_TABELI-- WHERE UID = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", buildingId));
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public List<CharacterList> SelectCharactersList(long accountId)
        {
            List<CharacterList> characters = new List<CharacterList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT ID_POST, IMIE, NAZWISKO, GOTOWKA, BANK_ILOSC FROM srv_postacie WHERE ID_KONTO = @P0";


                command.Parameters.Add(new MySqlParameter("@P0", accountId));


                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    try
                    {
                        while (reader.Read())
                        {
                            CharacterList character = new CharacterList
                            {
                                CID = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Surname = reader.GetString(2),
                                Money = reader.GetDecimal(3),
                                MoneyBank = reader.GetDecimal(4)

                            };


                            characters.Add(character);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                    return characters;
                }
            }
        }

        public CharacterEditor SelectCharacter(long cid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT ID_KONTO, DATA_DZIS_GRANE, IMIE, NAZWISKO, GOTOWKA, BANK_NR, BANK_ILOSC, WAGA, DATA_UR, WZROST," +
                    " SILA, KOND_BIEG, KOND_NURK, UID_GR_1, UID_GR_2, UID_GR_3, MA_DOWOD, MA_PRAWO_JAZD, OPIS_FORUM, HISTORIA," +
                    " CZY_ZYJE, PKT_ZYCIE, OST_POZ_X, OST_POZ_Y, OST_POZ_Z, SPAWN_POZ_X, SPAWN_POZ_Y, SPAWN_POZ_Z, PLEC, OBEC_WYMIAR, BW," +
                    " PRACA, GOTOWKA_PRACA, PRACA_LIMIT" +
                    " FROM srv_postacie WHERE ID_POST = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", cid));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            CharacterEditor character = new CharacterEditor
                            {
                                CID = cid,
                                AID = reader.GetInt64(0),
                                TodayPlayedTime = reader.GetDateTime(1),
                                Name = reader.GetString(2),
                                Surname = reader.GetString(3),
                                Money = reader.GetDecimal(4),
                                BankAccountNumber = reader.GetInt32(5),
                                BankMoney = reader.GetDecimal(6),
                                Weight = reader.GetInt16(7),
                                BornDate = reader.GetDateTime(8),
                                Height = reader.GetInt16(9),
                                Force = reader.GetInt16(10),
                                RunningEfficiency = reader.GetInt16(11),
                                DivingEfficiency = reader.GetInt16(12),
                                HasIDCard = reader.GetBoolean(16),
                                HasDrivingLicense = reader.GetBoolean(17),
                                ForumDescription = reader.GetString(18),
                                Story = reader.GetString(19),
                                IsAlive = reader.GetBoolean(20),
                                HitPoits = reader.GetInt32(21),
                                LastPosition = new Vector3(reader.GetFloat(22), reader.GetFloat(23), reader.GetFloat(24)),
                                SpawnPosition = new Vector3(reader.GetFloat(25), reader.GetFloat(26), reader.GetFloat(27)),
                                Gender = reader.GetBoolean(28),
                                CurrentDimension = reader.GetInt32(29),
                                BWState = reader.GetInt32(30),
                            };

                            character.FirstGID = reader.IsDBNull(13) ? (long?)null : reader.GetInt64(13);
                            character.SecondGID = reader.IsDBNull(14) ? (long?)null : reader.GetInt64(14);
                            character.ThirdGID = reader.IsDBNull(14) ? (long?)null : reader.GetInt64(15);
                            character.Job = reader.IsDBNull(31) ? (int?)null : reader.GetInt32(31);
                            character.MoneyJob = reader.IsDBNull(32) ? (decimal?)null : reader.GetDecimal(32);
                            character.JobLimit = reader.IsDBNull(33) ? (decimal?)null : reader.GetDecimal(33);

                            return character;
                        }

                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void AddCharacter(CharacterEditor character)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "INSERT INTO srv_postacie (ID_KONTO, IMIE, NAZWISKO, GOTOWKA, BANK_NR," +
                    " BANK_ILOSC, WAGA, DATA_UR, WZROST, SILA, KOND_BIEG, KOND_NURK, UID_GR_1, UID_GR_2, UID_GR_3," +
                    " MA_DOWOD, MA_PRAWO_JAZD, OPIS_FORUM, HISTORIA, CZY_ZYJE, PKT_ZYCIE OST_POZ_X, OST_POZ_Y, OST_POZ_Z," +
                    " SPAWN_POZ_X, SPAWN_POZ_Y, SPAWN_POZ_Z, PLEC, OBEC_WYMIAR, BW, PRACA, GOTOWKA_PRACA, PRACA_LIMIT)" +
                    " VALUES (@P0, @P1, @P2, @P3, @P4, @P5, @P6, @P8, @P9, @P10, @P11, @P12, @P13, @P14, @P15, @P16, @P17," +
                    " @P18, @P19, @P20, @P21, @P22, @P23, @P24, @P25, @P26, @P27, @P28, @P29, @P30, @P31, @P32, @P33)";

                command.Parameters.Add(new MySqlParameter("@P1", character.AID));
                command.Parameters.Add(new MySqlParameter("@P2", character.Name));
                command.Parameters.Add(new MySqlParameter("@P3", character.Surname));
                command.Parameters.Add(new MySqlParameter("@P4", character.Money));
                command.Parameters.Add(new MySqlParameter("@P5", character.BankAccountNumber));
                command.Parameters.Add(new MySqlParameter("@P6", character.BankMoney));
                command.Parameters.Add(new MySqlParameter("@P7", character.Weight));
                command.Parameters.Add(new MySqlParameter("@P8", character.BornDate));
                command.Parameters.Add(new MySqlParameter("@P9", character.Height));
                command.Parameters.Add(new MySqlParameter("@P10", character.Force));
                command.Parameters.Add(new MySqlParameter("@P11", character.RunningEfficiency));
                command.Parameters.Add(new MySqlParameter("@P12", character.DivingEfficiency));
                command.Parameters.Add(new MySqlParameter("@P13", character.FirstGID));
                command.Parameters.Add(new MySqlParameter("@P14", character.SecondGID));
                command.Parameters.Add(new MySqlParameter("@P15", character.ThirdGID));
                command.Parameters.Add(new MySqlParameter("@P16", character.HasIDCard));
                command.Parameters.Add(new MySqlParameter("@P17", character.HasDrivingLicense));
                command.Parameters.Add(new MySqlParameter("@P18", character.ForumDescription));
                command.Parameters.Add(new MySqlParameter("@P19", character.Story));
                command.Parameters.Add(new MySqlParameter("@P20", character.IsAlive));
                command.Parameters.Add(new MySqlParameter("@P21", character.HitPoits));
                command.Parameters.Add(new MySqlParameter("@P22", character.LastPosition.X));
                command.Parameters.Add(new MySqlParameter("@P23", character.LastPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P24", character.LastPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P25", character.SpawnPosition.X));
                command.Parameters.Add(new MySqlParameter("@P26", character.SpawnPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P27", character.SpawnPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P28", character.Gender));
                command.Parameters.Add(new MySqlParameter("@P29", character.CurrentDimension));
                command.Parameters.Add(new MySqlParameter("@P30", character.BWState));
                command.Parameters.Add(new MySqlParameter("@P31", character.Job));
                command.Parameters.Add(new MySqlParameter("@P32", character.MoneyJob));
                command.Parameters.Add(new MySqlParameter("@P33", character.JobLimit));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateCharacter(CharacterEditor character)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE srv_postacie SET DATA_OST_ZALOG = @P0, DATA_DZIS_GRANE = @P1, IMIE = @P2," +
                    " NAZWISKO = @P3, GOTOWKA = @P4, BANK_NR = @P5, BANK_ILOSC = @P6, WAGA = @P7, DATA_UR = @P8, WZROST = @P9," +
                    " SILA = @P10, KOND_BIEG = @P11, KOND_NURK = @P12, UID_GR_1 = @P13, UID_GR_2 = @P14, UID_GR_3 = @P15," +
                    " MA_DOWOD = @P16, MA_PRAWO_JAZD = @P17, OPIS_FORUM = @P18, HISTORIA = @P19, CZY_ZYJE = @P20, PKT_ZYCIE = @P21," +
                    " OST_POZ_X = @P22, OST_POZ_Y = @P23, OST_POZ_Z = @P24, SPAWN_POZ_X = @P25, SPAWN_POZ_Y = @P26, SPAWN_POZ_Z = @P27," +
                    " PLEC = @P28, OBEC_WYMIAR = @P29, BW = @P30, PRACA = @P31, GOTOWKA_PRACA = @P32, PRACA_LIMIT = @P33 WHERE ID_POST = @P34";

                command.Parameters.Add(new MySqlParameter("@P0", character.LastLoginTime));
                command.Parameters.Add(new MySqlParameter("@P1", character.TodayPlayedTime));
                command.Parameters.Add(new MySqlParameter("@P2", character.Name));
                command.Parameters.Add(new MySqlParameter("@P3", character.Surname));
                command.Parameters.Add(new MySqlParameter("@P4", character.Money));
                command.Parameters.Add(new MySqlParameter("@P5", character.BankAccountNumber));
                command.Parameters.Add(new MySqlParameter("@P6", character.BankMoney));
                command.Parameters.Add(new MySqlParameter("@P7", character.Weight));
                command.Parameters.Add(new MySqlParameter("@P8", character.BornDate));
                command.Parameters.Add(new MySqlParameter("@P9", character.Height));
                command.Parameters.Add(new MySqlParameter("@P10", character.Force));
                command.Parameters.Add(new MySqlParameter("@P11", character.RunningEfficiency));
                command.Parameters.Add(new MySqlParameter("@P12", character.DivingEfficiency));
                command.Parameters.Add(new MySqlParameter("@P13", character.FirstGID));
                command.Parameters.Add(new MySqlParameter("@P14", character.SecondGID));
                command.Parameters.Add(new MySqlParameter("@P15", character.ThirdGID));
                command.Parameters.Add(new MySqlParameter("@P16", character.HasIDCard));
                command.Parameters.Add(new MySqlParameter("@P17", character.HasDrivingLicense));
                command.Parameters.Add(new MySqlParameter("@P18", character.ForumDescription));
                command.Parameters.Add(new MySqlParameter("@P19", character.Story));
                command.Parameters.Add(new MySqlParameter("@P20", character.IsAlive));
                command.Parameters.Add(new MySqlParameter("@P21", character.HitPoits));
                command.Parameters.Add(new MySqlParameter("@P22", character.LastPosition.X));
                command.Parameters.Add(new MySqlParameter("@P23", character.LastPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P24", character.LastPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P25", character.SpawnPosition.X));
                command.Parameters.Add(new MySqlParameter("@P26", character.SpawnPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P27", character.SpawnPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P28", character.Gender));
                command.Parameters.Add(new MySqlParameter("@P29", character.CurrentDimension));
                command.Parameters.Add(new MySqlParameter("@P30", character.BWState));
                command.Parameters.Add(new MySqlParameter("@P31", character.Job));
                command.Parameters.Add(new MySqlParameter("@P32", character.MoneyJob));
                command.Parameters.Add(new MySqlParameter("@P33", character.JobLimit));

                command.Parameters.Add(new MySqlParameter("@P34", character.CID));

                command.ExecuteNonQuery();
                connection.Close();

            }
        }

        public void DeleteCharacter(long cid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM srv_postacie WHERE ID_POST = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", cid));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Typy właścicieli: 1 gracz, 2 grupa
        /// </summary>
        /// <param name="ownerUID"></param>
        /// <param name="ownerType"></param>
        /// <returns></returns>
        public List<VehicleList> SelectVehiclesList(long ownerUID, int ownerType)
        {
            List<VehicleList> vehicles = new List<VehicleList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT ID_POJ, NAZWA FROM srv_pojazdy WHERE ID_WLASC = @P0";

                //wybierz wszystkie rekordy, których UID właściciela pasuje
                command.Parameters.Add(new MySqlParameter("@P0", ownerUID));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            VehicleList vehicle = new VehicleList
                            {
                                VID = reader.GetInt64(0),
                                Name = reader.GetString(1)
                            };

                            vehicles.Add(vehicle);
                        }
                        return vehicles;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public VehicleEditor SelectVehicle(long vid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT NAZWA, ID_TW, ID_WLASC, REJESTRACJA, POJ_HASH, TYP_WLASC, SPAWN_POZ_X, SPAWN_POZ_Y, SPAWN_POZ_Z, ZESPAWNOWANY, MNOZNIK_SILNIKA," +
                    " SPAWN_ROT_X, SPAWN_ROT_Y, SPAWN_ROT_Z, ZYCIE, DRZWI_USZK_1, DRZWI_USZK_2, DRZWI_USZK_3, DRZWI_USZK_4, SZYBA_USZK_1, SZYBA_USZK_2," +
                    " SZYBA_USZK_3, SZYBA_USZK_4, KOLOR_PODST, KOLOR_DODAT, FELGI_TYP, FELGI_KOLOR FROM srv_pojazdy WHERE ID_POJ = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", vid));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            VehicleEditor vehicle = new VehicleEditor
                            {
                                VID = vid,
                                Name = reader.GetString(0),
                                CreatorsID = reader.GetInt64(1),
                                OwnerID = reader.GetInt64(2),
                                Registration = reader.GetString(3),
                                VehicleHash = reader.GetInt32(4),
                                OwnerType = reader.GetInt32(5),
                                SpawnPosition = new Vector3(reader.GetFloat(6), reader.GetFloat(7), reader.GetFloat(8)),
                                IsSpawned = reader.GetBoolean(9),
                                EngineMultipler = reader.GetFloat(10),
                                RotationPosition = new Vector3(reader.GetFloat(11), reader.GetFloat(12), reader.GetFloat(13)),
                                Health = reader.GetDouble(14),
                                Door1Damage = reader.GetBoolean(15),
                                Door2Damage = reader.GetBoolean(16),
                                Door3Damage = reader.GetBoolean(17),
                                Door4Damage = reader.GetBoolean(18),
                                Window1Damage = reader.GetBoolean(19),
                                Window2Damage = reader.GetBoolean(20),
                                Window3Damage = reader.GetBoolean(21),
                                Window4Damage = reader.GetBoolean(22),
                                PrimaryColor = reader.GetInt32(23),
                                SecondaryColor = reader.GetInt32(24),
                                WheelType = reader.GetInt32(25),
                                WheelColor = reader.GetInt32(26)
                            };

                            return vehicle;
                        }

                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void AddVehicle(VehicleEditor vehicle)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "INSERT INTO srv_pojazdy (ID_WLASC, TYP_WLASC, ID_TW, NAZWA, REJESTRACJA, POJ_HASH, SPAWN_POZ_X, SPAWN_POZ_Y, SPAWN_POZ_Z," +
                    " ZESPAWNOWANY, MNOZNIK_SILNIKA, SPAWN_ROT_X, SPAWN_ROT_Y, SPAWN_ROT_Z, ZYCIE, DRZWI_USZK_1, DRZWI_USZK_2, DRZWI_USZK_3, DRZWI_USZK_4," +
                    " SZYBA_USZK_1, SZYBA_USZK_2, SZYBA_USZK_3, SZYBA_USZK_4, KOLOR_PODST, KOLOR_DODAT, FELGI_TYP, FELGI_KOLOR) VALUES (@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10, @P11, @P12, @P13, @P14, @P15, @P16, @P17, @P18, @P19, @P20, @P21, @P22, @P23, @P24, @P25, @P26, @P27)";

                command.Parameters.Add(new MySqlParameter("@P1", vehicle.OwnerID));
                command.Parameters.Add(new MySqlParameter("@P2", vehicle.OwnerType));
                command.Parameters.Add(new MySqlParameter("@P3", vehicle.CreatorsID));
                command.Parameters.Add(new MySqlParameter("@P4", vehicle.Name));
                command.Parameters.Add(new MySqlParameter("@P5", vehicle.Registration));
                command.Parameters.Add(new MySqlParameter("@P6", vehicle.VehicleHash));
                command.Parameters.Add(new MySqlParameter("@P7", vehicle.SpawnPosition.X));
                command.Parameters.Add(new MySqlParameter("@P8", vehicle.SpawnPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P9", vehicle.SpawnPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P10", vehicle.IsSpawned));
                command.Parameters.Add(new MySqlParameter("@P11", vehicle.EngineMultipler));
                command.Parameters.Add(new MySqlParameter("@P12", vehicle.RotationPosition.X));
                command.Parameters.Add(new MySqlParameter("@P13", vehicle.RotationPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P14", vehicle.RotationPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P15", vehicle.Health));
                command.Parameters.Add(new MySqlParameter("@P16", vehicle.Door1Damage));
                command.Parameters.Add(new MySqlParameter("@P17", vehicle.Door2Damage));
                command.Parameters.Add(new MySqlParameter("@P18", vehicle.Door3Damage));
                command.Parameters.Add(new MySqlParameter("@P19", vehicle.Door4Damage));
                command.Parameters.Add(new MySqlParameter("@P20", vehicle.Window1Damage));
                command.Parameters.Add(new MySqlParameter("@P21", vehicle.Window2Damage));
                command.Parameters.Add(new MySqlParameter("@P22", vehicle.Window3Damage));
                command.Parameters.Add(new MySqlParameter("@P23", vehicle.Window4Damage));
                command.Parameters.Add(new MySqlParameter("@P24", vehicle.PrimaryColor));
                command.Parameters.Add(new MySqlParameter("@P25", vehicle.SecondaryColor));
                command.Parameters.Add(new MySqlParameter("@P26", vehicle.WheelType));
                command.Parameters.Add(new MySqlParameter("@P27", vehicle.WheelColor));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateVehicle(VehicleEditor vehicle)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE srv_pojazdy SET TYP_WLASC = @P0, ID_WLASC = @P1, NAZWA = @P2, POJ_HASH = @P3, REJESTRACJA = @P4, SPAWN_POZ_X = @P5," +
                    " SPAWN_POZ_Y = @P6, SPAWN_POZ_Z = @P7, ZESPAWNOWANY = @P8, MNOZNIK_SILNIKA = @P9, SPAWN_ROT_X = @P10, SPAWN_ROT_Y = @P11," +
                    " SPAWN_ROT_Z = @P12, ZYCIE = @P13, DRZWI_USZK_1 = @P14, DRZWI_USZK_2 = @P15, DRZWI_USZK_3 = @P16, DRZWI_USZK_4 = @P17," +
                    " SZYBA_USZK_1 = @P18, SZYBA_USZK_2 = @P19, SZYBA_USZK_3 = @P20, SZYBA_USZK_4 = @P21, KOLOR_PODST = @P22, KOLOR_DODAT = @P23," +
                    " FELGI_TYP = @P24, FELGI_KOLOR = @P25 WHERE ID_POJ = @P26";

                command.Parameters.Add(new MySqlParameter("@P0", vehicle.OwnerType));
                command.Parameters.Add(new MySqlParameter("@P1", vehicle.OwnerID));
                command.Parameters.Add(new MySqlParameter("@P2", vehicle.Name));
                command.Parameters.Add(new MySqlParameter("@P3", vehicle.VehicleHash));
                command.Parameters.Add(new MySqlParameter("@P4", vehicle.Registration));
                command.Parameters.Add(new MySqlParameter("@P5", vehicle.SpawnPosition.X));
                command.Parameters.Add(new MySqlParameter("@P6", vehicle.SpawnPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P7", vehicle.SpawnPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P8", vehicle.IsSpawned));
                command.Parameters.Add(new MySqlParameter("@P9", vehicle.EngineMultipler));
                command.Parameters.Add(new MySqlParameter("@P10", vehicle.RotationPosition.X));
                command.Parameters.Add(new MySqlParameter("@P11", vehicle.RotationPosition.Y));
                command.Parameters.Add(new MySqlParameter("@P12", vehicle.RotationPosition.Z));
                command.Parameters.Add(new MySqlParameter("@P13", vehicle.Health));
                command.Parameters.Add(new MySqlParameter("@P14", vehicle.Door1Damage));
                command.Parameters.Add(new MySqlParameter("@P15", vehicle.Door2Damage));
                command.Parameters.Add(new MySqlParameter("@P16", vehicle.Door3Damage));
                command.Parameters.Add(new MySqlParameter("@P17", vehicle.Door4Damage));
                command.Parameters.Add(new MySqlParameter("@P18", vehicle.Window1Damage));
                command.Parameters.Add(new MySqlParameter("@P19", vehicle.Window2Damage));
                command.Parameters.Add(new MySqlParameter("@P20", vehicle.Window3Damage));
                command.Parameters.Add(new MySqlParameter("@P21", vehicle.Window4Damage));
                command.Parameters.Add(new MySqlParameter("@P22", vehicle.PrimaryColor));
                command.Parameters.Add(new MySqlParameter("@P23", vehicle.SecondaryColor));
                command.Parameters.Add(new MySqlParameter("@P24", vehicle.WheelType));
                command.Parameters.Add(new MySqlParameter("@P25", vehicle.WheelColor));
                command.Parameters.Add(new MySqlParameter("@P26", vehicle.VID));
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteVehicle(long vehicleId)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM srv_pojazdy WHERE UID = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", vehicleId));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public List<GroupList> SelectAllGroups()
        {
            List<GroupList> groups = new List<GroupList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT ID_GRP, TAG, NAZWA, TYP_GRP FROM srv_grupy";

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            GroupList group = new GroupList
                            {
                                GID = reader.GetInt64(0),
                                Tag = reader.GetString(1),
                                Name = reader.GetString(2),
                                GroupType = reader.GetInt32(3)

                            };

                            groups.Add(group);
                        }
                        return groups;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public GroupEditor SelectGroup(long groupUid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT TAG, NAZWA, GOTOWKA, DOTACJA, MAX_WYPLATA, TYP_GRP, KOLOR FROM srv_grupy WHERE ID_GRP = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", groupUid));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            GroupEditor group = new GroupEditor
                            {
                                GID = groupUid,
                                Tag = reader.GetString(0),
                                Name = reader.GetString(1),
                                Money = reader.GetInt32(2),
                                Dotation = reader.GetInt32(3),
                                MaxPayday = reader.GetInt32(4),
                                GroupType = reader.GetInt32(5),
                                Color = reader.GetString(6)
                            };

                            return group;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void AddGroup(GroupEditor group)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "INSERT INTO srv_grupy (TAG, NAZWA, GOTOWKA, DOTACJA, MAX_WYPLATA, TYP_GRP, KOLOR) VALUES (@P0, @P1, @P2, @P3, @P4, @P5, @P6)";

                command.Parameters.Add(new MySqlParameter("@P0", group.Tag));
                command.Parameters.Add(new MySqlParameter("@P1", group.Name));
                command.Parameters.Add(new MySqlParameter("@P2", group.Money));
                command.Parameters.Add(new MySqlParameter("@P3", group.Dotation));
                command.Parameters.Add(new MySqlParameter("@P4", group.MaxPayday));
                command.Parameters.Add(new MySqlParameter("@P5", group.GroupType));
                command.Parameters.Add(new MySqlParameter("@P6", group.Color));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateGroup(GroupEditor group)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE srv_grupy SET TAG = @P0, NAZWA = @P1, GOTOWKA = @P2, DOTACJA = @P3, MAX_WYPLATA = @P4, TYP_GRP = @P5, KOLOR = @P6 WHERE ID_GRP = @P7";

                command.Parameters.Add(new MySqlParameter("@P0", group.Tag));
                command.Parameters.Add(new MySqlParameter("@P1", group.Name));
                command.Parameters.Add(new MySqlParameter("@P2", group.Money));
                command.Parameters.Add(new MySqlParameter("@P3", group.Dotation));
                command.Parameters.Add(new MySqlParameter("@P4", group.MaxPayday));
                command.Parameters.Add(new MySqlParameter("@P5", group.GroupType));
                command.Parameters.Add(new MySqlParameter("@P6", group.Color));

                command.Parameters.Add(new MySqlParameter("@P7", group.GID));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteGroup(long gid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM srv_grupy WHERE ID_GRP = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", gid));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public List<WorkerList> SelectWorkersList(long groupUid)
        {
            List<WorkerList> workers = new List<WorkerList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT ID_PRAC, ID_POST FROM srv_pracownicy WHERE ID_GRP = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", groupUid));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            WorkerList workerList = new WorkerList
                            {
                                WID = reader.GetInt64(0),
                                CID = reader.GetInt64(1)
                            };
                            workers.Add(workerList);
                        }
                        return workers;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public WorkerEditor SelectWorker(long workerId)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT ID_GRP, ID_POST, WYPLATA, MINUTY_SLUZBA, PRAWO_WYPLATA, PRAWO_DRZWI, PRAWO_REK, PRAWO_CZAT, PRAWO_MAGAZYN," +
                    " PRAWO_1, PRAWO_2, PRAWO_3, PRAWO_4, PRAWO_5, PRAWO_6 FROM srv_pracownicy WHERE ID_PRAC = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", workerId));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            WorkerEditor workerEditor = new WorkerEditor
                            {
                                WID = workerId,
                                GID = reader.GetInt64(0),
                                CID = reader.GetInt64(1),
                                Salary = reader.GetInt32(2),
                                DutyMinutes = reader.GetInt32(3),
                                PaycheckRight = reader.GetBoolean(4),
                                DoorsRight = reader.GetBoolean(5),
                                RecrutationRight = reader.GetBoolean(6),
                                ChatRight = reader.GetBoolean(7),
                                OfferFromWarehouseRight = reader.GetBoolean(8)
                            };

                            workerEditor.FirstRight = reader.IsDBNull(9) ? (bool?)null : reader.GetBoolean(9);
                            workerEditor.SecondRight = reader.IsDBNull(10) ? (bool?)null : reader.GetBoolean(10);
                            workerEditor.ThirdRight = reader.IsDBNull(11) ? (bool?)null : reader.GetBoolean(11);
                            workerEditor.FourthRight = reader.IsDBNull(12) ? (bool?)null : reader.GetBoolean(12);
                            workerEditor.FifthRight = reader.IsDBNull(13) ? (bool?)null : reader.GetBoolean(13);
                            workerEditor.SixthRight = reader.IsDBNull(14) ? (bool?)null : reader.GetBoolean(14);

                            return workerEditor;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void AddWorker(WorkerEditor worker)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "INSERT INTO srv_pracownicy (ID_GRP, ID_POST, WYPLATA, MINUTY_SLUZBA, PRAWO_WYPLATA, PRAWO_DRZWI, PRAWO_REK, PRAWO_CZAT, PRAWO_MAGAZYN, PRAWO_1, PRAWO_2, PRAWO_3, PRAWO_4, PRAWO_5, PRAWO_6) VALUES (@P0, @P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10, @P11, @P12, @P13, @P14)";

                command.Parameters.Add(new MySqlParameter("@P0", worker.GID));
                command.Parameters.Add(new MySqlParameter("@P1", worker.CID));
                command.Parameters.Add(new MySqlParameter("@P2", worker.Salary));
                command.Parameters.Add(new MySqlParameter("@P3", worker.DutyMinutes));
                command.Parameters.Add(new MySqlParameter("@P4", worker.PaycheckRight));
                command.Parameters.Add(new MySqlParameter("@P5", worker.DoorsRight));
                command.Parameters.Add(new MySqlParameter("@P6", worker.RecrutationRight));
                command.Parameters.Add(new MySqlParameter("@P7", worker.ChatRight));
                command.Parameters.Add(new MySqlParameter("@P8", worker.OfferFromWarehouseRight));
                command.Parameters.Add(new MySqlParameter("@P9", worker.FirstRight));
                command.Parameters.Add(new MySqlParameter("@P10", worker.SecondRight));
                command.Parameters.Add(new MySqlParameter("@P11", worker.ThirdRight));
                command.Parameters.Add(new MySqlParameter("@P12", worker.FourthRight));
                command.Parameters.Add(new MySqlParameter("@P13", worker.FifthRight));
                command.Parameters.Add(new MySqlParameter("@P14", worker.SixthRight));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateWorker(WorkerEditor worker)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE srv_pracownicy SET WYPLATA = @P0, MINUTY_SLUZBA = @P1, PRAWO_WYPLATA = @P2, PRAWO_DRZWI = @P3, PRAWO_REK = @P4," +
                    " PRAWO_CZAT = @P5, PRAWO_MAGAZYN = @P6, PRAWO_1 = @P7, PRAWO_2 = @P8, PRAWO_3 = @P9, PRAWO_4 = @P10," +
                    " PRAWO_5 = @P11, PRAWO_6 = @P12, PRAWO_7 = @P13, PRAWO_8 = @P14 WHERE ID_PRAC = @ID";

                command.Parameters.Add(new MySqlParameter("@P0", worker.Salary));
                command.Parameters.Add(new MySqlParameter("@P1", worker.DutyMinutes));
                command.Parameters.Add(new MySqlParameter("@P2", worker.PaycheckRight));
                command.Parameters.Add(new MySqlParameter("@P3", worker.DoorsRight));
                command.Parameters.Add(new MySqlParameter("@P4", worker.RecrutationRight));
                command.Parameters.Add(new MySqlParameter("@P5", worker.ChatRight));
                command.Parameters.Add(new MySqlParameter("@P6", worker.OfferFromWarehouseRight));
                command.Parameters.Add(new MySqlParameter("@P7", worker.FirstRight));
                command.Parameters.Add(new MySqlParameter("@P8", worker.SecondRight));
                command.Parameters.Add(new MySqlParameter("@P9", worker.ThirdRight));
                command.Parameters.Add(new MySqlParameter("@P10", worker.FourthRight));
                command.Parameters.Add(new MySqlParameter("@P11", worker.FifthRight));
                command.Parameters.Add(new MySqlParameter("@P12", worker.SixthRight));
                command.Parameters.Add(new MySqlParameter("@P13", worker.SeventhRight));
                command.Parameters.Add(new MySqlParameter("@P14", worker.EightRight));

                command.Parameters.Add(new MySqlParameter("@ID", worker.WID));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteWorker(long wid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM srv_pracownicy WHERE ID_PRAC = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", wid));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public List<DescriptionList> SelectDescriptionsList(long cid)
        {
            List<DescriptionList> descriptions = new List<DescriptionList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT ID_OPIS, TYTUL, OPIS FROM srv_opisy WHERE ID_POST = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", cid));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            DescriptionList description = new DescriptionList
                            {
                                DID = reader.GetInt64(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2)

                            };

                            descriptions.Add(description);
                        }
                        return descriptions;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public DescriptionEditor SelectDescription(long did)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "SELECT ID_POST, TYTUL, OPIS FROM srv_przedmioty WHERE ID_OPIS = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", did));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            DescriptionEditor description = new DescriptionEditor
                            {
                                DID = did,
                                CID = reader.GetInt64(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2)

                            };
                            return description;
                        }

                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void AddDescription(DescriptionEditor description)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "INSERT INTO srv_opisy (ID_POST, TYTUL, OPIS) VALUES (@P0, @P1, @P2)";

                command.Parameters.Add(new MySqlParameter("@P0", description.CID));
                command.Parameters.Add(new MySqlParameter("@P1", description.Title));
                command.Parameters.Add(new MySqlParameter("@P2", description.Description));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateDescription(DescriptionEditor description)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE srv_opisy SET TYTUL = @P1, OPIS = @P2 WHERE UID_OPIS = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", description.DID));

                command.Parameters.Add(new MySqlParameter("@P1", description.Title));
                command.Parameters.Add(new MySqlParameter("@P2", description.Description));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteDescription(long UID)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM srv_opisy WHERE ID_OPIS = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", UID));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public List<TelephoneContactList> SelectContactsList(long tid)
        {
            List<TelephoneContactList> contacts = new List<TelephoneContactList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT ID_KONTAKT, NAZWA, NUMER FROM srv_kontakty WHERE ID_TEL = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", tid));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            TelephoneContactList description = new TelephoneContactList
                            {
                                TID = tid,
                                COID = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Number = reader.GetInt32(2)


                            };
                            contacts.Add(description);
                        }
                        return contacts;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void AddContact(TelephoneContactEditor contact)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "INSERT INTO srv_kontakty (ID_TEL, NAZWA, NUMER) VALUES (@P0, @P1, @P2)";

                command.Parameters.Add(new MySqlParameter("@P0", contact.TID));
                command.Parameters.Add(new MySqlParameter("@P1", contact.Name));
                command.Parameters.Add(new MySqlParameter("@P2", contact.Number));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateContact(TelephoneContactEditor contact)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE srv_opisy SET NAZWA = @P1, NUMER = @P2 WHERE ID_KONTAKT = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", contact.COID));

                command.Parameters.Add(new MySqlParameter("@P1", contact.Name));
                command.Parameters.Add(new MySqlParameter("@P2", contact.Number));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteContact(long coid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM srv_kontakty WHERE ID_KONTAKT = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", coid));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }


        public List<TelephoneMessageList> SelectMessagesList(long tid)
        {
            List<TelephoneMessageList> contacts = new List<TelephoneMessageList>();

            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT ID_WIAD, TRESC, NUMER_NAD FROM srv_wiadomosci WHERE ID_TEL = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", tid));

                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    try
                    {
                        while (reader.Read())
                        {
                            TelephoneMessageList description = new TelephoneMessageList
                            {
                                TID = tid,
                                MID = reader.GetInt64(0),
                                Contenet = reader.GetString(1),
                                SenderNumber = reader.GetInt32(2)

                            };

                            contacts.Add(description);
                        }
                        return contacts;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void AddMessage(TelephoneMessageEditor message)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "INSERT INTO srv_wiadomosci (ID_TEL, TRESC, NUMER_NAD) VALUES (@P0, @P1, @P2)";

                command.Parameters.Add(new MySqlParameter("@P0", message.TID));
                command.Parameters.Add(new MySqlParameter("@P1", message.Contenet));
                command.Parameters.Add(new MySqlParameter("@P2", message.SenderNumber));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateMessage(TelephoneMessageEditor message)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "UPDATE srv_wiadomosci SET TRESC = @P1, NUMER_NAD = @P2 WHERE ID_WIAD = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", message.TID));

                command.Parameters.Add(new MySqlParameter("@P1", message.Contenet));
                command.Parameters.Add(new MySqlParameter("@P2", message.SenderNumber));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteMessage(long mid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "DELETE FROM srv_wiadomosci WHERE ID_KONTAKT = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", mid));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public CrimeBotEditor SelectCrimeBot(long gid)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                //command.CommandText = @"SELECT ID_BOT, ID_GRP, NAZWA, 453432689_ILOSC, 453432689_CENA, 453432689_ILOSC_DOM, 1593441988_ILOSC, 1593441988_CENA, " +
                //                "1593441988_ILOSC_DOM, -1716589765_ILOSC, -1716589765_CENA, -1716589765_ILOSC_DOM, -1076751822_ILOSC, -1076751822_CENA, " +
                //                " -1076751822_ILOSC_DOM, -771403250_ILOSC, -771403250_CENA, -771403250_ILOSC_DOM, 137902532_ILOSC, 137902532_CENA, 137902532_ILOSC_DOM, " +
                //                " -1045183535_ILOSC, -1045183535_CENA, -1045183535_ILOSC_DOM, 324215364_ILOSC, 324215364_CENA, 324215364_ILOSC_DOM, -619010992_ILOSC, " +
                //                " -619010992_CENA, -619010992_ILOSC_DOM, -1121678507_ILOSC, -1121678507_CENA, -1121678507_ILOSC_DOM, -1074790547_ILOSC, -1074790547_CENA, -1074790547_ILOSC_DOM" +
                //                " -2084633992_ILOSC, -2084633992_CENA, -2084633992_ILOSC_DOM, 1649403952_ILOSC, 1649403952_CENA, 1649403952_ILOSC_DOM, 100416529_ILOSC, 100416529_CENA, 100416529_ILOSC_DOM" +
                //                " 487013001_ILOSC, 487013001_CENA, 487013001_ILOSC_DOM, 2017895192_ILOSC, 2017895192_CENA, 2017895192_ILOSC_DOM, -275439685_ILOSC, -275439685_CENA, -275439685_ILOSC_DOM FROM " +
                //                " srv_boty_przestepcze WHERE ID_GRP = @P0"

                command.CommandText = "SELECT * FROM srv_boty_przestepcze WHERE ID_GRP = @P0";
                command.Parameters.Add(new MySqlParameter("@P0", gid));

                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    try
                    {
                        if (reader.Read())
                        {
                            CrimeBotEditor editor = new CrimeBotEditor()
                            {
                                BotId = reader.GetInt64(0),
                                GroupId = reader.GetInt64(1),
                                Name = reader.GetString(2),
                                GunsCost = new Dictionary<string, decimal?>(),
                                GunsCount = new Dictionary<string, int?>(),
                                GunsDefaultCount = new Dictionary<string, int?>(),
                                AmmoCost = new Dictionary<string, decimal?>(),
                                AmmoCount = new Dictionary<string, int?>(),
                                AmmoDefaultCount = new Dictionary<string, int?>(),
                                DrugsCost = new Dictionary<string, decimal?>(),
                                DrugsCount = new Dictionary<string, int?>(),
                                DrugsDefaultCount = new Dictionary<string, int?>(),

                            };

                            for (int i = 3; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i)) continue;
                                if (reader.GetName(i).Contains("_ILOSC_DOM"))
                                {
                                    var name = reader.GetName(i);

                                    if (int.TryParse(name.Substring(0, name.IndexOf('_')), out int key))
                                    {
                                        editor.GunsDefaultCount.Add(key.ToString(), reader.GetInt32(i));
                                    }
                                    else if (name.Substring(0, name.IndexOf('_')).EndsWith("M"))
                                    {
                                        editor.AmmoDefaultCount.Add(name.Substring(0, name.IndexOf('_') - 1),
                                            reader.GetInt32(i));
                                    }
                                    else
                                    {
                                        editor.DrugsDefaultCount.Add(name.Substring(0, name.IndexOf('_') - 1),
                                            reader.GetInt32(i));
                                    }
                                }
                                else if (reader.GetName(i).Contains("_ILOSC"))
                                {
                                    var name = reader.GetName(i);

                                    if (int.TryParse(name.Substring(0, name.IndexOf('_')), out int key))
                                    {
                                        editor.GunsCount.Add(key.ToString(), reader.GetInt32(i));
                                    }
                                    else if (name.Substring(0, name.IndexOf('_')).EndsWith("M"))
                                    {
                                        editor.AmmoCount.Add(name.Substring(0, name.IndexOf('_') - 1), reader.GetInt32(i));
                                    }
                                    else
                                    {
                                        editor.DrugsCount.Add(name.Substring(0, name.IndexOf('_') - 1), reader.GetInt32(i));
                                    }
                                }
                                else if (reader.GetName(i).Contains("_CENA"))
                                {
                                    var name = reader.GetName(i);

                                    if (int.TryParse(name.Substring(0, name.IndexOf('_')), out int key))
                                    {
                                        editor.GunsCost.Add(key.ToString(), reader.GetInt32(i));
                                    }
                                    else if (name.Substring(0, name.IndexOf('_')).EndsWith("M"))
                                    {
                                        editor.AmmoCost.Add(name.Substring(0, name.IndexOf('_') - 1), reader.GetInt32(i));
                                    }
                                    else
                                    {
                                        editor.DrugsCost.Add(name.Substring(0, name.IndexOf('_') - 1), reader.GetInt32(i));
                                    }
                                }
                            }
                            return editor;
                        }

                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        reader.Close();
                    }
                }
            }
        }

        public void UpdateCrimeBot(CrimeBotEditor editor)
        {
            using (MySqlConnection connection = new MySqlConnection(ServerConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = @"UPDATE srv_boty_przestepcze SET ID_GRP = @P0, NAZWA = @P1, `453432689_ILOSC` = @P2, `453432689_CENA` = @P3, `453432689_ILOSC_DOM` = @P4, `1593441988_ILOSC` = @P5, `1593441988_CENA` = @P6," +
                                " `1593441988_ILOSC_DOM` = @P7, `-1716589765_ILOSC` = @P8, `-1716589765_CENA` = @P9, `-1716589765_ILOSC_DOM` = @P10, `-1076751822_ILOSC` = @P11, `-1076751822_CENA` = @P12," +
                                " `-1076751822_ILOSC_DOM` = @P13, `-771403250_ILOSC` = @P14, `-771403250_CENA` = @P15, `-771403250_ILOSC_DOM` = @P16, `137902532_ILOSC` = @P17, `137902532_CENA` = @P18, `137902532_ILOSC_DOM` = @P19," +
                                " `-1045183535_ILOSC` = @P20, `-1045183535_CENA` = @P21, `-1045183535_ILOSC_DOM` = @P22, `324215364_ILOSC` = @P23, `324215364_CENA` = @P24, `324215364_ILOSC_DOM` = @P25, `-619010992_ILOSC` = @P26," +
                                " `-619010992_CENA` = @P27, `-619010992_ILOSC_DOM` = @P28, `-1121678507_ILOSC` = @P29, `-1121678507_CENA` = @P30, `-1121678507_ILOSC_DOM` = @P31, `-1074790547_ILOSC` = @P32, `-1074790547_CENA` = @P33, `-1074790547_ILOSC_DOM` = @P34," +
                                " `-2084633992_ILOSC` = @P35, `-2084633992_CENA` = @P36, `-2084633992_ILOSC_DOM` = @P37, `1649403952_ILOSC` = @P38, `1649403952_CENA` = @P39, `1649403952_ILOSC_DOM` = @P40, `100416529_ILOSC` = @P41, `100416529_CENA` = @P42, `100416529_ILOSC_DOM` = @P43," +
                                " `487013001_ILOSC` = @P44, `487013001_CENA` = @P45, `487013001_ILOSC_DOM` = @P46, `2017895192_ILOSC` = @P47, `2017895192_CENA` = @P48, `2017895192_ILOSC_DOM` = @P49, `-275439685_ILOSC` = @P50, `-275439685_CENA` = @P51, `-275439685_ILOSC_DOM`  = @P52," +
                                " `453432689_ILOSC_DOM_M` = @P53, `1593441988_ILOSC_M` = @P54, `1593441988_CENA_M` = @P55, `1593441988_ILOSC_DOM_M` = @P57, `-1716589765_ILOSC_M` = @P58, `-1716589765_CENA_M` = @P59, `-1716589765_ILOSC_DOM_M` = @P60, `-1076751822_ILOSC_M` = @P61, `-1076751822_CENA_M` = @P62," +
                                " `-1076751822_ILOSC_DOM_M` = @P63, `-771403250_ILOSC_M` = @P64, `-771403250_CENA_M` = @P65, `-771403250_ILOSC_DOM_M` = @P66, `137902532_ILOSC_M` = @P67, `137902532_CENA_M` = @P68, `137902532_ILOSC_DOM_M` = @P69," +
                                " `-1045183535_ILOSC_M` = @P70, `-1045183535_CENA_M` = @P71, `-1045183535_ILOSC_DOM_M` = @P72, `324215364_ILOSC_M` = @P73, `324215364_CENA_M` = @P74, `324215364_ILOSC_DOM_M` = @P75, `-619010992_ILOSC_M` = @P76," +
                                " `-619010992_CENA_M` = @P77, `-619010992_ILOSC_DOM_M` = @P78, `-1121678507_ILOSC_M` = @P79, `-1121678507_CENA_M` = @P80, `-1121678507_ILOSC_DOM_M` = @P81, `-1074790547_ILOSC_M` = @P82, `-1074790547_CENA_M` = @P83, `-1074790547_ILOSC_DOM_M` = @P84," +
                                " `-2084633992_ILOSC_M` = @P85, `-2084633992_CENA_M` = @P86, `-2084633992_ILOSC_DOM_M` = @P87, `1649403952_ILOSC_M` = @P88, `1649403952_CENA_M` = @P89, `1649403952_ILOSC_DOM_M` = @P90, `100416529_ILOSC_M` = @P91, `100416529_CENA_M` = @P92, `100416529_ILOSC_DOM_M` = @P93," +
                                " `487013001_ILOSC_M` = @P94, `487013001_CENA_M` = @P95, `487013001_ILOSC_DOM_M` = @P96, `2017895192_ILOSC_M` = @P97, `2017895192_CENA_M` = @P98, `2017895192_ILOSC_DOM_M` = @P99, `-275439685_ILOSC_M` = @P100, `-275439685_CENA_M` = @P101, `-275439685_ILOSC_DOM_M` = @P102 WHERE ID_BOT = @ID";

                command.Parameters.Add(new MySqlParameter("@ID", editor.BotId));
                command.Parameters.Add(new MySqlParameter("@P0", editor.GroupId));
                command.Parameters.Add(new MySqlParameter("@P1", editor.Name));
                command.Parameters.Add(new MySqlParameter("@P2", editor.GunsCount.ContainsKey("453432689") ? editor.GunsCount["453432689"] : null));
                command.Parameters.Add(new MySqlParameter("@P3", editor.GunsCost.ContainsKey("453432689") ? editor.GunsCost["453432689"] : null));
                command.Parameters.Add(new MySqlParameter("@P4", editor.GunsDefaultCount.ContainsKey("453432689") ? editor.GunsDefaultCount["453432689"] : null));
                command.Parameters.Add(new MySqlParameter("@P5", editor.GunsCount.ContainsKey("1593441988") ? editor.GunsCount["1593441988"] : null));
                command.Parameters.Add(new MySqlParameter("@P6", editor.GunsCost.ContainsKey("1593441988") ? editor.GunsCost["1593441988"] : null));
                command.Parameters.Add(new MySqlParameter("@P7", editor.GunsDefaultCount.ContainsKey("1593441988") ? editor.GunsDefaultCount["1593441988"] : null));
                command.Parameters.Add(new MySqlParameter("@P8", editor.GunsCount.ContainsKey("-1716589765") ? editor.GunsCount["-1716589765"] : null));
                command.Parameters.Add(new MySqlParameter("@P9", editor.GunsCost.ContainsKey("-1716589765") ? editor.GunsCost["-1716589765"] : null));
                command.Parameters.Add(new MySqlParameter("@P10", editor.GunsDefaultCount.ContainsKey("-1716589765") ? editor.GunsDefaultCount["-1716589765"] : null));
                command.Parameters.Add(new MySqlParameter("@P11", editor.GunsCount.ContainsKey("-1076751822") ? editor.GunsCount["-1076751822"] : null));
                command.Parameters.Add(new MySqlParameter("@P12", editor.GunsCost.ContainsKey("-1076751822") ? editor.GunsCost["-1076751822"] : null));
                command.Parameters.Add(new MySqlParameter("@P13", editor.GunsDefaultCount.ContainsKey("-1076751822") ? editor.GunsDefaultCount["-1076751822"] : null));
                command.Parameters.Add(new MySqlParameter("@P14", editor.GunsCount.ContainsKey("-771403250") ? editor.GunsCount["-771403250"] : null));
                command.Parameters.Add(new MySqlParameter("@P15", editor.GunsCost.ContainsKey("-771403250") ? editor.GunsCost["-771403250"] : null));
                command.Parameters.Add(new MySqlParameter("@P16", editor.GunsDefaultCount.ContainsKey("-771403250") ? editor.GunsDefaultCount["-771403250"] : null));
                command.Parameters.Add(new MySqlParameter("@P17", editor.GunsCount.ContainsKey("137902532") ? editor.GunsCount["137902532"] : null));
                command.Parameters.Add(new MySqlParameter("@P18", editor.GunsCost.ContainsKey("137902532") ? editor.GunsCost["137902532"] : null));
                command.Parameters.Add(new MySqlParameter("@P19", editor.GunsDefaultCount.ContainsKey("137902532") ? editor.GunsDefaultCount["137902532"] : null));
                command.Parameters.Add(new MySqlParameter("@P20", editor.GunsCount.ContainsKey("-1045183535") ? editor.GunsCount["-1045183535"] : null));
                command.Parameters.Add(new MySqlParameter("@P21", editor.GunsCost.ContainsKey("-1045183535") ? editor.GunsCost["-1045183535"] : null));
                command.Parameters.Add(new MySqlParameter("@P22", editor.GunsDefaultCount.ContainsKey("-1045183535") ? editor.GunsDefaultCount["-1045183535"] : null));
                command.Parameters.Add(new MySqlParameter("@P23", editor.GunsCount.ContainsKey("324215364") ? editor.GunsCount["324215364"] : null));
                command.Parameters.Add(new MySqlParameter("@P24", editor.GunsCost.ContainsKey("324215364") ? editor.GunsCost["324215364"] : null));
                command.Parameters.Add(new MySqlParameter("@P25", editor.GunsDefaultCount.ContainsKey("324215364") ? editor.GunsDefaultCount["324215364"] : null));
                command.Parameters.Add(new MySqlParameter("@P26", editor.GunsCount.ContainsKey("-619010992") ? editor.GunsCount["-619010992"] : null));
                command.Parameters.Add(new MySqlParameter("@P27", editor.GunsCost.ContainsKey("-619010992") ? editor.GunsCost["-619010992"] : null));
                command.Parameters.Add(new MySqlParameter("@P28", editor.GunsDefaultCount.ContainsKey("-619010992") ? editor.GunsDefaultCount["-619010992"] : null));
                command.Parameters.Add(new MySqlParameter("@P29", editor.GunsCount.ContainsKey("-1121678507") ? editor.GunsCount["-1121678507"] : null));
                command.Parameters.Add(new MySqlParameter("@P30", editor.GunsCost.ContainsKey("-1121678507") ? editor.GunsCost["-1121678507"] : null));
                command.Parameters.Add(new MySqlParameter("@P31", editor.GunsDefaultCount.ContainsKey("-1121678507") ? editor.GunsDefaultCount["-1121678507"] : null));
                command.Parameters.Add(new MySqlParameter("@P32", editor.GunsCount.ContainsKey("-1074790547") ? editor.GunsCount["-1074790547"] : null));
                command.Parameters.Add(new MySqlParameter("@P33", editor.GunsCost.ContainsKey("-1074790547") ? editor.GunsCost["-1074790547"] : null));
                command.Parameters.Add(new MySqlParameter("@P34", editor.GunsDefaultCount.ContainsKey("-1074790547") ? editor.GunsDefaultCount["-1074790547"] : null));
                command.Parameters.Add(new MySqlParameter("@P35", editor.GunsCount.ContainsKey("-2084633992") ? editor.GunsCount["-2084633992"] : null));
                command.Parameters.Add(new MySqlParameter("@P36", editor.GunsCost.ContainsKey("-2084633992") ? editor.GunsCost["-2084633992"] : null));
                command.Parameters.Add(new MySqlParameter("@P37", editor.GunsDefaultCount.ContainsKey("-2084633992") ? editor.GunsDefaultCount["-2084633992"] : null));
                command.Parameters.Add(new MySqlParameter("@P38", editor.GunsCount.ContainsKey("-1649403952") ? editor.GunsCount["-1649403952"] : null));
                command.Parameters.Add(new MySqlParameter("@P39", editor.GunsCost.ContainsKey("-1649403952") ? editor.GunsCost["-1649403952"] : null));
                command.Parameters.Add(new MySqlParameter("@P40", editor.GunsDefaultCount.ContainsKey("-1649403952") ? editor.GunsDefaultCount["-1649403952"] : null));
                command.Parameters.Add(new MySqlParameter("@P41", editor.GunsCount.ContainsKey("100416529") ? editor.GunsCount["100416529"] : null));
                command.Parameters.Add(new MySqlParameter("@P42", editor.GunsCost.ContainsKey("100416529") ? editor.GunsCost["100416529"] : null));
                command.Parameters.Add(new MySqlParameter("@P43", editor.GunsDefaultCount.ContainsKey("100416529") ? editor.GunsDefaultCount["100416529"] : null));
                command.Parameters.Add(new MySqlParameter("@P44", editor.GunsCount.ContainsKey("487013001") ? editor.GunsCount["487013001"] : null));
                command.Parameters.Add(new MySqlParameter("@P45", editor.GunsCost.ContainsKey("487013001") ? editor.GunsCost["487013001"] : null));
                command.Parameters.Add(new MySqlParameter("@P46", editor.GunsDefaultCount.ContainsKey("487013001") ? editor.GunsDefaultCount["487013001"] : null));
                command.Parameters.Add(new MySqlParameter("@P47", editor.GunsCount.ContainsKey("2017895192") ? editor.GunsCount["2017895192"] : null));
                command.Parameters.Add(new MySqlParameter("@P48", editor.GunsCost.ContainsKey("2017895192") ? editor.GunsCost["2017895192"] : null));
                command.Parameters.Add(new MySqlParameter("@P49", editor.GunsDefaultCount.ContainsKey("2017895192") ? editor.GunsDefaultCount["2017895192"] : null));
                command.Parameters.Add(new MySqlParameter("@P50", editor.GunsCount.ContainsKey("-275439685") ? editor.GunsCount["-275439685"] : null));
                command.Parameters.Add(new MySqlParameter("@P51", editor.GunsCost.ContainsKey("-275439685") ? editor.GunsCost["-275439685"] : null));
                command.Parameters.Add(new MySqlParameter("@P52", editor.GunsDefaultCount.ContainsKey("-275439685") ? editor.GunsDefaultCount["-275439685"] : null));
                command.Parameters.Add(new MySqlParameter("@P53", editor.AmmoCount.ContainsKey("453432689") ? editor.AmmoCount["453432689"] : null));
                command.Parameters.Add(new MySqlParameter("@P54", editor.AmmoCost.ContainsKey("453432689") ? editor.AmmoCost["453432689"] : null));
                command.Parameters.Add(new MySqlParameter("@P55", editor.AmmoDefaultCount.ContainsKey("453432689") ? editor.AmmoDefaultCount["453432689"] : null));
                command.Parameters.Add(new MySqlParameter("@P56", editor.AmmoCount.ContainsKey("1593441988") ? editor.AmmoCount["1593441988"] : null));
                command.Parameters.Add(new MySqlParameter("@P57", editor.AmmoCost.ContainsKey("1593441988") ? editor.AmmoCost["1593441988"] : null));
                command.Parameters.Add(new MySqlParameter("@P58", editor.AmmoDefaultCount.ContainsKey("1593441988") ? editor.AmmoDefaultCount["1593441988"] : null));
                command.Parameters.Add(new MySqlParameter("@P59", editor.AmmoCount.ContainsKey("-1716589765") ? editor.AmmoCount["-1716589765"] : null));
                command.Parameters.Add(new MySqlParameter("@P60", editor.AmmoCost.ContainsKey("-1716589765") ? editor.AmmoCost["-1716589765"] : null));
                command.Parameters.Add(new MySqlParameter("@P61", editor.AmmoDefaultCount.ContainsKey("-1716589765") ? editor.AmmoDefaultCount["-1716589765"] : null));
                command.Parameters.Add(new MySqlParameter("@P62", editor.AmmoCount.ContainsKey("-1076751822") ? editor.AmmoCount["-1076751822"] : null));
                command.Parameters.Add(new MySqlParameter("@P63", editor.AmmoCost.ContainsKey("-1076751822") ? editor.AmmoCost["-1076751822"] : null));
                command.Parameters.Add(new MySqlParameter("@P64", editor.AmmoDefaultCount.ContainsKey("-1076751822") ? editor.AmmoDefaultCount["-1076751822"] : null));
                command.Parameters.Add(new MySqlParameter("@P65", editor.AmmoCount.ContainsKey("-771403250") ? editor.AmmoCount["-771403250"] : null));
                command.Parameters.Add(new MySqlParameter("@P66", editor.AmmoCost.ContainsKey("-771403250") ? editor.AmmoCost["-771403250"] : null));
                command.Parameters.Add(new MySqlParameter("@P67", editor.AmmoDefaultCount.ContainsKey("-771403250") ? editor.AmmoDefaultCount["-771403250"] : null));
                command.Parameters.Add(new MySqlParameter("@P68", editor.AmmoCount.ContainsKey("137902532") ? editor.AmmoCount["137902532"] : null));
                command.Parameters.Add(new MySqlParameter("@P69", editor.AmmoCost.ContainsKey("137902532") ? editor.AmmoCost["137902532"] : null));
                command.Parameters.Add(new MySqlParameter("@P70", editor.AmmoDefaultCount.ContainsKey("137902532") ? editor.AmmoDefaultCount["137902532"] : null));
                command.Parameters.Add(new MySqlParameter("@P71", editor.AmmoCount.ContainsKey("-1045183535") ? editor.AmmoCount["-1045183535"] : null));
                command.Parameters.Add(new MySqlParameter("@P72", editor.AmmoCost.ContainsKey("-1045183535") ? editor.AmmoCost["-1045183535"] : null));
                command.Parameters.Add(new MySqlParameter("@P73", editor.AmmoDefaultCount.ContainsKey("-1045183535") ? editor.AmmoDefaultCount["-1045183535"] : null));
                command.Parameters.Add(new MySqlParameter("@P74", editor.AmmoCount.ContainsKey("324215364") ? editor.AmmoCount["324215364"] : null));
                command.Parameters.Add(new MySqlParameter("@P75", editor.AmmoCost.ContainsKey("324215364") ? editor.AmmoCost["324215364"] : null));
                command.Parameters.Add(new MySqlParameter("@P76", editor.AmmoDefaultCount.ContainsKey("324215364") ? editor.AmmoDefaultCount["324215364"] : null));
                command.Parameters.Add(new MySqlParameter("@P77", editor.AmmoCount.ContainsKey("-619010992") ? editor.AmmoCount["-619010992"] : null));
                command.Parameters.Add(new MySqlParameter("@P78", editor.AmmoCost.ContainsKey("-619010992") ? editor.AmmoCost["-619010992"] : null));
                command.Parameters.Add(new MySqlParameter("@P79", editor.AmmoDefaultCount.ContainsKey("-619010992") ? editor.AmmoDefaultCount["-619010992"] : null));
                command.Parameters.Add(new MySqlParameter("@P80", editor.AmmoCount.ContainsKey("-1121678507") ? editor.AmmoCount["-1121678507"] : null));
                command.Parameters.Add(new MySqlParameter("@P81", editor.AmmoCost.ContainsKey("-1121678507") ? editor.AmmoCost["-1121678507"] : null));
                command.Parameters.Add(new MySqlParameter("@P82", editor.AmmoDefaultCount.ContainsKey("-1121678507") ? editor.AmmoDefaultCount["-1121678507"] : null));
                command.Parameters.Add(new MySqlParameter("@P83", editor.AmmoCount.ContainsKey("-1074790547") ? editor.AmmoCount["-1074790547"] : null));
                command.Parameters.Add(new MySqlParameter("@P84", editor.AmmoCost.ContainsKey("-1074790547") ? editor.AmmoCost["-1074790547"] : null));
                command.Parameters.Add(new MySqlParameter("@P85", editor.AmmoDefaultCount.ContainsKey("-1074790547") ? editor.AmmoDefaultCount["-1074790547"] : null));
                command.Parameters.Add(new MySqlParameter("@P86", editor.AmmoCount.ContainsKey("-2084633992") ? editor.AmmoCount["-2084633992"] : null));
                command.Parameters.Add(new MySqlParameter("@P87", editor.AmmoCost.ContainsKey("-2084633992") ? editor.AmmoCost["-2084633992"] : null));
                command.Parameters.Add(new MySqlParameter("@P88", editor.AmmoDefaultCount.ContainsKey("-2084633992") ? editor.AmmoDefaultCount["-2084633992"] : null));
                command.Parameters.Add(new MySqlParameter("@P89", editor.AmmoCount.ContainsKey("-1649403952") ? editor.AmmoCount["-1649403952"] : null));
                command.Parameters.Add(new MySqlParameter("@P90", editor.AmmoCost.ContainsKey("-1649403952") ? editor.AmmoCost["-1649403952"] : null));
                command.Parameters.Add(new MySqlParameter("@P91", editor.AmmoDefaultCount.ContainsKey("-1649403952") ? editor.AmmoDefaultCount["-1649403952"] : null));
                command.Parameters.Add(new MySqlParameter("@P92", editor.AmmoCount.ContainsKey("100416529") ? editor.AmmoCount["100416529"] : null));
                command.Parameters.Add(new MySqlParameter("@P93", editor.AmmoCost.ContainsKey("100416529") ? editor.AmmoCost["100416529"] : null));
                command.Parameters.Add(new MySqlParameter("@P94", editor.AmmoDefaultCount.ContainsKey("100416529") ? editor.AmmoDefaultCount["100416529"] : null));
                command.Parameters.Add(new MySqlParameter("@P95", editor.AmmoCount.ContainsKey("487013001") ? editor.AmmoCount["487013001"] : null));
                command.Parameters.Add(new MySqlParameter("@P96", editor.AmmoCost.ContainsKey("487013001") ? editor.AmmoCost["487013001"] : null));
                command.Parameters.Add(new MySqlParameter("@P97", editor.AmmoDefaultCount.ContainsKey("487013001") ? editor.AmmoDefaultCount["487013001"] : null));
                command.Parameters.Add(new MySqlParameter("@P98", editor.AmmoCount.ContainsKey("2017895192") ? editor.AmmoCount["2017895192"] : null));
                command.Parameters.Add(new MySqlParameter("@P99", editor.AmmoCost.ContainsKey("2017895192") ? editor.AmmoCost["2017895192"] : null));
                command.Parameters.Add(new MySqlParameter("@P100", editor.AmmoDefaultCount.ContainsKey("2017895192") ? editor.AmmoDefaultCount["2017895192"] : null));
                command.Parameters.Add(new MySqlParameter("@P101", editor.AmmoCount.ContainsKey("-275439685") ? editor.AmmoCount["-275439685"] : null));
                command.Parameters.Add(new MySqlParameter("@P102", editor.AmmoCost.ContainsKey("-275439685") ? editor.AmmoCost["-275439685"] : null));
                command.Parameters.Add(new MySqlParameter("@P103", editor.AmmoDefaultCount.ContainsKey("-275439685") ? editor.AmmoDefaultCount["-275439685"] : null));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
