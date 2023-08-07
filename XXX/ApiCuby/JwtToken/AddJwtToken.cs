using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiCuby.JwtToken
{
    public class AddJwtToken
    {
        public const string ISSUER = "CubyServer"; // издатель токена
        public const string AUDIENCE = "CubyClient"; // потребитель токена
        const string KEY = "myCubykey15432623234";   // ключ для шифрации

        //Method: Генерация ключа токина
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));

        //Method:Создание токена
        public static string AddToken(string email, string id)
        {            
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, email), new Claim(ClaimTypes.NameIdentifier, id) };//Создание клеймов

            var jwt = new JwtSecurityToken(
                    issuer: ISSUER,
                    audience: AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)), // время жизни токена
                    signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
