namespace dentistAi_api.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BsonCollectionAttribute : Attribute
    {
        public string Name { get; }

        public BsonCollectionAttribute(string name)
        {
            Name = name;
        }
    }
}
