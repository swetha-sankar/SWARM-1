using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWARM.EF.Data;
using SWARM.EF.Models;
using SWARM.Server.Controllers.Base;
using SWARM.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;

namespace SWARM.Server.Controllers.Application
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZipcodeController : BaseController<Zipcode>, iBaseController<Zipcode>
    {

        public ZipcodeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Zipcode> lstZips = await _context.Zipcodes.OrderBy(x => x.Zip).ToListAsync();
            return Ok(lstZips);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(String input)
        {
            Zipcode itmZip = await _context.Zipcodes.Where(x => x.Zip == input).FirstOrDefaultAsync();
            return Ok(itmZip);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(String input)
        {
            Zipcode itmZip = await _context.Zipcodes.Where(x => x.Zip.Equals(input)).FirstOrDefaultAsync();
            _context.Zipcodes.Remove(itmZip);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Zipcode input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var z = await _context.Zipcodes.Where(x => x.Zip == input.Zip).FirstOrDefaultAsync();

                if (z != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Zip Already Exists");
                }

                z = new Zipcode();
                z.Zip = input.Zip;
                z.City = input.City;
                z.State = input.State;
                _context.Zipcodes.Add(z);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(input.Zip);
            }
            catch (Exception e)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Zipcode input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var z = await _context.Zipcodes.Where(x => x.Zip == input.Zip).FirstOrDefaultAsync();

                if ( z == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok(input.Zip);
                }

                z.City = input.City;
                z.State = input.State;
                z.Zip = input.Zip;
                _context.Zipcodes.Update(z);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.Zip);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}



