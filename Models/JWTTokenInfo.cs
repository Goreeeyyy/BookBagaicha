namespace StudentManagement.Models
{
    public class JWTTokenInfo
    {
   
        public required string Issuer { get; set; }

        public required string Audience { get; set; }

        public required int ExpiresInMinutes { get; set; }
        public required String Key { get; set; }
    }
}


