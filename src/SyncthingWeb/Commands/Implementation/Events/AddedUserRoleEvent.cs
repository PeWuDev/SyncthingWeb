namespace SyncthingWeb.Commands.Implementation.Events
{
    public class AddedUserRoleEvent
    {
        public AddedUserRoleEvent(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; private set; }
    }
}
