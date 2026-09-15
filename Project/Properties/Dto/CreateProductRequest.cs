namespace Project.Properties.Dto
{
    public record CreateProductRequest
    {
         public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
