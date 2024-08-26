using HerosProject.Data;
using HerosProject.Entity;
using System.Security.Cryptography;

namespace HerosProject.Handler
{
    public class RefreshHandler
    {
        private readonly DataContext _context;
        public RefreshHandler(DataContext dataContex)
        {
            _context = dataContex;
        }

        public async Task<String> CreateToken(string username)
        {
            var randonNumber = new Byte[31];
            using (var ramdonNumberGenerator = RandomNumberGenerator.Create())
            {
                ramdonNumberGenerator.GetBytes(randonNumber);
                string refreshToken = Convert.ToBase64String(randonNumber);
                var ExistToken = this._context.refreshtokens.FirstOrDefault(item => item.userID == username);
                if (ExistToken != null)
                {
                    ExistToken.refreshToken = refreshToken;
                }
                else {
                    await this._context.refreshtokens.AddAsync(new RefreshToken
                    {
                        userID = username,
                        tokenID = new Random().Next().ToString(),
                        refreshToken = refreshToken
                    });
                    await _context.SaveChangesAsync();
                }
                return refreshToken;
            } ;
        }
    }
}
