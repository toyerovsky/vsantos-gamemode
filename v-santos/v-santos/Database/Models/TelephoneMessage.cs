/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class TelephoneMessage
    {
        [Key]
        //ID wiadomości
        public long Id { get; set; }
        //Id telefonu czyli id przedmiotu telefonu
        public int PhoneNumber { get; set; }
        public string Content { get; set; }
        public int SenderNumber { get; set; }
    }
}