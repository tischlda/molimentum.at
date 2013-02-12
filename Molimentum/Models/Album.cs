using System;
using System.Collections.Generic;

namespace Molimentum.Models
{
    public class Album : CommentableItemBase
    {
        public DateTimeOffset DateTime { get; set; }

        public DateTimeOffset? DateFrom { get; set; }

        public DateTimeOffset? DateTo { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public IEnumerable<PictureLink> ThumbnailLinks { get; set; }

        public IEnumerable<Picture> Pictures { get; set; }

        private string _slug;

        public string Slug
        {
            get
            {
                return _slug;
            }
            set
            {
                if (!String.IsNullOrEmpty(_slug) && value != _slug)
                    throw new InvalidOperationException("The Slug cannot be changed.");

                _slug = value;
            }
        }
    }
}