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
    public class GradeController : BaseController<Grade>, iBaseController<Grade>
    {
        public GradeController(SWARMOracleContext context,
                                IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {

        }
        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Grade> lstGrades = await _context.Grades.OrderBy(x => x.StudentId).ToListAsync();
            return Ok(lstGrades);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Get/{section}/{student}/{school}/{code}/{occurrence}")]
        public async Task<IActionResult> Get(int section, int student, int school, string code, int occurrence)
        {
            Grade g = await _context.Grades.Where(x => x.SectionId == section && x.StudentId == student && x.SchoolId == school && x.GradeCodeOccurrence == occurrence && x.GradeTypeCode == code).FirstOrDefaultAsync();
            return Ok(g);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("Delete/{section}/{student}/{school}/{code}/{occurrence}")]
        public async Task<IActionResult> Delete(int section, int student, int school, string code, int occurrence)
        {
            Grade g = await _context.Grades.Where(x => x.SectionId == section && x.StudentId == student && x.SchoolId == school && x.GradeCodeOccurrence == occurrence && x.GradeTypeCode == code).FirstOrDefaultAsync();
            _context.Remove(g);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Grade input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var g = await _context.Grades.Where(x => x.StudentId == input.StudentId &&
                                                            x.SectionId == input.SectionId &&
                                                            x.GradeTypeCode == input.GradeTypeCode &&
                                                            x.GradeCodeOccurrence == input.GradeCodeOccurrence).FirstOrDefaultAsync();
                if (g != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Grade Already Exists");
                }
                g = new Grade();
                g.Comments = input.Comments;
                g.GradeCodeOccurrence = input.GradeCodeOccurrence;
                g.GradeTypeCode = input.GradeTypeCode;
                g.GradeTypeWeight = input.GradeTypeWeight;
                g.NumericGrade = input.NumericGrade;
                g.SchoolId = input.SchoolId;
                g.SectionId = input.SectionId;
                g.StudentId = input.StudentId;
                _context.Grades.Add(g);
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
        public async Task<IActionResult> Put([FromBody] Grade input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var g = await _context.Grades.Where(x => x.SectionId == input.SectionId && x.StudentId == input.StudentId && x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode && x.GradeCodeOccurrence == input.GradeCodeOccurrence).FirstOrDefaultAsync();
                if (g == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok();
                }
                g.StudentId = input.StudentId;
                g.SectionId = input.SectionId;
                g.GradeTypeCode = input.GradeTypeCode;
                g.GradeCodeOccurrence = input.GradeCodeOccurrence;
                g.SchoolId = input.SchoolId;
                _context.Grades.Update(g);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
