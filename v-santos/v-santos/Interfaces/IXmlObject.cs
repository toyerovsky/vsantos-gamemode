/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

namespace Serverside.Interfaces
{
    public interface IXmlObject
    {
        string FilePath { get; set; }
        string CreatorForumName { get; set; }
    }
}