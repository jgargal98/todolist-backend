using AutoMapper;
using TodoList.Application.DTOs.User;
using TodoList.Application.Interfaces;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Fetches all users from the repository and maps them to UserResponseDto
    /// </summary>
    /// <returns>A list of data transfer objects representing users</returns>
    public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
    {
        // 1. Get entities from the Infrastructure layer (Repository)
        var users = await _userRepository.GetAllAsync();

        // 2. Map the entities to DTOs to hide sensitive information
        return _mapper.Map<IEnumerable<UserResponseDto>>(users);
    }
}