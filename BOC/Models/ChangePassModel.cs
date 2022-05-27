﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOC.Models
{
    public class ChangePassModel 
    {
       
        public string Account { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string ConfirmPass { get; set; }
        public string ErrorMessage { get; set; }
        public int Success { get; set; }
    
    }
}
