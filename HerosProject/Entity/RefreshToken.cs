namespace HerosProject.Entity
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string userID { get; set; }

        public string? tokenID { get; set; }

        public string? refreshToken { get; set; }
    }
}
