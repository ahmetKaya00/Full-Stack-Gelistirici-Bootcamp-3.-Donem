namespace Ecommenrce.Api.Dtos.Auth
{
    
    public class RegisterDto
    {
        public string FullName {get;set;} = default!;
        public string Email {get;set;} = default!;
        public string Password{get;set;} = default!;
    }
}