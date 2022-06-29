using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bierza.Data.Models;
using Microsoft.IdentityModel.Tokens;

namespace Bierza.Server.Tokens;

public class TokenService
{
    private const double ExpiryDurationMinutes = 30;

    public string BuildToken(string key, string issuer, UserBaseModel user)
    {
        var claims = new[] {    
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier,
                Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);           
        var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims, 
            expires: DateTime.Now.AddMinutes(ExpiryDurationMinutes), signingCredentials: credentials);        
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);  
    }
    
    public bool IsTokenValid(string key, string issuer, string token)
    {
        var mySecret = Encoding.UTF8.GetBytes(key);           
        var mySecurityKey = new SymmetricSecurityKey(mySecret);
        var tokenHandler = new JwtSecurityTokenHandler(); 
        try 
        {
            tokenHandler.ValidateToken(token, 
                new TokenValidationParameters   
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true, 
                    ValidateAudience = true,    
                    ValidIssuer = issuer,
                    ValidAudience = issuer, 
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);            
        }
        catch
        {
            return false;
        }
        return true;    
    }
}