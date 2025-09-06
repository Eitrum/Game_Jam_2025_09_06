namespace Toolkit.Serializable
{
    public interface ISerializableObject
    {
        void Save(Block block);
        void Load(Block block);
    }
}
