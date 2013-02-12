using System;
using FluentAssertions;
using Molimentum.Models;
using Xunit;

namespace Molimentum.Tests.Models
{
    public class CategoryTests
    {
        [Fact]
        public void TheSlugCanBeSetIfItHasNotBeenSet()
        {
            var category = new Category();

            Action action = () => category.Slug = "slug1";

            action.ShouldNotThrow<InvalidOperationException>();
        }

        [Fact]
        public void TheSlugCanBeSetIfItIsEmpty()
        {
            var category = new Category { Slug = "" };

            Action action = () => category.Slug = "slug1";

            action.ShouldNotThrow<InvalidOperationException>();
        }

        [Fact]
        public void TheSlugCanBeSetToTheSameValue()
        {
            var category = new Category { Slug = "slug0" };

            Action action = () => category.Slug = "slug0";

            action.ShouldNotThrow<InvalidOperationException>();
        }

        [Fact]
        public void TheSlugCannotBeChanged()
        {
            var category = new Category { Slug = "slug0" };

            Action action = () => category.Slug = "slug1";

            action.ShouldThrow<InvalidOperationException>();
        }
    }
}
