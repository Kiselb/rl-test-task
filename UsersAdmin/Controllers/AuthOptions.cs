using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UsersAdmin
{
    public class AuthOptions
    {
        public const string ISSUER = "https://localhost:5001";
        public const string AUDIENCE = "https://localhost:5001";
        const string KEY = "0123456789!@#$%^&*()_+";
        public const int LIFETIME = 10000;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }    
} 
