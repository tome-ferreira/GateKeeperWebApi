namespace GateKeeperWebApiV1.Objects
{
    public class UserInfo
    {
        public string Name { get; }
        public string Email { get; }
        public string Id { get; }
        public string[] Roles { get; }
        public string Token { get; }    

        public UserInfo(string name, string email, string id, string[] role, string token)
        {
            Name = name;
            Email = email;
            Id = id;
            Roles = role;
            Token = token;
        }
    }
}
