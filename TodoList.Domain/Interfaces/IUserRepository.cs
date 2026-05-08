using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

public interface IUserRepository
{
    // The repository always works with Domain Entities
    Task<IEnumerable<User>> GetAllAsync();
}