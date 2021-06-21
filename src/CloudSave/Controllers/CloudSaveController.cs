using Dapr.Client;
using GamedevDapr.CloudSave.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace GamedevDapr.CloudSave.Controllers
{
    /**
     * Use "Buffering" instead of "Streaming" for the beginning, 
     * this will crash if upload size of file or number of parallel uploads exhaust available memory. 
     * From Documention "Any single buffered file exceeding 64 KB is moved from memory to a temp file on disk."
     */
    [ApiController]
    [Route("[Controller]")]
    public class CloudSaveController : ControllerBase
    {
        private readonly ILogger<CloudSaveController> _logger;
        private readonly string daprStateStoreName = "cloudsave";

        public CloudSaveController(ILogger<CloudSaveController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Saves a provided list of save files through the apropiate repository
        /// </summary>
        /// <param name="saveGames">List of Save Games Based on <see cref="GamedevDapr.CloudSave.Entities.SaveGameEntity"/></param>
        /// <returns></returns>
        [HttpPost("SaveGameEntity")]
        public async Task<IActionResult> Upload([FromForm] SaveGameEntity saveGame, [FromServices] DaprClient daprClient, CancellationToken cancellationToken)
        {
            var (state, originalETag) = await daprClient.GetStateAndETagAsync<SaveGameEntity>(daprStateStoreName,
                                                             saveGame.ToString(),
                                                             cancellationToken: cancellationToken);

            if (state.Id == null)
            {
                await daprClient.SaveStateAsync<SaveGameEntity>(daprStateStoreName, saveGame.ToString(), saveGame, cancellationToken: cancellationToken);
            }
            else
            {
                var isSaveStateSuccessfull = await daprClient.TrySaveStateAsync<SaveGameEntity>(daprStateStoreName, saveGame.ToString(), saveGame, originalETag, cancellationToken: cancellationToken);

                //For now, just logg away that we did run into a ETag conflict.
                if (!isSaveStateSuccessfull)
                {
                    _logger.LogWarning("CloudSave, Saving game unseccessfull due to ETag Conflict for Save: " + saveGame.ToString());
                    return BadRequest(new { message = "Could not save file, due to ETag Conflict" });
                }

            }

            return Ok(new { message = "UUID: " + saveGame.Id.ToString() + ", Name: " + saveGame.Name + " Data Name: " + saveGame.data.FileName });
        }

    }
}