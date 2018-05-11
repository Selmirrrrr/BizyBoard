namespace BizyBoard.Web.Controllers
{
    using Bizy.OuinneBiseSharp.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BoardController : Controller
    {
        private OuinneBiseSharpService _service { get; }
        private ILogger<BoardController> _logger { get; }

        public BoardController(OuinneBiseSharpService service, ILogger<BoardController> logger)
        {
            _service = service;
            _logger = logger;
        }


    }
}