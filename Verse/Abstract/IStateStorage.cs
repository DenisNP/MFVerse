namespace Verse.Abstract
{
    public interface IStateStorage<TState>
    {
        TState GetState(string userId);
        void SetState(string userId, TState state);
    }
}