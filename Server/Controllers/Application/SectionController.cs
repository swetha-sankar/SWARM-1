using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWARM.EF.Data;
using SWARM.EF.Models;
using SWARM.Server.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWARM.Server.Controllers.Application
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : BaseController<Section>, iBaseController<Section>
    {
        public SectionController(SWARMOracleContext context,
                                IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {

        }
        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Section> lstSections = await _context.Sections.OrderBy(x => x.SectionId).ToListAsync();
            return Ok(lstSections);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            Section itmSection = await _context.Sections.Where(x => x.SectionId == input).FirstOrDefaultAsync();
            return Ok(itmSection);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            Section itmSection = await _context.Sections.Where(x => x.CourseNo == input).FirstOrDefaultAsync();
            _context.Remove(itmSection);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Section input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var s = await _context.Sections.Where(x => x.SectionId == input.SectionId).FirstOrDefaultAsync();

                if (s != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Section Already Exists");
                }
                s = new Section();
                s.Capacity = input.Capacity;
                s.CourseNo = input.CourseNo;
                s.InstructorId = input.InstructorId;
                s.Location = input.Location;
                s.SchoolId = input.SchoolId;
                s.SectionId = input.SectionId;
                s.SectionNo = input.SectionNo;
                _context.Sections.Add(s);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.SectionId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Section input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var s = await _context.Sections.Where(x => x.SectionId == input.SectionId).FirstOrDefaultAsync();
                if (s == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok(input.CourseNo);
                }
                s = new Section();
                s.Capacity = input.Capacity;
                s.CourseNo = input.CourseNo;
                s.InstructorId = input.InstructorId;
                s.Location = input.Location;
                s.SchoolId = input.SchoolId;
                s.SectionId = input.SectionId;
                s.SectionNo = input.SectionNo;
                _context.Sections.Update(s);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(input.SectionId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}