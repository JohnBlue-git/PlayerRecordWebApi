using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using PlayerRecordsApi.Models;



namespace PlayerRecordsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerRecordsRESTfulStyleController : ControllerBase
    {
        public PlayerRecordsRESTfulStyleController() {}

        // GET: api/Player
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicPlayer>>> GetPlayer()
        {
            return await PlayerRecords.GetPlayerAsync();
        }

        // GET: api/Player/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PublicPlayer>> GetPlayer(long id)
        {
            if (id < 0)
            {
                ModelState.AddModelError("Id", $"Id: {id} < 0");
                return BadRequest(ModelState);
            }

            PublicPlayer player = await PlayerRecords.GetPlayerAsync(id);

            if (player == null)
            {
                return NotFound($"Player record with Id: {id} not exist");
            }
            return player;
        }

        // PUT: api/Player/{id}
        // modify
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(long id, PublicPlayer player)
        {
            if (id < 0)
            {
                ModelState.AddModelError("Id", $"Id: {id} < 0");
                return BadRequest(ModelState);
            }
            else if (id != player.Id)
            {
                ModelState.AddModelError("Id", $"Id: {id} != player.Id");
                return BadRequest(ModelState);
            }
            else if (await PlayerRecords.IsPlayerExistsAsync(player.Id) == false)
            {
                return NotFound($"Player record with Id: {id} not exist");
            }            

            await PlayerRecords.PutPlayerAsync(player);
            return Ok();
        }

        // POST: api/Player
        // new one
        [HttpPost]
        public async Task<ActionResult<PublicPlayer>> PostPlayer(PublicPlayer player)
        {
            if (player.Id < 0)
            {
                ModelState.AddModelError("Id", $"Id: {player.Id} < 0");
                return BadRequest(ModelState);
            }
            else if (await PlayerRecords.IsPlayerExistsAsync(player.Id) == true)
            {
                ModelState.AddModelError("Id", $"Player record with Id: {player.Id} already exist");
                return BadRequest(ModelState);
            }

            await PlayerRecords.PostPlayerAsync(player);
            return CreatedAtAction(nameof(PublicPlayer)
                                  , new { id = player.Id }
                                  , player);
        }

        // DELETE: api/Player/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(long id)
        {
            if (id < 0)
            {
                ModelState.AddModelError("Id", $"Id: {id} < 0");
                return BadRequest(ModelState);
            }
            else if (await PlayerRecords.IsPlayerExistsAsync(id) == false)
            {
                return NotFound($"Player record with Id: {id} not exist");
            }

            await PlayerRecords.DeletePlayerAsync(id);
            return Ok();
        }

        [HttpPut]
        [Route("MarkRecordSecret{id}")]
        public async Task<IActionResult> MarkRecordSecret(long id)
        {
            if (id < 0)
            {
                ModelState.AddModelError("Id", $"Id: {id} < 0");
                return BadRequest(ModelState);
            }
            else if (await PlayerRecords.IsPlayerExistsAsync(id) == false)
            {
                return NotFound($"Player record with Id: {id} not exist");
            }

            await PlayerRecords.MarkRecordSecretAsync(id);

            return Ok();
        }
    }


    [ApiController]
    [Consumes("application/x-www-form-urlencoded")]
    //[Produces("application/json")]    
    public class PlayerRecordsRPCStyleController : ControllerBase
    {
        public PlayerRecordsRPCStyleController() {}

        [HttpGet]
        [Route("api/PlayerRecordsRPCStyl/GetSummary")]
        public async Task<ActionResult<IEnumerable<PublicPlayer>>> GetSummary()
        {
            return await PlayerRecords.GetSummaryAsync();
        }

        [HttpGet]
        [Route("api/PlayerRecordsRPCStyl/GetGetPlayerPointsSummary")]        
        public async Task<ActionResult<int>> GetGetPlayerPointsSummary([FromForm] string name)
        {
            return await PlayerRecords.GetPlayerPointsSummaryAsync(name);
        }
    }
}
