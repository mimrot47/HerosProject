using System.ComponentModel.DataAnnotations;

namespace HerosProject.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Uname { get; set; }
        public string FristName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
