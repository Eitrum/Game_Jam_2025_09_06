namespace Toolkit.Threading
{
    public interface IThreadedUpdate : INullable
    {
        void Update(float dt);
    }
}
