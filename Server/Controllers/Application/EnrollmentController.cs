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
    public class EnrollmentController : BaseController<Enrollment>, iBaseController<Enrollment>
    {
        public EnrollmentController(SWARMOracleContext context,
                                IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {

        }
        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Enrollment> lstEnrollments = await _context.Enrollments.OrderBy(x => x.StudentId).ToListAsync();
            return Ok(lstEnrollments);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Get/{section}/{student}")]
        public async Task<IActionResult> Get(int section, int student)
        {
            Enrollment itmEnrollment = await _context.Enrollments.Where(x => x.SectionId == section && x.StudentId == student).FirstOrDefaultAsync();
            return Ok(itmEnrollment);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("Delete/{section}/{student}")]
        public async Task<IActionResult> Delete(int section, int student)
        {
            Enrollment itmEnrollment = await _context.Enrollments.Where(x => x.SectionId == section && x.StudentId == student).FirstOrDefaultAsync();
            _context.Remove(itmEnrollment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Enrollment input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var e = await _context.Enrollments.Where(x => x.StudentId == input.StudentId &&
                                                             x.SectionId == input.SectionId).FirstOrDefaultAsync();
                if (e != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Grade Already Exists");
                }
                e = new Enrollment();
                e.EnrollDate = input.EnrollDate;
                e.FinalGrade = input.FinalGrade;
                e.SchoolId = input.SchoolId;
                e.SectionId = input.SectionId;
                e.StudentId = input.StudentId;
                _context.Enrollments.Add(e);
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

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Enrollment input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var e = await _context.Enrollments.Where(x => x.StudentId == input.StudentId &&
                                                             x.SectionId == input.SectionId).FirstOrDefaultAsync();
                if (e == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok();
                }
                e.EnrollDate = input.EnrollDate;
                e.FinalGrade = input.FinalGrade;
                e.SchoolId = input.SchoolId;
                e.SectionId = input.SectionId;
                e.StudentId = input.StudentId;
                _context.Enrollments.Update(e);
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
