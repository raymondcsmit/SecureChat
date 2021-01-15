namespace Users.API.Application.Specifications.Attributes
{
    public class Sortable : SpecificationAttribute
    {
        public Sortable(string columnName = null) : base(columnName)
        {
        }
    }
}
