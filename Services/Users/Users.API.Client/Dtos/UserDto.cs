namespace Users.API.Client.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public ProfileDto Profile { get; set; }

    }
}
