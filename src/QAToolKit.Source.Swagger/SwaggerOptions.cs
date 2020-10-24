using QAToolKit.Core.Models;

namespace QAToolKit.Source.Swagger
{
    public class SwaggerOptions
    {
        internal RequestFilter RequestFilter { get; private set; }
        internal bool UseBasicAuth { get; private set; } = false;
        internal bool UseRequestFilter { get; private set; } = false;
        internal string UserName { get; private set; }
        internal string Password { get; private set; }

        public SwaggerOptions AddBasicAuthentication(string userName, string password)
        {
            UseBasicAuth = true;
            UserName = userName;
            Password = password;
            return this;
        }

        public SwaggerOptions AddRequestFilters(RequestFilter requestFilter)
        {
            UseRequestFilter = true;
            RequestFilter = requestFilter;
            return this;
        }
    }
}
