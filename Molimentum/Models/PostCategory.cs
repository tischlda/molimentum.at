namespace Molimentum.Models
{
    public class PostCategory
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public static implicit operator PostCategory(Category category)
        {
            if (category == null) return null;

            return new PostCategory
            {
                Id = category.Id,
                Title = category.Title,
                Slug = category.Slug
            };
        }
    }
}
