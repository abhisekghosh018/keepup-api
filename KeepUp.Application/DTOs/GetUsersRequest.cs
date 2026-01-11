namespace KeepUp.Application.DTOs
{
    public record GetUsersRequest
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string DOB { get; set; }
    }
}
