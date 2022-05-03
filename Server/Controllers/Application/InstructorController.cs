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
    public class InstructorController : BaseController<Instructor>, iBaseController<Instructor>
    {

        public InstructorController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Instructor> lstInstructor = await _context.Instructors.OrderBy(x => x.SchoolId).ThenBy(x => x.InstructorId).ToListAsync();
            return Ok(lstInstructor);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Get/{school}/{instructor}")]
        public async Task<IActionResult> Get(int school, int instructor)
        {
            Instructor i = await _context.Instructors.Where(x => x.SchoolId == school && x.InstructorId == instructor).FirstOrDefaultAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("Delete/{school}/{instructor}")]
        public async Task<IActionResult> Delete(int school, int instructor)
        {
            Instructor itmInstructor = await _context.Instructors.Where(x => x.SchoolId == school && x.InstructorId == instructor).FirstOrDefaultAsync();
            _context.Remove(itmInstructor);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Instructor input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var i = await _context.Instructors.Where(x => x.InstructorId == input.InstructorId).FirstOrDefaultAsync();

                if (i != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Instructor Already Exists.");
                }

                i = new Instructor();

                i.Zip = input.Zip;
                i.FirstName = input.FirstName;
                i.InstructorId = input.InstructorId;
                i.LastName = input.LastName;
                i.Phone = input.Phone;
                i.Salutation = input.Salutation;
                i.SchoolId = input.SchoolId;
                i.StreetAddress = input.StreetAddress;
                _context.Instructors.Add(i);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.InstructorId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Instructor input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var i = await _context.Instructors.Where(x => x.InstructorId == input.InstructorId).FirstOrDefaultAsync();

                if (i == null)
                {
                    await this.Post(input);
                    return Ok();
                }
                i.Zip = input.Zip;
                i.FirstName = input.FirstName;
                i.InstructorId = input.InstructorId;
                i.LastName = input.LastName;
                i.Phone = input.Phone;
                i.Salutation = input.Salutation;
                i.SchoolId = input.SchoolId;
                i.StreetAddress = input.StreetAddress;
                _context.Instructors.Update(i);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.InstructorId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}



