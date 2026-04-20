using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;

namespace GoodHamburger.Application.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<IEnumerable<MenuItemDto>> GetMenuAsync()
    {
        var items = await _menuRepository.GetAllAsync();
        return items.Select(i => new MenuItemDto
        {
            Id    = i.Id,
            Name  = i.Name,
            Price = i.Price,
            Type  = i.Type.ToString()
        });
    }
}
