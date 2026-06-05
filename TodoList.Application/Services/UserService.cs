using AutoMapper;
using TodoList.Application.DTOs.User;
using TodoList.Application.Interfaces;

namespace TodoList.Application.Services;

/// <summary>Orchestrates user retrieval and DTO mapping operations.</summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>Initializes a new instance of the <see cref="UserService"/> class.</summary>
    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserResponseDto>>(users);
    }
}