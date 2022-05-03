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
    public class GradeConversionController : BaseController<GradeConversion>, iBaseController<GradeConversion>
    {
        public GradeConversionController(SWARMOracleContext context,
                                IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {

        }
        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<GradeConversion> lstGrade = await _context.GradeConversions.OrderBy(x => x.SchoolId).ThenBy(x => x.LetterGrade).ToListAsync();
            return Ok(lstGrade);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Get/{school}/{grade}")]
        public async Task<IActionResult> Get(int school, String grade)
        {
            GradeConversion g = await _context.GradeConversions.Where(x => x.SchoolId == school && x.LetterGrade.Equals(grade)).FirstOrDefaultAsync();
            return Ok(g);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("Delete/{school}/{grade}")]
        public async Task<IActionResult> Delete(int school, String grade)
        {
            GradeConversion g = await _context.GradeConversions.Where(x => x.SchoolId == school && x.LetterGrade == grade).FirstOrDefaultAsync();
            _context.Remove(g);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GradeConversion input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var g = await _context.GradeConversions.Where(x => x.LetterGrade == input.LetterGrade).FirstOrDefaultAsync();

                if (g != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Grade Conversion Already Exists.");
                }

                g = new GradeConversion();
                g.GradePoint = input.GradePoint;
                g.LetterGrade = input.LetterGrade;
                g.MaxGrade = input.MaxGrade;
                g.MinGrade = input.MinGrade;
                g.SchoolId = input.SchoolId;
                _context.GradeConversions.Add(g);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(input.LetterGrade);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] GradeConversion input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var g = await _context.GradeConversions.Where(x => x.LetterGrade == input.LetterGrade).FirstOrDefaultAsync();
                if (g == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok();
                }
                g.GradePoint = input.GradePoint;
                g.LetterGrade = input.LetterGrade;
                g.MaxGrade = input.MaxGrade;
                g.MinGrade = input.MinGrade;
                g.SchoolId = input.SchoolId;
                _context.GradeConversions.Update(g);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(input.LetterGrade);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
