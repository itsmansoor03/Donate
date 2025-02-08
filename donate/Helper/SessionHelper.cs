using donate.Models;

namespace donate.Helper
{
    public static class SessionHelper
    {
        public static void SetAsUser(HttpContext context)
        {
            context.Session.SetString("isUser", "True");
        }

        public static void UserInfo(HttpContext context, User user)
        {
            context.Session.SetString("username", user.Username);
            context.Session.SetString("usertype", user.UserType);
            context.Session.SetInt32("UserId", user.Id);
        }

        public static bool IsUser(HttpContext context)
        {
            return context.Session.Keys.Contains("isUser");
        }

        public static int GetCurrentUserId(HttpContext context)
        {
            return context.Session.GetInt32("UserId") ?? 0;
        }

        public static string GetUserName(HttpContext context)
        {
            return context.Session.GetString("username") ?? string.Empty;
        }

        public static string GetUserType(HttpContext context)
        {
            return context.Session.GetString("usertype") ?? string.Empty;
        }

        public static void ClearSession(HttpContext context)
        {
            context.Session.Clear();
        }
    }
}
