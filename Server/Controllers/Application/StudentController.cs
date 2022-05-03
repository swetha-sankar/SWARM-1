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
    public class StudentController : BaseController<Student>, iBaseController<Student>
    {

        public StudentController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Student> lstS = await _context.Students.OrderBy(x => x.StudentId).ToListAsync();
            return Ok(lstS);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            Student s = await _context.Students.Where(x => x.StudentId == input).FirstOrDefaultAsync();
            return Ok(s);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            Student s = await _context.Students.Where(x => x.StudentId == input).FirstOrDefaultAsync();
            _context.Remove(s);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var s = await _context.Students.Where(x => x.StudentId == input.StudentId).FirstOrDefaultAsync();

                if (s != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Student Already Exists");
                }

                s = new Student();
                s.StudentId = input.StudentId;
                s.SchoolId = input.SchoolId;
                s.LastName = input.LastName;
                s.Zip = input.Zip;
                s.RegistrationDate = input.RegistrationDate;
                _context.Students.Add(s);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(input.StudentId);
            }
            catch (Exception e)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Student input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var s = await _context.Students.Where(x => x.StudentId == input.StudentId).FirstOrDefaultAsync();

                if (s == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok(input.StudentId);
                }

                s.StudentId = input.StudentId;
                s.SchoolId = input.SchoolId;
                s.LastName = input.LastName;
                s.Zip = input.Zip;
                s.RegistrationDate = input.RegistrationDate;
                _context.Students.Update(s);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.StudentId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}



