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
    public class GradeTypeWeightController : BaseController<GradeTypeWeight>, iBaseController<GradeTypeWeight>
    {

        public GradeTypeWeightController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<GradeTypeWeight> lstGradeTypeWeight = await _context.GradeTypeWeights.OrderBy(x => x.SchoolId).ThenBy(x => x.SectionId).ThenBy(x => x.GradeTypeCode).ToListAsync();
            return Ok(lstGradeTypeWeight);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Get/{school}/{section}/{type}")]
        public async Task<IActionResult> Get(int school, int section, String type)
        {
            GradeTypeWeight itmGradeTypeWeight = await _context.GradeTypeWeights.Where(x => x.SchoolId == school && x.SectionId == section && x.GradeTypeCode == type).FirstOrDefaultAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("Delete/{school}/{section}/{type}")]
        public async Task<IActionResult> Delete(int school, int section, String type)
        {
            GradeTypeWeight itmGradeTypeWeight = await _context.GradeTypeWeights.Where(x => x.SchoolId == school && x.SectionId == section && x.GradeTypeCode == type).FirstOrDefaultAsync();
            _context.Remove(itmGradeTypeWeight);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GradeTypeWeight input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var i = await _context.GradeTypeWeights.Where(x => x.SectionId == input.SectionId && x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode).FirstOrDefaultAsync();

                if (i != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Grade type weight Already Exists.");
                }

                i = new GradeTypeWeight();
                i.NumberPerSection = input.NumberPerSection;
                i.PercentOfFinalGrade = input.PercentOfFinalGrade;
                i.DropLowest = input.DropLowest;
                i.SectionId = input.SectionId;
                i.GradeTypeCode = input.GradeTypeCode;
                i.SchoolId = input.SchoolId;
                _context.GradeTypeWeights.Add(i);
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

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] GradeTypeWeight input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var i = await _context.GradeTypeWeights.Where(x => x.SectionId == input.SectionId && x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode).FirstOrDefaultAsync();
                if (i == null)
                {
                    await this.Post(input);
                    return Ok();
                }
                i.NumberPerSection = input.NumberPerSection;
                i.PercentOfFinalGrade = input.PercentOfFinalGrade;
                i.DropLowest = input.DropLowest;
                i.SectionId = input.SectionId;
                i.GradeTypeCode = input.GradeTypeCode;
                i.SchoolId = input.SchoolId;
                _context.GradeTypeWeights.Update(i);
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



