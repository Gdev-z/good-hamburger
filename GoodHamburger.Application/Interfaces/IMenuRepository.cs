using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Interfaces;

public interface IMenuRepository
{
    Task<IEnumerable<MenuItem>> GetAllAsync();
    Task<MenuItem?> GetByIdAsync(int id);
    Task<IEnumerable<MenuItem>> GetByIdsAsync(IEnumerable<int> ids);
}
