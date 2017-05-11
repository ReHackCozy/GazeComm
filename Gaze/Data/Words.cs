using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.Data
    {
    public class Words
        {
        public int Id { get; set; }
        public String Language { get; set; }
        public int count { get; set; }
        public List<String> words { get; set; }
        }
    }
