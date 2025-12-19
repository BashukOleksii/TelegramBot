using TelegramBot_MinimalAPI.MongoDB.State.Repository.Interface;
using TelegramBot_MinimalAPI.MongoDB.State.Service.Interface;

namespace TelegramBot_MinimalAPI.MongoDB.State.Service.Realization
{
    public class StateService : IStateService
    {
        private readonly IWeatherDataRepository _stateService;

        public StateService(IWeatherDataRepository stateService) 
        {
            _stateService = stateService; 
        }

        public async Task<UserStates> GetStateAsync(long userId)
        {
            var state = await _stateService.GetByUserIdAsync(userId);

            if (state is null)
                return UserStates.None;

            return state.UserStates;
        }

        public async Task<bool> SetState(long userId, UserStates state)
        {
            try
            {
                var userState = await _stateService.GetByUserIdAsync(userId);

                var newState = new CacheUserState() { UserId = userId, UserStates = state };

                if (userState is null)
                    await _stateService.CreateAsync(newState);
                else
                {
                    newState.Id = userState.Id;
                    await _stateService.UpdateAsync(newState);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
