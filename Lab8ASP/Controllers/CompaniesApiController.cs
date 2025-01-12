using Lab8ASP.Models.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab8ASP.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesApiController : ControllerBase
{
    private readonly MoviesContext _context;

    public CompaniesApiController(MoviesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetFiltered(string filter)
    {
        return Ok(_context.ProductionCompanies
            .Where(o => o.CompanyName.ToLower().Contains(filter.ToLower()))
            .OrderBy(o => o.CompanyName)
            .AsNoTracking()
            .AsEnumerable()
        );
    }
}