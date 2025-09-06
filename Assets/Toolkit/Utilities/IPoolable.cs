namespace Toolkit
{
    public interface IPoolable
    {
        /// <summary>
        /// Whenever an poolable object gets created
        /// </summary>
        void OnPoolInitialize();

        /// <summary>
        /// Whenever an poolable object gets destroyed
        /// </summary>
        void OnPoolDestroy();
    }
}
