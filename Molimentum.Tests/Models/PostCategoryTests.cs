using FluentAssertions;
using Molimentum.Models;
using Xunit;

namespace Molimentum.Tests.Models
{
    public class PostCategoryTests
    {
        [Fact]
        public void PostCategoryCanBeAssignedFromCategory()
        {
            var category = new Category
            {
                Id = "category/1",
                Title = "TestTitle",
                Body = "TestBody"
            };

            PostCategory postCategory = category;

            postCategory.ShouldHave().SharedProperties().EqualTo(category);
        }
    }
}
