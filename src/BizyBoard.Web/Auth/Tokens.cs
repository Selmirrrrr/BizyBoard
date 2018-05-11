namespace BizyBoard.Web.Auth
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Linq;

    public class Tokens
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions)
        {
            var response = new
            {
              id = identity.Claims.Single(c => c.Type == "id").Value,
              auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
              expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };
            var jwt =  JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return jwt;
        }
    }
}
