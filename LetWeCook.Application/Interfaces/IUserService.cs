using LetWeCook.Application.Dtos;

namespace LetWeCook.Application.Interfaces;

public interface IUserService
{
    Task SeedUsersAsync(List<SeedUserDTO> seedUserDTOs, CancellationToken cancellationToken = default);
    Task SeedUserProfiles(CancellationToken cancellationToken = default);

}