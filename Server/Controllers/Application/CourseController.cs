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
    public class CourseController : BaseController<Course>, iBaseController<Course>
    {

        public CourseController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Course> lstCourses = await _context.Courses.OrderBy(x => x.CourseNo).ToListAsync();
            return Ok(lstCourses);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            Course itmCourse = await _context.Courses.Where(x => x.CourseNo == input).FirstOrDefaultAsync();
            return Ok(itmCourse);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            Course itmCourse = await _context.Courses.Where(x => x.CourseNo == input).FirstOrDefaultAsync();
            _context.Remove(itmCourse);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Course input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var c = await _context.Courses.Where(x => x.CourseNo == input.CourseNo).FirstOrDefaultAsync();

                if (c != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Course Already Exists");
                }

                c = new Course();
                c.Cost = input.Cost;
                c.Description = input.Description;
                c.Prerequisite = input.Prerequisite;
                c.PrerequisiteSchoolId = input.PrerequisiteSchoolId;
                c.SchoolId = input.SchoolId;
                _context.Courses.Add(c);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(input.CourseNo);
            }
            catch (Exception e)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Course input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var c = await _context.Courses.Where(x => x.CourseNo == input.CourseNo).FirstOrDefaultAsync();

                if (c == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok(input.CourseNo);
                }

                c.Cost = input.Cost;
                c.Description = input.Description;
                c.Prerequisite = input.Prerequisite;
                c.PrerequisiteSchoolId = input.PrerequisiteSchoolId;
                c.SchoolId = input.SchoolId;
                _context.Courses.Update(c);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.CourseNo);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}



