using System;
using FluentAssertions;
using Molimentum.Models;
using Xunit;

namespace Molimentum.Tests.Models
{
    public class PostTests
    {
        [Fact]
        public void TheSlugCanBeSetIfItHasNotBeenSet()
        {
            var post = new Post();

            Action action = () => post.Slug = "slug1";

            action.ShouldNotThrow<InvalidOperationException>();
        }

        [Fact]
        public void TheSlugCanBeSetIfItIsEmpty()
        {
            var post = new Post { Slug = "" };

            Action action = () => post.Slug = "slug1";

            action.ShouldNotThrow<InvalidOperationException>();
        }

        [Fact]
        public void TheSlugCanBeSetToTheSameValue()
        {
            var post = new Post { Slug = "slug0" };

            Action action = () => post.Slug = "slug0";

            action.ShouldNotThrow<InvalidOperationException>();
        }

        [Fact]
        public void TheSlugCannotBeChanged()
        {
            var post = new Post { Slug = "slug0" };

            Action action = () => post.Slug = "slug1";

            action.ShouldThrow<InvalidOperationException>();
        }
    }
}
