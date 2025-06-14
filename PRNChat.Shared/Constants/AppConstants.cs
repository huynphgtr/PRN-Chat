namespace PRNChat.Shared.Constants;

public static class AppConstants
{
    public static class ApiRoutes
    {
        public const string Auth = "/api/auth";
        public const string Chat = "/api/chat";
        public const string Contact = "/api/contact";
        public const string Bot = "/api/bot";
        
        public static class AuthEndpoints
        {
            public const string Login = Auth + "/login";
            public const string Register = Auth + "/register";
            public const string Logout = Auth + "/logout";
            public const string Profile = Auth + "/profile";
            public const string ChangePassword = Auth + "/change-password";
            public const string ResetPassword = Auth + "/reset-password";
        }
        
        public static class ChatEndpoints
        {
            public const string Rooms = Chat + "/rooms";
            public const string Messages = Chat + "/messages";
            public const string SendMessage = Chat + "/send";
        }
    }
    
    public static class Settings
    {
        public const string DefaultPageSize = "50";
        public const string MaxFileSize = "10485760"; // 10MB
        public const string SupportedImageFormats = ".jpg,.jpeg,.png,.gif,.bmp";
        public const string SupportedFileFormats = ".pdf,.doc,.docx,.txt,.zip,.rar";
    }
    
    public static class Messages
    {
        public const string LoginRequired = "You must be logged in to perform this action.";
        public const string InvalidCredentials = "Invalid email or password.";
        public const string RegistFailed = "Registation failed."; 
        public const string LoginFailed = "Login failed.";
        public const string ProfileFailed = "Create user failed."; 
        public const string UserNotFound = "User not found.";
        public const string ChatRoomNotFound = "Chat room not found.";
        public const string MessageNotFound = "Message not found.";
        public const string AccessDenied = "Access denied.";
        public const string ServerError = "An error occurred on the server.";
    }
}

