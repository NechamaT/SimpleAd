using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreePost.Data;

namespace FreePosts.Web.Models
{
    public class GetAllViewModel
    {
        public List<Post> Posts { get; set; }
        public List<int> Ids { get; set; }
    }
}
