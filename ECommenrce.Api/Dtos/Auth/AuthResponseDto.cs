namespace Ecommenrce.Api.Dtos.Auth
{ 
    public class AuthResponseDto
    {
        public string Token {get;set;} = default!;
        public string Email {get;set;} = default!;
        public string FullName{get;set;} = default!;
        public string Role {get;set;} = default!;
    }
}