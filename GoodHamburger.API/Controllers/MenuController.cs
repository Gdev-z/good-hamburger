using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>Retorna o cardápio completo da lanchonete.</summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var menu = await _menuService.GetMenuAsync();
        return Ok(menu);
    }
}
