namespace UserCRUD.Utils
{
    public class CommonUtils
    {
        public static string GetToken(HttpContext context)
        {
            string authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                return token;
            }
            return string.Empty;
        }
    }
}
