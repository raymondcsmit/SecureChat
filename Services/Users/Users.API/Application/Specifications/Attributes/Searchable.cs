namespace Users.API.Application.Specifications.Attributes
{
    public class Searchable : SpecificationAttribute
    {
        public Searchable(string columnName = null) : base(columnName)
        {
        }
    }
}
