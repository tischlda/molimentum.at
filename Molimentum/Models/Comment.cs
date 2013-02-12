using System;
using System.ComponentModel.DataAnnotations;
using Molimentum.Properties;

namespace Molimentum.Models
{
    public class Comment
    {
        public string Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName="TitleRequired")]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "BodyRequired")]
        public string Body { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AuthorRequired")]
        public string Author { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EmailNotValid")]
        public string Email { get; set; }

        [DataType(DataType.Url, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "UrlNotValid")]
        public string Website { get; set; }
    }
}