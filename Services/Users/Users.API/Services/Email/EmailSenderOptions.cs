namespace Users.API.Services.Email
{
    public class EmailSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }

        public string Sender { get; set; }
    }
}
