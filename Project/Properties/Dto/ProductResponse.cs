namespace Project.Properties.Dto
{
    public record ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
