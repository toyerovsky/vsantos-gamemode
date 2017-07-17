/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Runtime.Serialization;

namespace Serverside.Database.Exceptions
{
        [Serializable]
        public class RoleplayConnectionException : Exception
        {
            //
            // For guidelines regarding the creation of new exception types, see
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
            // and
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
            //

            public RoleplayConnectionException()
            {
            }

            public RoleplayConnectionException(string message) : base(message)
            {
            }

            public RoleplayConnectionException(string message, Exception inner) : base(message, inner)
            {
            }

            protected RoleplayConnectionException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }
    
}