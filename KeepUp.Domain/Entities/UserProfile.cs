namespace KeepUp.Domain.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; private set; }
        public string UserDisplayName { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateOnly? DOB { get; private set; }


        private UserProfile() { }

        public UserProfile(string userDisplayName, Guid applicationUserId, DateOnly? dob)
        {
            UserDisplayName = userDisplayName;
            CreatedAt = DateTime.UtcNow;
            ApplicationUserId = applicationUserId;
            DOB = dob;
        }
    }
}
