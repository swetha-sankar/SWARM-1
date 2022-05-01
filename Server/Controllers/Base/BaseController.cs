using Microsoft.AspNetCore.Http;
using SWARM.EF.Data;
using Microsoft.AspNetCore.Mvc;

namespace SWARM.Server.Controllers.Base
{
    [ApiController, Route("api/[controller]")]

    public class BaseController<T> : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }
    }
}