using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GamedevDapr.CloudSave.Entities
{
    /// <summary>
    ///     Expected format of a save game
    /// </summary>
    public class SaveGameEntity
    {
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
        public IFormFile data { get; set; }

        public async Task<byte[]> GetData()
        {
            using (var memoryStream = new MemoryStream())
            {
                await data.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
         }

        public override string ToString()
        {
            return Id + Name;
        }
    }
}
