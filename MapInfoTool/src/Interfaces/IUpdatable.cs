
namespace MapInfoTool.Interfaces
{
    internal interface IUpdatable
    {
        /// <summary>
        /// Method to be called every update.
        /// </summary>
        /// <param name="gameTime">The time (in ms) since the start of the game.</param>
        void Update(int gameTime);
    }
}
