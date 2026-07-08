namespace LMS.Services
{
    public interface IAuthService {

        Task<string> GenerateJwtTokenAsync(string UserID, string USERGROUP, double tokenExpiryInMinutes);

    }

}
