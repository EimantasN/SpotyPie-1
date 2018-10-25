﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models.BackEnd
{
    public class Copyright
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string Type { get; set; }

        public List<Album> Albums { get; set; }
    }
}
