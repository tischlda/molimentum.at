using System.Collections.Generic;
using Molimentum.Models;

namespace Molimentum.ViewModels
{
    public class PhotosDetailViewModel
    {
        public Album Album { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}