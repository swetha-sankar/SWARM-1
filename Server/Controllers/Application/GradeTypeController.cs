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
    public class GradeTypeController : BaseController<GradeType>, iBaseController<GradeType>
    {

        public GradeTypeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<GradeType> lstGradeType = await _context.GradeTypes.OrderBy(x => x.SchoolId).ThenBy(x => x.GradeTypeCode).ToListAsync();
            return Ok(lstGradeType);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Get/{school}/{type}")]
        public async Task<IActionResult> Get(int school, String type)
        {
            GradeType itmGradeType = await _context.GradeTypes.Where(x => x.SchoolId == school && x.GradeTypeCode == type).FirstOrDefaultAsync();
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
        public async Task<IActionResult> Delete(int school, String type)
        {
            GradeType itmGradeType = await _context.GradeTypes.Where(x => x.SchoolId == school && x.GradeTypeCode == type).FirstOrDefaultAsync();
            _context.Remove(itmGradeType);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GradeType input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var i = await _context.GradeTypes.Where(x => x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode).FirstOrDefaultAsync();

                if (i != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Grade type Already Exists.");
                }

                i = new GradeType();
                i.Description = input.Description;
                i.GradeTypeCode = input.GradeTypeCode;
                i.SchoolId = input.SchoolId;
                _context.GradeTypes.Add(i);
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
        public async Task<IActionResult> Put([FromBody] GradeType input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var i = await _context.GradeTypes.Where(x => x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode).FirstOrDefaultAsync();
                if (i == null)
                {
                    await this.Post(input);
                    return Ok();
                }
                i.Description = input.Description;
                i.GradeTypeCode = input.GradeTypeCode;
                i.SchoolId = input.SchoolId;
                _context.GradeTypes.Update(i);
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



