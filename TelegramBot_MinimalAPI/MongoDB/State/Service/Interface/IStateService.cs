namespace TelegramBot_MinimalAPI.MongoDB.State.Service.Interface
{
    public interface IStateService
    {
        Task<UserStates> GetStateAsync(long userId);
        Task<bool> SetState(long userId, UserStates state);
    }
}
