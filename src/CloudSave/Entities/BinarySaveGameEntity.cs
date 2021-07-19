using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamedevDapr.CloudSave.Entities
{
    public class BinarySaveGameEntity
    {
        public BinarySaveGameEntity(SaveGameEntity saveGame)
        {
            Id = saveGame.Id;
            Name = saveGame.Name;
            data = saveGame.GetData().Result;
        }

        /// <summary>
        ///     Uniquie ID of the save game
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     name of this specific save game
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     data of the savegame
        /// </summary>
        public byte[] data { get; set; }

        public override string ToString()
        {
            return Id + Name;
        }
    }
}
