using System.Collections.Generic;
using Molimentum.Models;

namespace Molimentum.ViewModels
{
    public class TagCloudViewModel
    {
        public IEnumerable<ClassifiedItem<TagCount>> ClassifiedTags { get; set; }
    }
}