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
    
    public partial class TestDrive
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> CarId { get; set; }
        public string BookDate { get; set; }
        public string TestDate { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
