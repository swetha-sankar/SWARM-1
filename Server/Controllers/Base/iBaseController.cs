using Microsoft.AspNetCore.Mvc;
using SWARM.EF.Models;
using System.Threading.Tasks;

namespace SWARM.Server.Controllers.Base
{
    public interface iBaseController<T>
    {
        Task<IActionResult> Get();
        Task<IActionResult> Get(int input);
        Task<IActionResult> Delete(int input);
        Task<IActionResult> Post([FromBody] T input);
        Task<IActionResult> Put([FromBody] T input);
    }
}