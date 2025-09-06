namespace Toolkit.Serializable
{
    internal class Variable
    {
        public string Name;
        private int id;
        public int Id => (id == 0 ? id = Name.GetHash32() : id);
        public byte Type;
        public object Value;

        public Variable(string name, byte type, object value) {
            this.Name = name;
            this.id = 0;
            this.Type = type;
            this.Value = value;
        }

        public Variable(string name, int id, byte type, object value) {
            this.Name = name;
            this.id = id;
            this.Type = type;
            this.Value = value;
        }

        public Variable(int id, byte type, object value) {
            this.Name = "";
            this.id = id;
            this.Type = type;
            this.Value = value;
        }
    }
}
