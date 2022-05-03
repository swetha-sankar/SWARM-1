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
    public class SchoolController : BaseController<School>, iBaseController<School>
    {

        public SchoolController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<School> s = await _context.Schools.OrderBy(x => x.SchoolId).ToListAsync();
            return Ok(s);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            School itmCourse = await _context.Schools.Where(x => x.SchoolId == input).FirstOrDefaultAsync();
            return Ok(itmCourse);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            School itmCourse = await _context.Schools.Where(x => x.SchoolId == input).FirstOrDefaultAsync();
            _context.Remove(itmCourse);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] School input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var c = await _context.Schools.Where(x => x.SchoolId == input.SchoolId).FirstOrDefaultAsync();

                if (c != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "School Already Exists");
                }

                c = new School();
                c.SchoolId = input.SchoolId;
                c.SchoolName = input.SchoolName;
                _context.Schools.Add(c);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(input.SchoolId);
            }
            catch (Exception e)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] School input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var c = await _context.Schools.Where(x => x.SchoolId == input.SchoolId).FirstOrDefaultAsync();

                if (c == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok(input.SchoolId);
                }

                c.SchoolId = input.SchoolId;
                c.SchoolName = input.SchoolName;
                _context.Schools.Update(c);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.SchoolId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}



