using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo.Authentication.Authentication
{
    /// <summary>
    /// Пример обработчика аутентификации с помощью JWT токена.
    /// В обработчике проверяем наличие токена и его валидность.
    /// В случае, если токен валидный, то аутентифицируем пользователя.
    /// </summary>
    public class JwtSchemeHandler : AuthenticationHandler<JwtSchemeOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Random _random;
        // Секретный ключ, используемый для создания токена и проверки полученного токена.
        // Так хранить небезопасно, лучше принимать через JwtSchemeOptions
        private const string SecretKey = "super_secret_key";

        public JwtSchemeHandler(
            IOptionsMonitor<JwtSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor)
            : base(options, logger, encoder, clock)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._random = new Random();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Получаем токен из заголовков запроса.
            // Для доступа к заголовкам, добавляем поле типа IHttpContextAccessor и добавляем его в конструктор.
            // При создании обработчика, ASP.NET автоматически добавить IHttpContextAccessor в данный обработчик.
            var tokenPresented = this
                ._httpContextAccessor
                .HttpContext
                .Request
                .Cookies
                .TryGetValue("jwt", out var token);

            // Если токена нет, то ничего не делаем - мы не можем аутентифицировать пользователя без токена
            if (!tokenPresented)
            {
                return Task.FromResult(AuthenticateResult.Fail("Токен отсутствует"));
            }

            // Сформировать токен можно здесь https://jwt.io/
            // (в качестве секретного ключа использовать JwtSchemeHandler.SecretKey)

            #region Проверка токена

            // Пример кода взят отсюда
            // https://stackoverflow.com/questions/38725038/c-sharp-how-to-verify-signature-on-jwt-token
            // Обычно руками токен не разбираем, есть специальные NuGet пакеты для работы с токеном

            var handler = new JwtSecurityTokenHandler();

            var parts = token.ToString().Split(".".ToCharArray());

            // Разбираем токен на части
            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];

            var bytesToSign = Encoding.UTF8.GetBytes($"{header}.{payload}");

            var secret = Encoding.UTF8.GetBytes(JwtSchemeHandler.SecretKey);

            var alg = new HMACSHA256(secret);
            var hash = alg.ComputeHash(bytesToSign);

            // Вычисляем подпись
            var calculatedSignature = JwtSchemeHandler.Base64UrlEncode(hash);

            // Сравниваем подпись с той, что в самом токене, чтоб убедиться,
            // что токен был зашифрован с таким же секретным ключом, который у нас
            if (!calculatedSignature.Equals(signature))
            {
                return Task.FromResult(AuthenticateResult.Fail("Токен невалидный"));
            }

            #endregion

            var jwtToken = handler.ReadJwtToken(token);

            // JWT токен может содержать в себе любую информацию, которую мы в него положили при формировании.
            // Забираем данные пользователя из токена и аутентифицируем пользователя.
            var claims = jwtToken.Claims;

            var identity = new ClaimsIdentity(claims, this.Scheme.Name);

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            var value = Convert.ToBase64String(bytes);

            value = value.Split('=')[0]; // Remove any trailing '='s
            value = value.Replace('+', '-'); // 62nd char of encoding
            value = value.Replace('/', '_'); // 63rd char of encoding

            return value;
        }
    }
}
