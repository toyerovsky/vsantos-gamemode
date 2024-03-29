﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Runtime.Serialization;

namespace Serverside.Exceptions
{
    [Serializable]
    public class AccountNotLoggedException : Exception
    {
        public AccountNotLoggedException()
        {
        }

        public AccountNotLoggedException(string message) : base(message)
        {
        }

        public AccountNotLoggedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AccountNotLoggedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}