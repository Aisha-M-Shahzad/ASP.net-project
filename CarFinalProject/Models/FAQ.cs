//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarFinalProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FAQ
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
