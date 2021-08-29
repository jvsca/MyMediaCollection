﻿#region Usings

using Dapper.Contrib.Extensions;

using MyMediaCollection.Enums;

#endregion

namespace MyMediaCollection.Model
{
    public class Medium
    {

        #region Properties

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ItemType MediaType { get; set; }

        #endregion

    }
}
