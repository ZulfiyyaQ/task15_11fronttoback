namespace task15_11fronttoback.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductTags> ProductTags { get; set; }
    }
}
