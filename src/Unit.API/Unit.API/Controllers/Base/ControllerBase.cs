
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Unit.API.Controllers
{
    public class ControllerBase<T>: ControllerBase
    {
        private ILogger<T> _logger;
        internal ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();       
    }
}
